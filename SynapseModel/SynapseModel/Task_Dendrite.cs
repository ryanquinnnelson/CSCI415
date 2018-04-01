using System;
using System.Threading;

namespace SynapseModel
{
    public class Task_Dendrite
    {
        public const int RESTING_POTENTIAL = -70000; //Volts
        public int Id { get; private set; }
        public int Frequency { get; private set; }

        public Task_Dendrite(int id, int frequency)
        {
            //Console.WriteLine("Task_Dendrite " + id + " is created.");
            this.Id = id;
            this.Frequency = frequency;
        }

        public void Consume(Dendrite dt, TimeSpan runLength)
        {
            DateTime start = DateTime.Now;
            //Console.WriteLine("Task_Dendrite {0} is working...", Id);
            while (DateTime.Now - start < runLength)
            {
                int voltage = dt.TryRemoveFromBuffer();
                Console.WriteLine("Task_Dendrite {0} consumed {1} from buffer.", Id, voltage);
            }//end while

            //Console.WriteLine("Task_Dendrite {0} is done.", Id);
        }

        public void Produce(Dendrite dt, CellBody cb, TimeSpan runLength)
        {
            DateTime start = DateTime.Now;
            //Console.WriteLine("Task_Dendrite {0} is working...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(Frequency);
                int currentMembranePotential = dt.MembranePotential;
                int difference = (currentMembranePotential - RESTING_POTENTIAL);
                cb.AddToBuffer(difference);

                Console.WriteLine("Task_Dendrite {0} produced {1} to cell body buffer.", Id, currentMembranePotential);
            }//end while

            //Console.WriteLine("Task_Dendrite {0} is done.", Id);
        }


        public void Decay(Dendrite dt, TimeSpan runLength)
        {
            DateTime start = DateTime.Now;
            //Console.WriteLine("Task_Dendrite {0} is working...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(Frequency);
                dt.DecayMembranePotential();
                Console.WriteLine("Task_Dendrite {0} decayed dendrite membrane potential to {1}.", Id, dt.MembranePotential);
            }
            //Console.WriteLine("Task_Dendrite {0} is done.", Id);
        }

        public override String ToString()
        {
            return "Task_Dendrite " + Id;
        }
    }
}
