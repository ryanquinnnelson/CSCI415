using System;
using System.Threading;
namespace SynapseModel
{
    public class Producer_Axon
    {
        public int Id { get; private set; }
        public int Frequency { get; private set; }

        public Producer_Axon(int id, int frequency)
        {
            //Console.WriteLine("Producer_Axon " + id + " is created.");
            this.Id = id;
            this.Frequency = frequency; //make random later
        }

        public void Work(Dendrite dt, TimeSpan runLength)
        {
            DateTime start = DateTime.Now;
            //Console.WriteLine("Producer_Axon {0} is working...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(Frequency);
                Neurotransmitter newest = new Neurotransmitter(true);
                dt.AddToBuffer(newest);

                Console.WriteLine("Producer_Axon {0} added {1} to dendrite {2} buffer.", Id, newest.ElectricalPotential, dt.Id);
            }//end while

            //Console.WriteLine("Producer_Axon {0} is done.", Id);
        }//end Work()

        public override String ToString()
        {
            return "Producer_Axon " + Id + " with frequency " + Frequency;
        }
    }
}
