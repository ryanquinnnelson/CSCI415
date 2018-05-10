//Ryan Nelson and Brian Engelbrecht

using System;

namespace NeuronModel
{
    public class EventRecord : IComparable<EventRecord>
    {
        //fields
        private int membranePotential;
        private double time;


        //constructors
        public EventRecord(TimeSpan t, int membranePotential) //tested
        {
            //convert timespan to numeric format
            String s = t.Seconds + "." + t.Milliseconds;
            time = Double.Parse(s);

            this.membranePotential = membranePotential;
        }


        //properties
        public int MembranePotential //tested
        {
            get
            {
                return this.membranePotential;
            }
            private set
            {
                membranePotential = value;
            }
        }

        public double Time //tested
        {
            get
            {
                return this.time;
            }
            private set
            {
                time = value;
            }
        }


        //public methods
        public int CompareTo(EventRecord other) //tested
        {
            if (this.time < other.time)
            {
                return -1;
            }
            else if (this.time > other.time)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override string ToString() //tested
        {
            return time + "\t" + membranePotential;
        }


        ////tests
        //public static void Main()
        //{
        //    Console.WriteLine("Test of Constructor 1");
        //    EventRecord e = new EventRecord(new TimeSpan(0, 0, 2), -1000);
        //    Console.WriteLine(e);
        //    Console.WriteLine("Test of GetTime()");
        //    Console.WriteLine(e.Time);
        //    Console.WriteLine("Test of GetMembranePotential()");
        //    Console.WriteLine(e.MembranePotential);
        //    Console.WriteLine("Test of CompareTo()");
        //    EventRecord later = new EventRecord(new TimeSpan(0, 0, 5), -1000);
        //    Console.WriteLine(e.CompareTo(later));
        //    Console.WriteLine(later.CompareTo(e));
        //    EventRecord equal = new EventRecord(new TimeSpan(0, 0, 2), -1000);
        //    Console.WriteLine(e.CompareTo(equal));
        //}
    }
}
