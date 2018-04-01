using System;
namespace SynapseModel
{
    public class Consumer_CellBody
    {
        public int Id { get; private set; }

        public Consumer_CellBody(int id)
        {
            //Console.WriteLine("Consumer_CellBody " + id + " is created.");
            this.Id = id;
        }

        public void Work(CellBody cb, TimeSpan runLength)
        {
            DateTime start = DateTime.Now;
            //Console.WriteLine("Consumer_CellBody {0} is working...", Id);
            while (DateTime.Now - start < runLength)
            {
                int voltage = cb.TryRemoveFromBuffer();
                Console.WriteLine("Consumer_CellBody {0} removed {1} from buffer.", Id, voltage);
            }//end while

            //Console.WriteLine("Consumer_CellBody {0} is done.", Id);
        }//end Work()

        public override String ToString()
        {
            return "Consumer_CellBody " + Id;
        }
    }
}
