using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace SynapseModel3
{
    public class CellBody
    {
        //constants
        private const int RESTING_POTENTIAL = -70000; //Volts
        private const int THRESHOLD_POTENTIAL = -50000; //Volts
        private const int RESTORE_INCREMENT = 50;


        //fields
        private BlockingCollection<int> buffer; //shared
        private int membranePotential; //Volts
        private ConcurrentBag<EventRecord> outputs;
        private DateTime start;
        private int state; //0 is resting state, 1 is action potential
        private int decayFrequency;

        //constructors
        public CellBody(DateTime start, int decayFrequency) //tested
        {
            buffer = new BlockingCollection<int>(new ConcurrentQueue<int>());
            membranePotential = RESTING_POTENTIAL;
            outputs = new ConcurrentBag<EventRecord>();
            this.start = start;
            state = 0;
            this.decayFrequency = decayFrequency;
        }


        //properties
        public int MembranePotential //tested
        {
            get
            {
                return membranePotential;
            }
            private set
            {
                membranePotential = value;
            }
        }

        public int State //tested
        {
            get
            {
                return state;
            }
            private set
            {
                state = value;
            }
        }

        public int DecayFrequency{
            get{
                return this.decayFrequency;
            }
            private set{
                decayFrequency = value;
            }
        }


        //public methods
        public void AddToBuffer(int voltage) //tested
        {
            buffer.Add(voltage);
        }

        public void DecayMembranePotential() //tested
        {
            if (state == 0 && membranePotential < RESTING_POTENTIAL)
            {
                int current = Interlocked.Add(ref membranePotential, RESTORE_INCREMENT);
            }
            else if (state == 0 && membranePotential > RESTING_POTENTIAL)
            {
                int current = Interlocked.Add(ref membranePotential, -RESTORE_INCREMENT);
            }
            else
            {
                //do nothing
            }
        }

        public List<EventRecord> GetOutputsAsList() //tested
        {
            List<EventRecord> list = new List<EventRecord>(outputs.Count);

            while (!outputs.IsEmpty)
            {
                EventRecord current = null;
                outputs.TryTake(out current);

                if (current != null)
                {
                    list.Add(current);
                }
            }
            list.Sort(); //??untested line
            return list;
        }

        public int TryRemoveFromBuffer() //tested
        {
            //Console.WriteLine("TryRemoveFromBuffer()");

            int voltage = buffer.Take();
            if (state == 0)
            {
                //affect membrane potential
                int current = Interlocked.Add(ref membranePotential, voltage);

                //save result as output
                StoreAsOutput(current);

                //check whether threshold potential is reached
                if (IsThresholdPotentialReached())
                {
                    SetActionPotentialState();
                }
            }
            return voltage;
        }

        public override string ToString() //tested
        {
            return "CellBody{ start=" + start + ", state=" + state
                + ", membranePotential=" + membranePotential + ", buffer="
                + buffer + ", outputs=" + OutputsListToString() + " }";
        }


        //private helper methods
        private bool IsThresholdPotentialReached() //tested
        {
            return membranePotential >= THRESHOLD_POTENTIAL;
        }

        private void PerformMembranePotentialSpike() //tested
        {
            //Console.WriteLine("PerformMembranePotentialSpike()");
            int current;
            current = Interlocked.Exchange(ref membranePotential, +30000);
            StoreAsOutput(30000);

            current = Interlocked.Exchange(ref membranePotential, -100000);
            StoreAsOutput(-100000);
            Thread.Sleep(1); //absolute refractory period
        }

        private void SetActionPotentialState() //tested
        {
            //Console.WriteLine("SetActionPotentialState()");
            int original = Interlocked.CompareExchange(ref state, 1, 0);
            if (original == 0)
            {
                RaiseActionPotentialEvent(DateTime.Now); //raise event to alert neuron
            }
        }

        private void SetRestingPotentialState() //tested
        {
            Interlocked.CompareExchange(ref state, 0, 1);
        }

        private void StoreAsOutput(int potential) //tested
        {
            DateTime now = DateTime.Now;
            TimeSpan ts = now.Subtract(start);
            outputs.Add(new EventRecord(ts, potential));
        }

        private string OutputsListToString() //tested
        {
            List<EventRecord> outputList = GetOutputsAsList();
            StringBuilder sb = new StringBuilder("[");
            foreach (EventRecord e in outputList)
            {
                sb.Append(e);
                sb.Append(",");
            }
            if (outputList.Count > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("]");
            return sb.ToString();
        }


        //Action Potential event code
        private void RaiseActionPotentialEvent(DateTime when) //tested
        {
            //event
            Console.WriteLine("Cell Body raises action potential event.");
            EventArgs_ActionPotential args = new EventArgs_ActionPotential(when);
            OnActionPotentialEvent(args);

            //action within cell body
            PerformMembranePotentialSpike();

            //restore to resting potential
            Interlocked.Exchange(ref membranePotential, RESTING_POTENTIAL);
            SetRestingPotentialState();
        }

        protected virtual void OnActionPotentialEvent(EventArgs_ActionPotential e)
        {
            EventHandler<EventArgs_ActionPotential> handler = ActionPotentialEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<EventArgs_ActionPotential> ActionPotentialEvent;


        ////tests
        //public static void Main()
        //{
        //    Console.WriteLine("Test of Constructor 1");
        //    CellBody c = new CellBody(DateTime.Now);
        //    Console.WriteLine(c);
        //    Console.WriteLine("Buffer count: " + c.buffer.Count);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetMembranePotential()");
        //    Console.WriteLine(c.MembranePotential);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetState()");
        //    Console.WriteLine(c.State);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of AddToBuffer()");
        //    c.AddToBuffer(100);
        //    c.AddToBuffer(20);
        //    Console.WriteLine("Buffer count: " + c.buffer.Count);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of TryRemoveFromBuffer() with state == 1");
        //    c.State = 1;
        //    Console.WriteLine(c.TryRemoveFromBuffer());
        //    Console.WriteLine("Membrane Potential: " + c.MembranePotential);
        //    Console.WriteLine("Count of outputs: " + c.outputs.Count);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of TryRemoveFromBuffer() with state == 0, threshold potential not reached");
        //    c.State = 0;
        //    Console.WriteLine(c.TryRemoveFromBuffer());
        //    Console.WriteLine("Membrane Potential: " + c.MembranePotential);
        //    Console.WriteLine("Count of outputs: " + c.outputs.Count);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of IsThresholdPotentialReached() with membrane potential = -49000");
        //    c.MembranePotential = -49000;
        //    Console.WriteLine(c.IsThresholdPotentialReached());
        //    Console.WriteLine();

        //    Console.WriteLine("Test of TryRemoveFromBuffer() with state == 0, threshold potential reached");
        //    c.AddToBuffer(20);
        //    c.TryRemoveFromBuffer();
        //    Console.WriteLine(c);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of DecayMembranePotential() too negative");
        //    c.MembranePotential = -90000;
        //    Console.WriteLine(c.MembranePotential);
        //    c.DecayMembranePotential();
        //    Console.WriteLine(c.MembranePotential);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of DecayMembranePotential() too positive");
        //    c.MembranePotential = -60000;
        //    Console.WriteLine(c.MembranePotential);
        //    c.DecayMembranePotential();
        //    Console.WriteLine(c.MembranePotential);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of DecayMembranePotential() at rest");
        //    c.MembranePotential = -70000;
        //    Console.WriteLine(c.MembranePotential);
        //    c.DecayMembranePotential();
        //    Console.WriteLine(c.MembranePotential);
        //    Console.WriteLine();
        //}
    }
}
