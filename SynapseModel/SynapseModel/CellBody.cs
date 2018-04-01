using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SynapseModel
{
    public class CellBody
    {
        public const int RESTING_POTENTIAL = -70000; //Volts
        public const int RESTORE_INCREMENT = 1;
        private BlockingCollection<int> buffer; //shared
        private int membranePotential; //Volts
        private int state; //0 is resting state, 1 is action potential


        public CellBody()
        {
            buffer = new BlockingCollection<int>(new ConcurrentQueue<int>());
            state = 0;
            membranePotential = RESTING_POTENTIAL;
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


        public void AddToBuffer(int voltage)
        {
            buffer.Add(voltage);
        }

        public int TryRemoveFromBuffer()
        {
            int voltage = buffer.Take();
            if (state == 0)
            {
                Interlocked.Add(ref membranePotential, voltage);
            }
            return voltage;
        }

        public void DecayMembranePotential()
        {
            if (membranePotential < RESTING_POTENTIAL)
            {
                Interlocked.Add(ref membranePotential, RESTORE_INCREMENT);
            }
            else
            {
                Interlocked.Add(ref membranePotential, -RESTORE_INCREMENT);
            }
        }

        public void SetActionPotential()
        {
            Interlocked.CompareExchange(ref state, 1, 0);
        }

        public void SetRestingPotential()
        {
            Interlocked.CompareExchange(ref state, 0, 1);
        }



        //public void CheckActionPotential(){
        //    //Console.WriteLine("CellBody Membrane Potential is " + membranePotential);
        //    if (membranePotential >= -50000 && cellState != CellState.ActionPotential)
        //    {
        //        TriggerActionPotential();
        //    }
        //}

        //public void TriggerActionPotential(){
        //    //Console.WriteLine("Action Potential triggered...");
        //    //prevent another action potential from being triggered until finished with absolute refractory period
        //    cellState = CellState.ActionPotential;

        //    //spike to +30 mV
        //    membranePotential = +30000;
        //    Thread.Sleep(2);
        //    //Console.WriteLine("CellBody Membrane Potential is " + membranePotential);

        //    //decrease to -100 mV
        //    membranePotential = -100000;
        //    Thread.Sleep(2);
        //    //Console.WriteLine("CellBody Membrane Potential is " + membranePotential);

        //    //absolute refractory period
        //    Thread.Sleep(1); 

        //    //stabilize to resting potential
        //    while(membranePotential <= -70000){
        //        Interlocked.Increment(ref membranePotential);
        //    }
        //    //Console.WriteLine("Resting Potential reset.");
        //    cellState = CellState.RestingPotential;
        //}
    } //end class
}
