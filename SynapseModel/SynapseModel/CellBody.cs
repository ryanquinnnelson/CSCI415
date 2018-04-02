using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace SynapseModel
{
    public class CellBody
    {
        public const int RESTING_POTENTIAL = -70000; //Volts
        public const int RESTORE_INCREMENT = 50;
        private BlockingCollection<int> buffer; //shared
        private int membranePotential; //Volts
        private int state; //0 is resting state, 1 is action potential
        private DateTime start;
        public List<Record> results = new List<Record>();




        public CellBody(DateTime start)
        {
            buffer = new BlockingCollection<int>(new ConcurrentQueue<int>());
            state = 0;
            membranePotential = RESTING_POTENTIAL;
            this.start = start;
        }

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


        public void AddToBuffer(int voltage)
        {
            buffer.Add(voltage);
        }

        public int TryRemoveFromBuffer()
        {
            int voltage = buffer.Take();
            if (state == 0)
            {
                int current = Interlocked.Add(ref membranePotential, voltage);


                DateTime now = DateTime.Now;
                TimeSpan ts = now.Subtract(start);
                Output(ts, current);


                CheckForActionPotential();
                //Console.WriteLine("CellBody Membrane Potential is \t\t\t\t\t{0}", membranePotential);

            }

            return voltage;
        }

        public void DecayMembranePotential()
        {
            if (state == 0 && membranePotential < RESTING_POTENTIAL)
            {
                int current = Interlocked.Add(ref membranePotential, RESTORE_INCREMENT);
                //Output(current);
            }
            else if (state == 0 && membranePotential > RESTING_POTENTIAL)
            {
                int current = Interlocked.Add(ref membranePotential, -RESTORE_INCREMENT);
                //Output(current);
            }
            else
            {
                //do nothing
            }
        }

        public void SetActionPotentialState()
        {
            int original = Interlocked.CompareExchange(ref state, 1, 0);
            if (original == 0)
            {
                //Thread.Sleep(1);
                TriggerActionPotentialTask();
            }
        }

        public void SetRestingPotentialState()
        {
            Interlocked.CompareExchange(ref state, 0, 1);
        }



        public void CheckForActionPotential()
        {
            //Console.WriteLine("CellBody Membrane Potential is " + membranePotential);
            if (membranePotential >= -50000)
            {
                SetActionPotentialState();
            }
        }

        public void TriggerActionPotentialTask()
        {
            //Console.WriteLine("Action Potential triggered.");
            int current;
            DateTime now;
            TimeSpan ts;


            current = Interlocked.Exchange(ref membranePotential, +30000);
            now = DateTime.Now;
            ts = now.Subtract(start);
            Output(ts, 30000);
            //Thread.Sleep(1);


            //while(membranePotential > -100000){
            //    current = Interlocked.Add(ref membranePotential, -RESTORE_INCREMENT);
            //    //Output(current);
            //}

            current = Interlocked.Exchange(ref membranePotential, -100000);
            now = DateTime.Now;
            ts = now.Subtract(start);
            Output(ts, -100000);
            Thread.Sleep(1);

            ////restore to resting potential
            //while (membranePotential < -70000)
            //{
            //    current = Interlocked.Increment(ref membranePotential);
            //    //Output(current);
            //}
            current = Interlocked.Exchange(ref membranePotential, RESTING_POTENTIAL);

            SetRestingPotentialState();
        }

        public void Output(TimeSpan ts, int potential)
        {
            DateTime now = DateTime.Now;
            results.Add(new Record(ts, potential));
        }

    } //end class
}
