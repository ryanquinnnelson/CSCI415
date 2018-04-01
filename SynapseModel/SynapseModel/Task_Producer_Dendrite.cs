using System;
using System.Threading;
namespace SynapseModel
{
    public class Producer_Dendrite
    {
        public const int RESTING_POTENTIAL = -70000; //Volts
        public int Id { get; private set; }
        public int Frequency { get; private set; }

        public Producer_Dendrite(int id, int frequency)
        {
            //Console.WriteLine("Producer_Dendrite " + id + " is created.");
            this.Id = id;
            this.Frequency = frequency;
        }

        public void Work(Dendrite dt, CellBody cb, TimeSpan runLength)
        {
            DateTime start = DateTime.Now;
            //Console.WriteLine("Producer_Dendrite {0} is working...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(Frequency);
                int currentMembranePotential = dt.MembranePotential;
                int difference = (currentMembranePotential - RESTING_POTENTIAL);
                cb.AddToBuffer(difference);

                Console.WriteLine("Producer_Dendrite {0} added {1} to cell body buffer.", Id, currentMembranePotential);
            }//end while

            //Console.WriteLine("Producer_Dendrite {0} is done.", Id);
        }//end Work()


        public override String ToString()
        {
            return "Producer_Dendrite " + Id + " with frequency " + Frequency;
        }
    }
}
