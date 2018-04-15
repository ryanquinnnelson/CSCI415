using System;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace SynapseModel3
{
    public class Dendrite
    {
        //constants
        public const int RESTING_POTENTIAL = -70000; //Volts


        //fields
        private BlockingCollection<Neurotransmitter> buffer; //shared
        private int decayFrequency;
        private int id;
        private int membranePotential; //Volts
        private SecondaryMessenger messenger;
        private int nextSynapseId;
        private int numAvailableSynapses;
        private int numSynapsesToAddInGrowthEvent;
        private int productionFrequency;
        private int restoreIncrement;
        private int state; //0 no growth; 1 growth
        private List<Synapse> synapses;
        private int type; //0 is proximal; 1 is basal; 2 is apical


        //constructors
        public Dendrite(int id,
                        int type,
                        int decayFrequency,
                        int productionFrequency,
                        int restoreIncrement,
                        int numSynapsesToAddInGrowthEvent,
                        TimeSpan secondaryMessengerWindow,
                        int secondaryMessengerFrequencyTrigger,
                        int numStartingSynapses)
        {
            state = 0;
            nextSynapseId = 0;
            membranePotential = RESTING_POTENTIAL;
            buffer = new BlockingCollection<Neurotransmitter>(new ConcurrentQueue<Neurotransmitter>());
            synapses = new List<Synapse>();

            this.id = id;
            this.type = type;
            this.decayFrequency = decayFrequency;
            this.productionFrequency = productionFrequency;
            this.restoreIncrement = restoreIncrement;
            this.numSynapsesToAddInGrowthEvent = numSynapsesToAddInGrowthEvent;
            messenger = new SecondaryMessenger(DateTime.Now, secondaryMessengerFrequencyTrigger, secondaryMessengerWindow);
            this.numAvailableSynapses = numStartingSynapses;

            CreateSynapses(numStartingSynapses);
        }


        //properties
        public int DecayFrequency
        {
            get
            {
                return this.decayFrequency;
            }
            private set
            {
                decayFrequency = value;
            }
        }

        public int Id //tested
        {
            get
            {
                return this.id;
            }
            private set
            {
                id = value;
            }
        }

        public int MembranePotential //tested
        {
            get
            {
                return this.membranePotential;
            }
            private set
            {
                membranePotential = value;
            }
        }

        public int NumAvailableSynapses //tested
        {
            get
            {
                return numAvailableSynapses;
            }
            private set
            {
                numAvailableSynapses = value;
            }
        }

        public int ProductionFrequency
        {
            get
            {
                return this.productionFrequency;
            }
            private set
            {
                productionFrequency = value;
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

        public int Type //tested
        {
            get
            {
                return type;
            }
            private set
            {
                type = value;
            }
        }


        //public methods
        public void AddToBuffer(Neurotransmitter nt)
        {
            buffer.Add(nt);

            //decide when to add events to SecondaryMessenger


            //check whether dendrite growth state threshold reached
            if (state == 0 && messenger.IsGrowthStateTriggered(DateTime.Now))
            {
                SetGrowthState();
            }
        }

        public void DecayMembranePotential()
        {
            if (membranePotential < RESTING_POTENTIAL)
            {
                Interlocked.Add(ref membranePotential, restoreIncrement);
            }
            else if (membranePotential > RESTING_POTENTIAL)
            {
                Interlocked.Add(ref membranePotential, -restoreIncrement);
            }
            else
            {
                //do nothing
            }
        }

        public bool FormSynapseConnection(Synapse synapse, InputAxon axon)
        {
            if (numAvailableSynapses > 0 && !synapse.IsConnectionAlreadyFormed())
            {
                synapse.Connect(axon);
                Interlocked.Decrement(ref numAvailableSynapses);
                return true;
            }

            return false;
        }

        public int GetMembranePotentialDifference()
        {
            return (membranePotential - RESTING_POTENTIAL);
        }


        public Synapse GetOpenSynapse() //tested
        {
            Synapse open = null;

            foreach (Synapse s in synapses)
            {
                if (!s.IsConnectionAlreadyFormed())
                {
                    open = s;
                    break; //stop searching
                }
            }

            return open;
        }

        public Synapse GetSynapse(int id) //tested
        {
            Synapse open = null;

            foreach (Synapse s in synapses)
            {
                if (s.Id == id)
                {
                    open = s;
                    break; //stop searching
                }
            }

            return open;

        }

        public int TryRemoveFromBuffer() //tested
        {
            Neurotransmitter removed = buffer.Take();
            int charge = removed.Charge;

            //affect local membrane potential
            Interlocked.Add(ref membranePotential, charge);

            return charge;
        }

        public override string ToString() //tested
        {
            return "Dendrite{ id=" + id + ", type=" + type + ", state="
                + state + ", numAvailableSynapses=" + numAvailableSynapses
                + ", membranePotential=" + membranePotential + ", buffer="
                + buffer + ", messenger=" + messenger + ", nextSynapseId=" +
                nextSynapseId + ", synapses=" + SynapsesListToString() + " }";

        }


        //private helper methods
        private void CreateSynapses(int num) //tested
        {
            for (int i = 0; i < num; i++)
            {
                Synapse newest = new Synapse(nextSynapseId++);
                synapses.Add(newest);
            }
        }

        private void SetGrowthState() //tested
        {
            //Console.WriteLine("SetGrowthState()");
            int original = Interlocked.CompareExchange(ref state, 1, 0);
            if (original == 0)
            {
                RaiseDendriteGrowthEvent(DateTime.Now); //raise event to alert ProcessManager
            }
        }

        private void SetNoGrowthState() //tested
        {
            //Console.WriteLine("SetNoGrowthState()");
            Interlocked.CompareExchange(ref state, 0, 1);
        }

        private string SynapsesListToString() //tested
        {
            StringBuilder sb = new StringBuilder("[");
            foreach (Synapse s in synapses)
            {
                sb.Append(s);
                sb.Append(",");
            }
            if (synapses.Count > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("]");
            return sb.ToString();
        }



        //DendriteGrowthEvent code
        private void RaiseDendriteGrowthEvent(DateTime when) //tested
        {
            Console.WriteLine("Dendrite Growth event raised.");

            //action inside dendrite
            numAvailableSynapses++;
            CreateSynapses(numSynapsesToAddInGrowthEvent);

            //restore no growth state
            SetNoGrowthState();


            //event
            EventArgs_DendriteGrowth args = new EventArgs_DendriteGrowth(when);
            OnDendriteGrowthEvent(args);
        }

        protected virtual void OnDendriteGrowthEvent(EventArgs_DendriteGrowth e)
        {
            EventHandler<EventArgs_DendriteGrowth> handler = DendriteGrowthEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<EventArgs_DendriteGrowth> DendriteGrowthEvent;


        ////tests
        //public static void Main()
        //{
        //    Console.WriteLine("Test of Constructor 1");
        //    Dendrite d = new Dendrite(1, 1, 5, 100, 50, 1, new TimeSpan(0, 0, 2), 100, 1);
        //    Console.WriteLine(d);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetType()");
        //    Console.WriteLine(d.Type);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetId()");
        //    Console.WriteLine(d.Id);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetState()");
        //    Console.WriteLine(d.State);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetNumAvailableSynapses()");
        //    Console.WriteLine(d.NumAvailableSynapses);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetMembranePotential()");
        //    Console.WriteLine(d.MembranePotential);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of AddToBuffer()");
        //    d.AddToBuffer(new Neurotransmitter(-10));
        //    Console.WriteLine("Buffer count: " + d.buffer.Count);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of TryRemoveFromBuffer()");
        //    Console.WriteLine(d.TryRemoveFromBuffer());
        //    Console.WriteLine("Buffer count: " + d.buffer.Count);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of DecayMembranePotential()");
        //    d.MembranePotential = -90000;
        //    d.DecayMembranePotential();
        //    Console.WriteLine(d.MembranePotential);
        //    Console.WriteLine();
        //    d.MembranePotential = -70000;

        //    d.messenger = new SecondaryMessenger(DateTime.Now, 1, new TimeSpan(0, 0, 5));
        //    d.AddToBuffer(new Neurotransmitter(-10));

        //    Console.WriteLine("Test of GetOpenSynapse()");
        //    Console.WriteLine(d.GetOpenSynapse());
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetSynapse()");
        //    Console.WriteLine(d.GetSynapse(0));
        //    Console.WriteLine();

        //    Console.WriteLine("Test of FormSynapseConnection()");
        //    Synapse current = d.GetSynapse(0);
        //    Console.WriteLine(d.FormSynapseConnection(current, new InputAxon(1, 1, 0)));
        //    Console.WriteLine(d.GetSynapse(0));
        //    Console.WriteLine();

        //    Console.WriteLine("Test of RaiseDendriteGrowthEvent()");
        //    d.RaiseDendriteGrowthEvent(DateTime.Now);
        //    Console.WriteLine(d.SynapsesListToString());
        //    Console.WriteLine();

        //}

    }
}
