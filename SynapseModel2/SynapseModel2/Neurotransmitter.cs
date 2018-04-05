using System;
namespace SynapseModel2
{
    public class Neurotransmitter
    {
        //fields
        int charge;


        //constructors
        public Neurotransmitter(int charge)
        {
            this.charge = charge;
        }


        //properties
        public int Charge
        {
            get
            {
                return this.charge;
            }
            private set
            {
                charge = value;
            }
        }
    }
}
