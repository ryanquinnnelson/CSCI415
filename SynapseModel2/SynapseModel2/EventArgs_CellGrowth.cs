using System;
namespace SynapseModel2
{
    public class EventArgs_CellGrowth : EventArgs
    {
        //fields
        private Neuron neuron;
        private DateTime when;


        //constructors
        public EventArgs_CellGrowth(DateTime when, Neuron neuron)
        {
            this.when = when;
            this.neuron = neuron;
        }


        //properties
        public Neuron Neuron
        {
            get
            {
                return this.neuron;
            }
            set
            {
                neuron = value;
            }
        }

        public DateTime When
        {
            get
            {
                return this.when;
            }
            set
            {
                when = value;
            }
        }

    }
}
