using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Threading;

namespace SynapseModel3
{
    public class Neuron
    {
        //fields
        private CellBody body;
        private List<Dendrite> dendrites;
        private int[] growthEventDendriteTypesList;
        private SecondaryMessenger messenger;
        private int nextDendriteId;
        private int numDendritesToAddInGrowthEvent;
        private int state; //0 for no growth; 1 for growth


        //for each dendrite to be added
        private int d_DecayFrequency;
        private int d_ProductionFrequency;
        private int d_RestoreIncrement;
        private int d_NumSynapsesToAddInGrowthEvent;
        private TimeSpan d_SecondaryMessengerWindow;
        private int d_SecondaryMessengerFrequencyTrigger;
        private int d_NumStartingSynapsesPerDendrite;


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

                      int numStartingDendrites,
                      int[] startingDendriteTypesList)
        {
            nextDendriteId = 0;
            state = 0;
            dendrites = new List<Dendrite>();

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


            //initialize dendrites
            AddDendrites(numStartingDendrites,
                         startingDendriteTypesList,

                         d_DecayFrequency,
                         d_ProductionFrequency,
                         d_RestoreIncrement,
                         d_NumSynapsesToAddInGrowthEvent,
                        d_SecondaryMessengerWindow,
                        d_SecondaryMessengerFrequencyTrigger,
                        d_NumStartingSynapsesPerDendrite);

            //add listener for ActionPotential event
            body.ActionPotentialEvent += ReceiveActionPotentialEvent;
        }


        //properties
        public CellBody Body
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

        public List<Dendrite> Dendrites
        {
            get
            {
                return this.dendrites;
            }
            private set
            {
                dendrites = value;
            }
        }

        public int State
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
        public string DendritesListToString() //tested
        {
            StringBuilder sb = new StringBuilder("[");
            foreach (Dendrite d in dendrites)
            {
                sb.Append(d.Id);
                sb.Append(",");
            }
            if (dendrites.Count > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("]");
            return sb.ToString();
        }

        public Dendrite GetDendrite(int id)
        {

            Dendrite target = null;

            foreach (Dendrite d in dendrites)
            {
                if (d.Id == id)
                {
                    target = d;
                    break;
                }
            }

            return target;
        }

        public Synapse SearchForOpenSynapse(int dendriteType) //tested
        {
            Synapse openSynapse = null;

            foreach (Dendrite d in dendrites)
            {
                if (d.Type == dendriteType && d.NumAvailableSynapses > 0)
                {
                    openSynapse = d.GetOpenSynapse();
                }
            }

            return openSynapse;
        }

        public override string ToString() //tested
        {
            return "Neuron{ nextDendriteId=" + nextDendriteId + ", state="
                + state + ", body=" + body + ", dendrites="
                + DendritesListToString() + ", messenger=" + messenger + " }";
        }




        //private helper methods
        private void AddDendrites(int numToAdd,
                                  int[] typesList,
                                  int decayFrequency,
                                  int productionFrequency,
                                  int restoreIncrement,
                                  int numSynapsesToAddInGrowthEvent,
                                  TimeSpan window,
                                  int frequencyTrigger,
                                  int numStartingSynapses)
        {
            if (typesList.Length != numToAdd)
            {
                throw new ArgumentException("Number of dendrites to be created doesn't match number of elements in type list.");
            }

            for (int i = 0; i < numToAdd; i++)
            {
                Dendrite newest = new Dendrite(nextDendriteId++,
                                               typesList[i],
                                               decayFrequency,
                                               productionFrequency,
                                               restoreIncrement,
                                               numSynapsesToAddInGrowthEvent,
                                               window,
                                               frequencyTrigger,
                                               numStartingSynapses);
                dendrites.Add(newest);
            }
        }

        private void CheckCellGrowthEventThreshold()
        {
            if (messenger.IsGrowthStateTriggered(DateTime.Now))
            {
                Interlocked.CompareExchange(ref state, 1, 0);
                RaiseCellGrowthEvent(DateTime.Now); //send event to ProcessManager
            }
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
        public void RaiseCellGrowthEvent(DateTime when)
        {
            //action within the cell
            AddDendrites(numDendritesToAddInGrowthEvent,
                         growthEventDendriteTypesList,
                         d_DecayFrequency,
                         d_ProductionFrequency,
                         d_RestoreIncrement,
                         d_NumSynapsesToAddInGrowthEvent,
                         d_SecondaryMessengerWindow,
                         d_SecondaryMessengerFrequencyTrigger,
                         d_NumStartingSynapsesPerDendrite);

            //reset neuron state
            Interlocked.CompareExchange(ref state, 0, 1);

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
            //Neuron n = new Neuron(100,50,new TimeSpan(0,0,2), 100, 1, new int[]{0}, 100, 100, 50, 1, new TimeSpan(0,0,2), 100, 1, 1, new int []{0});
            //Console.WriteLine(n);
            //Console.WriteLine();

            //Console.WriteLine("Test of GetDendrite()");
            //Console.WriteLine(n.GetDendrite(0));
            //Console.WriteLine();

            //Console.WriteLine("Test of GetBody()");
            //Console.WriteLine(n.Body);
            //Console.WriteLine();

            //Console.WriteLine("Test of GetState()");
            //Console.WriteLine(n.State);
            //Console.WriteLine();

            //Console.WriteLine("Test of GetDendrites()");
            //Console.WriteLine(n.Dendrites);
            //Console.WriteLine();

            //Console.WriteLine("Test of AddDendrite() with incorrect number in list");
            //try
            //{
            //    n.AddDendrites(1, new int[] { 1, 1 }, 100, 100, 50, 1, new TimeSpan(0,0,2), 100, 1);
            //}
            //catch (ArgumentException ae)
            //{
            //    Console.WriteLine(ae);
            //}
            //Console.WriteLine();

            //Console.WriteLine("Test of AddDendrite() with correct number in list");
            //n.AddDendrites(2, new int[] { 1, 1 }, 100, 100, 50, 1, new TimeSpan(0, 0, 2), 100, 1);
            //Console.WriteLine(n.DendritesListToString());
            //Console.WriteLine();

            //Console.WriteLine("Test of SearchForOpenSynapse() with synapse available");
            //Console.WriteLine(n.SearchForOpenSynapse(0));
            //Console.WriteLine();

            //Console.WriteLine("Test of SearchForOpenSynapse() with no synapse available");
            //Console.WriteLine(n.SearchForOpenSynapse(2));
            //Console.WriteLine();

            //Console.WriteLine("Test of CheckGrowthEventThreshold() threshold not met");
            //n.CheckCellGrowthEventThreshold();
            //Console.WriteLine();

            //Console.WriteLine("Test of CheckGrowthEventThreshold() threshold met");
            //n.messenger = new SecondaryMessenger(DateTime.Now, 1, new TimeSpan(0, 0, 5));
            ////n.Body.RaiseActionPotentialEvent(DateTime.Now); //make public to test
            //n.CheckCellGrowthEventThreshold();
            //Console.WriteLine();
        //}
    }
}
