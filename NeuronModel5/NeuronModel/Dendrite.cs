using System;
using System.Collections.Concurrent;
using System.Text;
using System.Collections.Generic;
using System.Threading;

namespace NeuronModel
{
    public class Dendrite
    {
        //constants
        public const int RESTING_POTENTIAL = -70000; //Volts


        //fields
        private BlockingCollection<Neurotransmitter> buffer;        //shared
        private int decayFrequency;
        private int id;
        private int membranePotential; //Volts                      //shared
        private SecondaryMessenger messenger;                       //shared
        private int numAvailableSynapses;                           //shared
        private int numSynapsesToAddInGrowthEvent;
        private int productionFrequency;
        private int restoreIncrement;
        private int significantVoltageChange;
        private int state; //0 no growth; 1 growth                  //shared
        private ConcurrentDictionary<int, InputAxon> synapses;      //shared
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
                        int numStartingSynapses,
                        int significantVoltageChange)
        {
            state = 0;
            membranePotential = RESTING_POTENTIAL;
            numAvailableSynapses = numStartingSynapses;
            buffer = new BlockingCollection<Neurotransmitter>(new ConcurrentQueue<Neurotransmitter>());
            synapses = new ConcurrentDictionary<int, InputAxon>();

            this.id = id;
            this.type = type;
            this.decayFrequency = decayFrequency;
            this.productionFrequency = productionFrequency;
            this.restoreIncrement = restoreIncrement;
            this.numSynapsesToAddInGrowthEvent = numSynapsesToAddInGrowthEvent;
            messenger = new SecondaryMessenger(DateTime.Now, secondaryMessengerFrequencyTrigger, secondaryMessengerWindow);
            this.significantVoltageChange = significantVoltageChange;
        }


        //properties
        public int DecayFrequency //tested
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

        public int NumSynapsesToAddInGrowthEvent //tested
        {
            get
            {
                return this.numSynapsesToAddInGrowthEvent;
            }
            private set
            {
                numSynapsesToAddInGrowthEvent = value;
            }
        }

        public int ProductionFrequency //tested
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
        }

        public void DecayMembranePotential() //tested
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

        public int GetMembranePotentialDifference() //tested
        {
            return (membranePotential - RESTING_POTENTIAL);
        }

        public bool TryConnect(InputAxon axon) //tested
        {
            if (numAvailableSynapses > 0)
            {
                //atomically try to form a connection
                //If key exists, update with original value to prevent changes
                InputAxon result = synapses.AddOrUpdate(axon.Id, axon, (key, oldValue) => oldValue);
                if (result.Equals(axon))
                {
                    Interlocked.Decrement(ref numAvailableSynapses);
                    return true;
                }
            }

            return false;
        }

        public int TryRemoveFromBuffer() //tested
        {
            Neurotransmitter removed = buffer.Take();
            int charge = removed.Charge;

            int oldMembranePotential = membranePotential;

            //affect local membrane potential
            int currentMembranePotential = Interlocked.Add(ref membranePotential, charge);


            //decide whether change was significant enough to add to SecondaryMessenger
            if (Math.Abs(currentMembranePotential - oldMembranePotential) > significantVoltageChange)
            {
                messenger.AddEvent(DateTime.Now);

                //check whether dendrite growth state threshold reached
                if (state == 0 && messenger.IsGrowthStateTriggered(DateTime.Now))
                {
                    SetGrowthState();
                }
            }

            return charge;
        }

        public override string ToString() //tested
        {
            return "Dendrite{\n\tid=" + id + ",\n\ttype=" + type + ",\n\tstate="
                + state + ",\n\tnumAvailableSynapses=" + numAvailableSynapses
                + ",\n\tmembranePotential=" + membranePotential + ",\n\tbuffer="
                + buffer + ",\n\tmessenger=" + messenger + ",\n\tsynapses=" 
                + SynapsesToString() + ",\n\trestoreIncrement=" 
                + restoreIncrement+ ",\n\tproductionFrequency=" 
                + productionFrequency + ",\n\tnumSynapsesToAddInGrowthEvent="
                + numSynapsesToAddInGrowthEvent + ",\n\tdecayFrequency=" 
                + decayFrequency+ "\n}";

        }


        //private helper methods
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

        private string SynapsesToString() //tested
        {
            StringBuilder sb = new StringBuilder("[");
            foreach (InputAxon axon in synapses.Values)
            {
                sb.Append(axon);
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
            Interlocked.Add(ref numAvailableSynapses, numSynapsesToAddInGrowthEvent);

            //restore no growth state
            messenger.reset();
            SetNoGrowthState();


            //event
            EventArgs_DendriteGrowth args = new EventArgs_DendriteGrowth(when, this.Id);
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
        //    Dendrite d = new Dendrite(1, 1, 100, 100, 50, 1, new TimeSpan(0, 0, 2), 100, 1);
        //    Console.WriteLine(d);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetDecayFrequency()");
        //    Console.WriteLine(d.DecayFrequency);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetProductionFrequency()");
        //    Console.WriteLine(d.ProductionFrequency);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetNumSynapsesToAddInGrowthEvent()");
        //    Console.WriteLine(d.NumSynapsesToAddInGrowthEvent);
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

        //    Console.WriteLine("Test of DecayMembranePotential() too negative");
        //    d.MembranePotential = -90000;
        //    d.DecayMembranePotential();
        //    Console.WriteLine(d.MembranePotential);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of DecayMembranePotential() too positive");
        //    d.MembranePotential = -40000;
        //    d.DecayMembranePotential();
        //    Console.WriteLine(d.MembranePotential);
        //    Console.WriteLine();
        //    d.MembranePotential = -70000;

        //    d.messenger = new SecondaryMessenger(DateTime.Now, 1, new TimeSpan(0, 0, 5));
        //    d.AddToBuffer(new Neurotransmitter(-10));

        //    Console.WriteLine("Test of FormSynapseConnection() success");
        //    Console.WriteLine(d.TryConnect(new InputAxon(0, 100, 1)));
        //    Console.WriteLine(d.SynapsesToString());
        //    Console.WriteLine();

        //    Console.WriteLine("Test of FormSynapseConnection() failure");
        //    Console.WriteLine(d.TryConnect(new InputAxon(1, 100, 1)));
        //    Console.WriteLine(d.SynapsesToString());
        //    Console.WriteLine();

        //    Console.WriteLine("Test of RaiseDendriteGrowthEvent()");
        //    d.RaiseDendriteGrowthEvent(DateTime.Now);
        //    Console.WriteLine(d.SynapsesToString());
        //    Console.WriteLine();
        //}

    }
}
