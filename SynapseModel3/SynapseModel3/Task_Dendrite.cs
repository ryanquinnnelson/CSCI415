using System;
using System.Threading;

namespace SynapseModel3
{
    public class Task_Dendrite
    {
        //fields
        private CellBody body;
        private Dendrite dendrite;
        private int id;
        private TimeSpan runLength;


        //constructors
        public Task_Dendrite(int id, TimeSpan runLength, Dendrite dendrite, CellBody body)
        {
            Console.WriteLine("Task_Dendrite " + id + " is created.");

            this.id = id;
            this.dendrite = dendrite;
            this.runLength = runLength;
            this.body = body;
        }


        //properties
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

        public Dendrite Dendrite
        {
            get
            {
                return this.dendrite;
            }
            private set
            {
                dendrite = value;
            }
        }

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


        //public methods
        public void Consume()
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("Task_Dendrite {0} is consuming...", Id);
            while (DateTime.Now - start < runLength)
            {
                int voltage = dendrite.TryRemoveFromBuffer();
                //Console.WriteLine("Task_Dendrite {0} consumed {1} from buffer.", Id, voltage);
            }//end while

            Console.WriteLine("Task_Dendrite {0} is done.", Id);
        }

        public void Decay()
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("Task_Dendrite {0} is decaying...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(dendrite.DecayFrequency);
                dendrite.DecayMembranePotential();
                //Console.WriteLine("Task_Dendrite {0} decayed dendrite membrane potential to {1}.", Id, dendrite.MembranePotential);
            }
            Console.WriteLine("Task_Dendrite {0} is done.", Id);
        }

        public void Produce()
        {
            DateTime start = DateTime.Now;
            Console.WriteLine("Task_Dendrite {0} is producing...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(dendrite.ProductionFrequency);
                int difference = dendrite.GetMembranePotentialDifference();
                body.AddToBuffer(difference);

                //Console.WriteLine("Task_Dendrite {0} produced {1} to cell body buffer.", Id, difference);
            }//end while

            Console.WriteLine("Task_Dendrite {0} is done.", Id);
        }

        public override String ToString()
        {
            return "Task_Dendrite{ id=" + Id + " }";
        }

       
        ////tests
        //public static void Main()
        //{
        //    Console.WriteLine("Test for Constructor 1");
        //    Task_Dendrite td = new Task_Dendrite(1, new TimeSpan(0, 0, 2),
        //                                         new Dendrite(0, 0, 5, 100, 100),
        //                                         new CellBody(DateTime.Now, 100));

        //    Console.WriteLine(td);
        //    Console.WriteLine();

        //    Console.WriteLine("Test for Produce()");
        //    td.Produce();
        //    Console.WriteLine();

        //    Console.WriteLine("Test for Decay()");
        //    td.Decay();
        //    Console.WriteLine();
        //}
    }
}
