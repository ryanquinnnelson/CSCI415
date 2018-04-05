using System;
namespace SynapseModel3
{
    public class EventArgs_ActionPotential : EventArgs
    {
        //fields
        private DateTime when;


        //constructors
        public EventArgs_ActionPotential(DateTime when) //tested
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
            return "EventArgs_ActionPotential{ when=" + when + " }";
        }


        ////tests
        //public static void Main()
        //{
        //    Console.WriteLine("Test of Constructor 1");
        //    EventArgs_ActionPotential e = new EventArgs_ActionPotential(DateTime.Now);
        //    Console.WriteLine(e);

        //    Console.WriteLine("Test of GetWhen()");
        //    Console.WriteLine(e.When);
        //}

    }
}
