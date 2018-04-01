using System;

namespace SynapseModel
{
    public class Neurotransmitter
    {
        private const int CHARGE = 100;
        public int ElectricalPotential { get; private set; }

        public Neurotransmitter(bool excitatory)
        {
            ElectricalPotential = excitatory ? CHARGE : -CHARGE;
        }

        public override string ToString(){
            return "NT: " + ElectricalPotential;
        }
    }
}
