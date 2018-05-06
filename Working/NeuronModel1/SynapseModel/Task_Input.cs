using System;
using System.Threading;

namespace SynapseModel
{
    public class Task_Input
    {
        public int Id { get; private set; }
        public int Frequency { get; private set; }

        public Task_Input(int id, int frequency)
        {
            //Console.WriteLine("Task_Input " + id + " is created.");
            this.Id = id;
            this.Frequency = frequency; //make random later
        }

        public void Produce(Dendrite dt, TimeSpan runLength)
        {
            DateTime start = DateTime.Now;
            //Console.WriteLine("Task_Input {0} is working...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(Frequency);
                Neurotransmitter newest = new Neurotransmitter(true);
                dt.AddToBuffer(newest);

                //Console.WriteLine("Task_Input {0} added {1} to dendrite {2} buffer.", Id, newest.ElectricalPotential, dt.Id);

            }//end while

            //Console.WriteLine("Task_Input {0} is done.", Id);
        }//end Work()

        public override String ToString()
        {
            return "Task_Input " + Id + " with frequency " + Frequency;
        }
    }
}
