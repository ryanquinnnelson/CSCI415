using System;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SynapseModel3
{
    public class Neuron
    {
        //fields
        private CellBody body;                                  //shared
        private ConcurrentDictionary<int, Dendrite> dendrites;  //shared
        private int[] growthEventDendriteTypesList;
        private SecondaryMessenger messenger;                   //shared
        private int nextDendriteId;
        private int numDendritesToAddInGrowthEvent;
        private int state; //0 for no growth; 1 for growth      //shared


        //for each dendrite to be added
        private int d_DecayFrequency;
        private int d_ProductionFrequency;
        private int d_RestoreIncrement;
        private int d_NumSynapsesToAddInGrowthEvent;
        private TimeSpan d_SecondaryMessengerWindow;
        private int d_SecondaryMessengerFrequencyTrigger;
        private int d_NumStartingSynapsesPerDendrite;
        private int d_SignificantVoltageChange;


        //constructors
        public Neuron(int cellBodyDecayFrequency,
                      int cellBodyRestoreIncrement,
                      TimeSpan neuronSecondaryMessengerWindow,
                      int neuronFrequencyTrigger,
                      int numDendritesToAddInGrowthEvent,
                      int[] growthEventDendriteTypesList,

                      int dendriteDecayFrequency,
                      int dendriteProductionFrequency,
                      int dendriteRestoreIncrement,
                      int dendriteNumSynapsesToAddInGrowthEvent,
                      TimeSpan dendriteSecondaryMessengerWindow,
                      int dendriteSecondaryMessengerFrequencyTrigger,
                      int numStartingSynapsesPerDendrite,
                      int dendriteSignificantVoltageChange,

                      int numStartingDendrites,
                      int[] startingDendriteTypesList)
        {
            nextDendriteId = 0;
            state = 0;
            dendrites = new ConcurrentDictionary<int, Dendrite>();

            body = new CellBody(DateTime.Now, cellBodyDecayFrequency, cellBodyRestoreIncrement);
            messenger = new SecondaryMessenger(DateTime.Now, neuronFrequencyTrigger, neuronSecondaryMessengerWindow);
            this.numDendritesToAddInGrowthEvent = numDendritesToAddInGrowthEvent;
            this.growthEventDendriteTypesList = growthEventDendriteTypesList;

            //individual dendrite characteristics
            this.d_DecayFrequency = dendriteDecayFrequency;
            this.d_RestoreIncrement = dendriteRestoreIncrement;
            this.d_ProductionFrequency = dendriteProductionFrequency;
            this.d_NumSynapsesToAddInGrowthEvent = dendriteNumSynapsesToAddInGrowthEvent;
            this.d_SecondaryMessengerWindow = dendriteSecondaryMessengerWindow;
            this.d_SecondaryMessengerFrequencyTrigger = dendriteSecondaryMessengerFrequencyTrigger;
            this.d_NumStartingSynapsesPerDendrite = numStartingSynapsesPerDendrite;
            this.d_SignificantVoltageChange = dendriteSignificantVoltageChange;


            //initialize dendrites
            AddDendrites(numStartingDendrites, startingDendriteTypesList);

            //add listener for ActionPotential event
            body.ActionPotentialEvent += ReceiveActionPotentialEvent;
        }


        //properties
        public CellBody Body //tested
        {
            get
            {
                return this.body;
            }
            private set
            {
                body = value;
            }
        }

        public ICollection<Dendrite> Dendrites
        {
            get
            {
                return this.dendrites.Values;
            }
            private set
            {
                dendrites = (ConcurrentDictionary<int, Dendrite>)value;
            }
        }

        public int State //tested
        {
            get
            {
                return this.state;
            }
            private set
            {
                state = value;
            }
        }


        //public methods
        public Dendrite GetDendrite(int id) //tested
        {
            Dendrite result = dendrites.GetOrAdd(id, (key) => null);
            return result;
        }

        public Dendrite SearchForOpenSynapse(InputAxon axon) //tested
        {
            Dendrite dendrite = null;

            foreach (Dendrite current in dendrites.Values)
            {
                bool success = current.TryConnect(axon);
                if (success)
                {
                    Console.WriteLine("Neuron found open synapse on dendrite.");
                    dendrite = current;
                    break; //stop searching
                }
            }

            return dendrite;
        }

        public override string ToString() //tested
        {
            return "Neuron{\n\tnextDendriteId=" + nextDendriteId + ",\n\tstate="
                + state + ",\n\tbody=" + body + ",\n\tdendrites="
                + DendritesListToString() + ",\n\tmessenger=" + messenger
                + ",\n\tnumDendritesToAddInGrowthEvent="
                + numDendritesToAddInGrowthEvent 
                + ",\n\tgrowthEventDendriteTypesList=" 
                + GrowthEventDendriteTypesListToString() 
                + ",\n\n\td_DecayFrequency=" + d_DecayFrequency
                + ",\n\td_ProductionFrequency=" + d_ProductionFrequency 
                + ",\n\td_RestoreIncrement=" + d_RestoreIncrement
                + ",\n\td_NumSynapsesToAddInGrowthEvent="
                + d_NumSynapsesToAddInGrowthEvent 
                + ",\n\td_SecondaryMessengerWindow="
                + d_SecondaryMessengerWindow 
                + ",\n\td_SecondaryMessengerFrequencyTrigger=" 
                + d_SecondaryMessengerFrequencyTrigger
                + ",\n\td_NumStartingSynapsesPerDendrite=" 
                + d_NumStartingSynapsesPerDendrite +"\n}";
        }




        //private helper methods
        private void AddDendrites(int numToAdd, int[] typesList)
        {
            if (typesList.Length != numToAdd)
            {
                throw new ArgumentException("Number of dendrites to be created doesn't match number of elements in type list.");
            }

            for (int i = 0; i < numToAdd; i++)
            {
                Dendrite newest = new Dendrite(nextDendriteId++,
                                               typesList[i],
                                               d_DecayFrequency,
                                               d_ProductionFrequency,
                                               d_RestoreIncrement,
                                               d_NumSynapsesToAddInGrowthEvent,
                                               d_SecondaryMessengerWindow,
                                               d_SecondaryMessengerFrequencyTrigger,
                                               d_NumStartingSynapsesPerDendrite,
                                               d_SignificantVoltageChange);

                dendrites.AddOrUpdate(newest.Id, newest, (key, oldValue) => oldValue);
            }
        }

        private void CheckCellGrowthEventThreshold() //tested
        {
            if (messenger.IsGrowthStateTriggered(DateTime.Now))
            {
                SetGrowthState();
            }
        }

        private string DendritesListToString() //tested
        {
            StringBuilder sb = new StringBuilder("[");
            foreach (Dendrite d in dendrites.Values)
            {
                if(d != null)
                {
                    sb.Append(d.Id);
                    sb.Append(",");
                }
            }
            if (dendrites.Count > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("]");
            return sb.ToString();
        }

        private string GrowthEventDendriteTypesListToString() //tested
        {
            StringBuilder sb = new StringBuilder("[");

            for (int i = 0; i < growthEventDendriteTypesList.Length; i++)
            {
                sb.Append(growthEventDendriteTypesList[i]);
                sb.Append(",");
            }
            if (growthEventDendriteTypesList.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("]");


            return sb.ToString();
        }

        private void SetGrowthState() //tested
        {
            int original = Interlocked.CompareExchange(ref state, 1, 0);
            if (original == 0)
            {
                RaiseCellGrowthEvent(DateTime.Now); //raise event to ProcessManager
            }
        }

        private void SetNoGrowthState() //tested
        {
            Interlocked.CompareExchange(ref state, 0, 1);
        }


        //ActionPotentialEvent code
        public void ReceiveActionPotentialEvent(object sender, EventArgs_ActionPotential e) //tested
        {
            Console.WriteLine("Neuron receives action potential event.");

            //add to secondary messenger for checking
            messenger.AddEvent(e.When);

            //check whether threshold is met for cell growth event
            CheckCellGrowthEventThreshold();
        }




        //CellGrowthEvent code
        public void RaiseCellGrowthEvent(DateTime when) //tested
        {
            //action within the cell
            AddDendrites(numDendritesToAddInGrowthEvent, growthEventDendriteTypesList);

            //reset neuron state
            SetGrowthState();

            //event
            Console.WriteLine("Neuron raises cell growth event.");
            EventArgs_CellGrowth args = new EventArgs_CellGrowth(when);
            OnCellGrowthEvent(args);
        }

        protected virtual void OnCellGrowthEvent(EventArgs_CellGrowth e)
        {
            EventHandler<EventArgs_CellGrowth> handler = CellGrowthEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<EventArgs_CellGrowth> CellGrowthEvent;


        //tests
        //public static void Main()
        //{
            //Console.WriteLine("Test of Constructor 1");
            //Neuron n = new Neuron(100, 50, new TimeSpan(0, 0, 2), 1, 1, new int[] { 0 }, 100, 100, 50, 1, new TimeSpan(0, 0, 2), 100, 1, 2, new int[] { 0,1 });
            //Console.WriteLine(n);
            //Console.WriteLine();

            //Console.WriteLine("Test of GetDendrite() success");
            //Console.WriteLine(n.GetDendrite(0));
            //Console.WriteLine();

            //Console.WriteLine("Test of GetDendrite() failure");
            //Console.WriteLine(n.GetDendrite(3));
            //Console.WriteLine();

            //Console.WriteLine("Test of GetBody()");
            //Console.WriteLine(n.Body);
            //Console.WriteLine();

            //Console.WriteLine("Test of GetState()");
            //Console.WriteLine(n.State);
            //Console.WriteLine();

            //Console.WriteLine("Test of SearchForOpenSynapse() with synapse available");
            //InputAxon axon1 = new InputAxon(1, 100, 0);
            //Console.WriteLine(n.SearchForOpenSynapse(axon1));
            //Console.WriteLine();

            //Console.WriteLine("Test of SearchForOpenSynapse() with no synapse available");
            //Console.WriteLine(n.SearchForOpenSynapse(axon1));
            //Console.WriteLine();

            //Console.WriteLine("Test of AddDendrite() with incorrect number in list");
            //try
            //{
            //    n.AddDendrites(1, new int[] { 1, 1 });
            //}
            //catch (ArgumentException ae)
            //{
            //    Console.WriteLine(ae);
            //}
            //Console.WriteLine();

            //Console.WriteLine("Test of AddDendrite() with correct number in list");
            //n.AddDendrites(2, new int[] { 1, 1 });
            //Console.WriteLine(n.DendritesListToString());
            //Console.WriteLine();



            //Console.WriteLine("Test of CheckGrowthEventThreshold() threshold not met");
            //n.CheckCellGrowthEventThreshold();
            //Console.WriteLine();

            //Console.WriteLine("Test of CheckGrowthEventThreshold() threshold met");
            //n.messenger = new SecondaryMessenger(DateTime.Now, 1, new TimeSpan(0, 0, 5));
            ////n.Body.RaiseActionPotentialEvent(DateTime.Now); //make public to test
            //n.CheckCellGrowthEventThreshold();
            //Console.WriteLine(n);
            //Console.WriteLine();
        //}
    }
}
