using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SynapseModel3
{
    public class EventArgs_DendriteGrowth : EventArgs
    {
        //fields
        private DateTime when;


        //constructor
        public EventArgs_DendriteGrowth(DateTime when) //tested
        {
            this.when = when;
        }


        //properties
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
            return "EventArgs_DendriteGrowth{ when=" + when + " }";
		}


		////tests
		//public static void Main(){
        //    Console.WriteLine("Test of Constructor 1");
        //    EventArgs_DendriteGrowth e = new EventArgs_DendriteGrowth(DateTime.Now);
        //    Console.WriteLine(e);

        //    Console.WriteLine("Test of GetWhen()");
        //    Console.WriteLine(e.When);
        //}

    }
}
