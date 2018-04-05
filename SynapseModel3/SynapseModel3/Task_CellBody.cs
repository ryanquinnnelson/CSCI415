using System;
using System.Threading;

namespace SynapseModel3
{
    public class Task_CellBody
    {
        //fields
        private int id;
        private CellBody body;
        private TimeSpan runLength;


        //constructors
        public Task_CellBody(int id, TimeSpan runLength, CellBody body) //tested
        {
            Console.WriteLine("Task_CellBody " + id + " is created.");

            this.id = id;
            this.runLength = runLength;
            this.body = body;
        }


        //properties
        public int Id
        {
            get
            {
                return this.id;
            }
            private set
            {
                id = value;
            }
        }

        public CellBody Body
        {
            get
            {
                return this.body;

            }
            private set
            {
                body = value;
            }
        }


        //public methods
        public override String ToString() //tested
        {
            return "Task_CellBody{ id=" + Id + " }";
        }


        public void Consume()
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("Task_CellBody {0} is consuming...", Id);

            while (DateTime.Now - start < runLength)
            {
                int voltage = body.TryRemoveFromBuffer();
                //Console.WriteLine("Task_CellBody {0} removed {1} from buffer.", Id, voltage);
            }//end while

            Console.WriteLine("Task_CellBody {0} is done.", Id);
        }

        public void Decay() //tested
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("Task_CellBody {0} is decaying...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(body.DecayFrequency);
                body.DecayMembranePotential();
                //Console.WriteLine("Task_CellBody {0} decayed cell body membrane potential to {1}.", Id, body.MembranePotential);
            }
            Console.WriteLine("Task_CellBody {0} is done.", Id);
        }


        ////tests
        //public static void Main()
        //{
        //    Console.WriteLine("Test for Constructor 1");
        //    Task_CellBody tcb = new Task_CellBody(1, new TimeSpan(0, 0, 2), new CellBody(DateTime.Now, 100));
        //    Console.WriteLine(tcb);
        //    Console.WriteLine();

        //    Console.WriteLine("Test for Constructor 1");
        //    tcb.Decay();
        //    Console.WriteLine();

        //}
    }
}
