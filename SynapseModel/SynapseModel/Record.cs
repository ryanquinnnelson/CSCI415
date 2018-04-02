using System;
namespace SynapseModel
{
    public class Record
    {
        private double time;
        private int membranePotential;

        public Record(TimeSpan t, int m)
        {
            String s = t.Seconds + "." + t.Milliseconds;
            time = Double.Parse(s);
            membranePotential = m;
        }

        public override String ToString(){
            return time + "\t" + membranePotential;
        }
    }
}
