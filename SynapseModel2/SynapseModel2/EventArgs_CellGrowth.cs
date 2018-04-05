using System;
namespace SynapseModel2
{
    public class EventArgs_CellGrowth : EventArgs
    {
        //fields
        private Neuron neuron;
        private DateTime when;


        //constructors
        public EventArgs_CellGrowth(DateTime when, Neuron neuron) //tested
        {
            this.when = when;
            this.neuron = neuron;
        }


        //properties
        public Neuron Neuron //tested
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

        public DateTime When //tested
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

        //public methods
        public override string ToString() //tested
        {
            return "EventArgs_CellGrowth{ neuron=" + neuron + ", when=" + when + " }";
        }


        ////tests
        //public static void Main(){
        //    Console.WriteLine("Test of Constructor 1");
        //    EventArgs_CellGrowth e = new EventArgs_CellGrowth(DateTime.Now, new Neuron());
        //    Console.WriteLine(e);

        //    Console.WriteLine("Test of GetNeuron()");
        //    Console.WriteLine(e.Neuron);

        //    Console.WriteLine("Test of GetWhen()");
        //    Console.WriteLine(e.When);
        //}
    }
}
