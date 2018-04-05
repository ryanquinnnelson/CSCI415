using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace SynapseModel2
{
    public class CellBody
    {
        //constants
        private const int RESTING_POTENTIAL = -70000; //Volts
        private const int RESTORE_INCREMENT = 50;


        //fields
        private BlockingCollection<int> buffer; //shared
        private int membranePotential; //Volts
        private ConcurrentBag<EventRecord> outputs;
        private DateTime start;
        private int state; //0 is resting state, 1 is action potential


        //constructors
        public CellBody(DateTime start)
        {
            buffer = new BlockingCollection<int>(new ConcurrentQueue<int>());
            membranePotential = RESTING_POTENTIAL;
            outputs = new ConcurrentBag<EventRecord>();
            this.start = start;
            state = 0;
        }


        //properties
        public int MembranePotential
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

        public int State
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


        //public methods
        public void AddToBuffer(int voltage)
        {
            buffer.Add(voltage);
        }

        public void DecayMembranePotential()
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

        public int TryRemoveFromBuffer()
        {
            int voltage = buffer.Take();
            if (state == 0)
            {
                //affect membrane potential
                int current = Interlocked.Add(ref membranePotential, voltage);

                //save result as output
                StoreAsOutput(current);

                //check whether threshold potential is reached
                if(IsThresholdPotentialReached()){
                    SetActionPotentialState();
                }
                //Console.WriteLine("CellBody Membrane Potential is \t\t\t\t\t{0}", membranePotential);

            }

            return voltage;
        }


        //private helper methods
        private bool IsThresholdPotentialReached()
        {
            return membranePotential >= -50000;
        }

        private void PerformMembranePotentialSpike()
        {
            //Console.WriteLine("Action Potential triggered.");
            int current;
            current = Interlocked.Exchange(ref membranePotential, +30000);
            StoreAsOutput(30000);

            current = Interlocked.Exchange(ref membranePotential, -100000);
            StoreAsOutput(-100000);
            Thread.Sleep(1); //absolute refractory period

            //restore to resting potential
            current = Interlocked.Exchange(ref membranePotential, RESTING_POTENTIAL);
            SetRestingPotentialState();
        }

        private void SetActionPotentialState()
        {
            int original = Interlocked.CompareExchange(ref state, 1, 0);
            if (original == 0)
            {
                RaiseActionPotentialEvent(DateTime.Now); //raise event to alert neuron
                PerformMembranePotentialSpike();
            }
        }

        private void SetRestingPotentialState()
        {
            Interlocked.CompareExchange(ref state, 0, 1);
        }

        private void StoreAsOutput(int potential)
        {
            DateTime now = DateTime.Now;
            TimeSpan ts = now.Subtract(start);
            outputs.Add(new EventRecord(ts, potential));
        }


        //Action Potential event code
        private void RaiseActionPotentialEvent(DateTime when)
        {
            Console.WriteLine("Action Potential event raised.");

            EventArgs_ActionPotential args = new EventArgs_ActionPotential(when);

            OnActionPotentialEvent(args);
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
    }
}
