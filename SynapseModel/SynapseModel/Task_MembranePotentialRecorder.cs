using System;
using System.Threading;
using System.Collections.Generic;
namespace SynapseModel
{
    //output membrane potential of cell body
    public class Recorder
    {
        private DateTime program_start;
        private List<Record> results = new List<Record>();

        public Recorder(DateTime start)
        {
            this.program_start = start;
        }

        public void Record(CellBody cb, TimeSpan workDay){
            DateTime start = DateTime.Now;
            DateTime current;
            TimeSpan ts;

            while (DateTime.Now - start < workDay)
            {
                int sleep = (cb.State == 0 ? 5 : 1); //finer grained recording during an action potential
                Thread.Sleep(sleep);
                current = DateTime.Now; 
                ts = current.Subtract(program_start);
                results.Add(new Record(ts, cb.MembranePotential));
            }//end while

            //output results
            OutputResults();
        }

        private void OutputResults(){
            //sort queue
            //results.Sort();

            //output results
            foreach(Record r in results){
                Console.WriteLine(r);
            }
        }


    }
}
