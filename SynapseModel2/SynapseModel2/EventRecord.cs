using System;
namespace SynapseModel2
{
    public class EventRecord : IComparable<EventRecord>
    {
        //fields
        double time;
        int membranePotential;


        //constructors
        public EventRecord(TimeSpan t, int membranePotential)
        {
            //convert timespan to numeric format
            String s = t.Seconds + "." + t.Milliseconds;
            time = Double.Parse(s);

            this.membranePotential = membranePotential;
        }


        //properties
        public double Time
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

        public int MembranePotential
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


        //public methods
        public int CompareTo(EventRecord other)
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

        public override string ToString()
        {
            return time + "\t" + membranePotential;
        }
    }
}
