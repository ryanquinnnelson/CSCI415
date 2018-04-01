using System;
using System.Threading;
namespace SynapseModel
{
    //output membrane potential of cell body
    public class Recorder
    {
        private DateTime program_start;

        public Recorder(DateTime start)
        {
            this.program_start = start;
        }

        public void Work(CellBody cb, TimeSpan workDay){
            DateTime start = DateTime.Now;
            DateTime current;
            TimeSpan ts;
            int cellBodyMembranePotential;

            while (DateTime.Now - start < workDay)
            {
                Thread.Sleep(10);
                current = DateTime.Now; 
                ts = current.Subtract(program_start);
                cellBodyMembranePotential = cb.MembranePotential;
                Console.WriteLine(ts.Seconds + "." + ts.Milliseconds + "\t" + cellBodyMembranePotential);
            }//end while
        }


    }
}
