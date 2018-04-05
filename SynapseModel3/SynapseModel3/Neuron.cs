using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

namespace SynapseModel3
{
    public class Neuron
    {
        //fields
        private int nextDendriteId;
        private int state; //0 for no growth; 1 for growth
        private CellBody body;
        private List<Dendrite> dendrites;
        private SecondaryMessenger messenger;


        //constructors
        public Neuron(int cellBodyDecayFrequency, int dendriteDecayFrequency, int dendriteProductionFrequency)
        {
            nextDendriteId = 0;
            state = 0;
            body = new CellBody(DateTime.Now, cellBodyDecayFrequency);
            dendrites = new List<Dendrite>();

            //look at the previous 2 seconds
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 2;
            int milliseconds = 0;
            TimeSpan window = new TimeSpan(days, hours, minutes, seconds, milliseconds);

            messenger = new SecondaryMessenger(DateTime.Now, 100, window);

            //initialize dendrites
            AddDendrites(1, new int[] { 0 }, 1, dendriteDecayFrequency, dendriteProductionFrequency);

            //add listener for ActionPotential event
            body.ActionPotentialEvent += ReceiveActionPotentialEvent;
        }


        //properties
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


        //public methods
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

        public Dendrite GetDendrite(int id){
            
            Dendrite target = null;

            foreach(Dendrite d in dendrites){
                if(d.Id == id){
                    target = d;
                    break;
                }
            }

            return target;
        }


        //private helper methods
        private void AddDendrites(int num, int[] typeList, int numAvailableSynapses, int dendriteDecayFrequency, int dendriteProductionFrequency)
        {
            if (typeList.Length != num)
            {
                throw new ArgumentException("Number of dendrites to be created doesn't match number of elements in type list.");
            }

            for (int i = 0; i < num; i++)
            {
                Dendrite newest = new Dendrite(nextDendriteId, typeList[i], numAvailableSynapses, dendriteDecayFrequency, dendriteProductionFrequency);
                dendrites.Add(newest);
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

        public void CheckCellGrowthEventThreshold() //tested
        {
            if (messenger.IsGrowthStateTriggered(DateTime.Now))
            {
                state = 1;
                RaiseCellGrowthEvent(); //send event to ProcessManager
            }
        }


        //CellGrowthEvent code
        public void RaiseCellGrowthEvent() //tested
        {
            //action within the cell
            AddDendrites(1, new int[] { 0 }, 5, 100, 100); //??hardcoded values should be changed

            //reset neuron state
            state = 0;

            //event
            Console.WriteLine("Neuron raises cell growth event.");
            EventArgs_CellGrowth args = new EventArgs_CellGrowth(DateTime.Now);
            OnCellGrowthTriggered(args);
        }

        protected virtual void OnCellGrowthTriggered(EventArgs_CellGrowth e)
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
            //Neuron n = new Neuron();
            //Console.WriteLine(n);
            //Console.WriteLine();

            //Console.WriteLine("Test of GetDendrite()");
            //Console.WriteLine(n.GetDendrite(0));
            //Console.WriteLine();
        
        

        //    Console.WriteLine("Test of GetBody()");
        //    Console.WriteLine(n.Body);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetState()");
        //    Console.WriteLine(n.State);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetDendrites()");
        //    Console.WriteLine(n.Dendrites);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of AddDendrite() with incorrect number in list");
        //    try
        //    {
        //        n.AddDendrites(1, new int[] { 1, 1 }, 100);
        //    }
        //    catch (ArgumentException ae)
        //    {
        //        Console.WriteLine(ae);
        //    }
        //    Console.WriteLine();

        //    Console.WriteLine("Test of AddDendrite() with correct number in list");
        //    n.AddDendrites(2, new int[] { 1, 1 }, 100);
        //    Console.WriteLine(n.OutputDendritesList());
        //    Console.WriteLine();

        //    Console.WriteLine("Test of SearchForOpenSynapse() with synapse available");
        //    Console.WriteLine(n.SearchForOpenSynapse(0));
        //    Console.WriteLine();

        //    Console.WriteLine("Test of SearchForOpenSynapse() with no synapse available");
        //    Console.WriteLine(n.SearchForOpenSynapse(2));
        //    Console.WriteLine();

        //    Console.WriteLine("Test of CheckGrowthEventThreshold() threshold not met");
        //    n.CheckCellGrowthEventThreshold();
        //    Console.WriteLine();

        //    Console.WriteLine("Test of CheckGrowthEventThreshold() threshold met");
        //    n.messenger = new SecondaryMessenger(DateTime.Now, 1, new TimeSpan(0, 0, 5));
        //    //n.Body.RaiseActionPotentialEvent(DateTime.Now); //make public to test
        //    n.CheckCellGrowthEventThreshold();
        //    Console.WriteLine();
        //}
    }
}
