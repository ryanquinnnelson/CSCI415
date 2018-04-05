using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SynapseModel2
{
    public class EventArgs_DendriteGrowth : EventArgs
    {
        //fields
        private DateTime when;
        private Dendrite dendrite;


        //constructor
        public EventArgs_DendriteGrowth(DateTime when, Dendrite dendrite) //tested
        {
            this.when = when;
            this.dendrite = dendrite;
        }


        //properties
        public Dendrite Dendrite //tested
        {
            get
            {
                return this.dendrite;
            }
            set
            {
                dendrite = value;
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
            return "EventArgs_DendriteGrowth{ dendrite=" + dendrite + ", when=" + when + " }";
		}


		////tests
		//public static void Main(){
        //    Console.WriteLine("Test of Constructor 1");
        //    EventArgs_DendriteGrowth e = new EventArgs_DendriteGrowth(DateTime.Now, new Dendrite(1, 1, 100));
        //    Console.WriteLine(e);

        //    Console.WriteLine("Test of GetDendrite()");
        //    Console.WriteLine(e.Dendrite);

        //    Console.WriteLine("Test of GetWhen()");
        //    Console.WriteLine(e.When);
        //}

    }
}
