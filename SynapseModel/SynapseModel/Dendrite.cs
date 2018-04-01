using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections;

namespace SynapseModel
{
    /*
         * Task 1 - decay polarization of membrane potential
         * 
         * Task 2 - consume input buffer, alter membrane potential
         *
         * Task 3 - send current membrane potential to cell body buffer
    */
    public class Dendrite
    {
        public const int RESTING_POTENTIAL = -70000; //Volts
        public const int RESTORE_INCREMENT = 1;

        private DendriteGrowthState state;
        public int Id { get; private set; }
        private DendriteType type;
        private int membranePotential; //Volts
        private BlockingCollection<Neurotransmitter> buffer; //shared


        public Dendrite(int id, DendriteType type)
        {
            state = DendriteGrowthState.NoGrowth;
            this.type = type;
            this.Id = id;
            this.membranePotential = RESTING_POTENTIAL; //Volts
            buffer = new BlockingCollection<Neurotransmitter>(new ConcurrentQueue<Neurotransmitter>());
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

        public void AddToBuffer(Neurotransmitter nt)
        {
            buffer.Add(nt);
        }

        public int TryRemoveFromBuffer()
        {
            Neurotransmitter removed = buffer.Take();
            Interlocked.Add(ref membranePotential, removed.ElectricalPotential);
            //Console.WriteLine("Dendrite {0} Membrane Potential is " + membranePotential, Id);
            return removed.ElectricalPotential;
        }

        public void DecayMembranePotential(){
            if (membranePotential < RESTING_POTENTIAL)
            {
                Interlocked.Add(ref membranePotential, RESTORE_INCREMENT);
            }
            else
            {
                Interlocked.Add(ref membranePotential, -RESTORE_INCREMENT);
            }
        }


    }//end class
}
