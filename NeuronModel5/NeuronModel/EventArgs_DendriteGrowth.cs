using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuronModel
{
    public class EventArgs_DendriteGrowth : EventArgs
    {
        //fields
        private int dendriteId;
        private DateTime when;


        //constructor
        public EventArgs_DendriteGrowth(DateTime when, int dendriteId) //tested
        {
            this.when = when;
            this.dendriteId = dendriteId;
        }


        //properties
        public int DendriteId //tested
        {
            get
            {
                return this.dendriteId;
            }
            set
            {
                dendriteId = value;
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
            return "EventArgs_DendriteGrowth{ dendriteId=" + dendriteId + ", when=" + when + " }";
        }


        ////tests
        //public static void Main(){
        //    Console.WriteLine("Test of Constructor 1");
        //    EventArgs_DendriteGrowth e = new EventArgs_DendriteGrowth(DateTime.Now, 0);
        //    Console.WriteLine(e);

        //    Console.WriteLine("Test of GetWhen()");
        //    Console.WriteLine(e.When);

        //    Console.WriteLine("Test of GetDendriteId()");
        //    Console.WriteLine(e.DendriteId);
        //}

    }
}
