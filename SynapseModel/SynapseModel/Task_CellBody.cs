using System;
using System.Threading;

namespace SynapseModel
{
    public class Task_CellBody
    {
        public int Id { get; private set; }
        public int Frequency { get; private set; }

        public Task_CellBody(int id, int frequency)
        {
            //Console.WriteLine("Task_CellBody " + id + " is created.");
            this.Id = id;
            this.Frequency = frequency;
        }

        public void Consume(CellBody cb, TimeSpan runLength)
        {
            DateTime start = DateTime.Now;
            //Console.WriteLine("Task_CellBody {0} is working...", Id);
            while (DateTime.Now - start < runLength)
            {
                int voltage = cb.TryRemoveFromBuffer();
                //Console.WriteLine("Task_CellBody {0} removed {1} from buffer.", Id, voltage);
            }//end while

            //Console.WriteLine("Task_CellBody {0} is done.", Id);
        }


        public void Decay(CellBody cb, TimeSpan runLength)
        {
            DateTime start = DateTime.Now;
            //Console.WriteLine("Task_CellBody {0} is working...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(Frequency);
                cb.DecayMembranePotential();
                //Console.WriteLine("Task_CellBody {0} decayed cell body membrane potential to {1}.", Id, cb.MembranePotential);
            }
            //Console.WriteLine("Task_CellBody {0} is done.", Id);
        }

        public override String ToString()
        {
            return "Task_CellBody " + Id;
        }
    }
}
