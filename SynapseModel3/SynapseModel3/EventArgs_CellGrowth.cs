using System;
using System.Collections.Generic;

namespace SynapseModel3
{
    public class EventArgs_CellGrowth : EventArgs
    {
        //fields
        private DateTime when;
        private List<Dendrite> dendritesAdded;


        //constructors
        public EventArgs_CellGrowth(DateTime when, List<Dendrite> added) //tested
        {
            this.when = when;
            this.dendritesAdded = added;
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

        public List<Dendrite> DendritesAdded //tested
        {
            get
            {
                return this.dendritesAdded;
            }
            set
            {
                dendritesAdded = value;
            }
        }

        //public methods
        public override string ToString() //tested
        {
            return "EventArgs_CellGrowth{ when=" + when + " }";
        }


        ////tests
        //public static void Main(){
        //    Console.WriteLine("Test of Constructor 1");
        //    EventArgs_CellGrowth e = new EventArgs_CellGrowth(DateTime.Now);
        //    Console.WriteLine(e);

        //    Console.WriteLine("Test of GetWhen()");
        //    Console.WriteLine(e.When);
        //}
    }
}
