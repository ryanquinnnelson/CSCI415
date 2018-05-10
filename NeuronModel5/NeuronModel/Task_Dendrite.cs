//Ryan Nelson and Brian Engelbrecht

using System;
using System.Threading;

namespace NeuronModel
{
    public class Task_Dendrite
    {
        //fields
        private CellBody body;
        private Dendrite dendrite;
        private int id;
        private TimeSpan runLength;
        private DateTime start;


        //constructors
        public Task_Dendrite(int id, TimeSpan runLength, Dendrite dendrite, CellBody body, DateTime start) //tested
        {
            Console.WriteLine("Task_Dendrite " + id + " is created.");
            //Console.WriteLine("Dendrite id= " + dendrite.Id);

            this.id = id;
            this.dendrite = dendrite;
            this.runLength = runLength;
            this.body = body;
            this.start = start;
        }


        //properties
        public CellBody Body //tested
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

        public Dendrite Dendrite //tested
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

        public int Id //tested
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
        public void Consume() //tested
        {
            Console.WriteLine("Task_Dendrite {0} is consuming...", Id);
            while (DateTime.Now - start < runLength)
            {
                int voltage = dendrite.TryRemoveFromBuffer();
                //Console.WriteLine("Task_Dendrite {0} consumed {1} from buffer.", Id, voltage);
            }//end while

            Console.WriteLine("Task_Dendrite {0} is done.", Id);
        }

        public void Decay() //tested
        {
            Console.WriteLine("Task_Dendrite {0} is decaying...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(dendrite.DecayFrequency); //simulate biology speed
                dendrite.DecayMembranePotential();
                //Console.WriteLine("Task_Dendrite {0} decayed dendrite membrane potential to {1}.", Id, dendrite.MembranePotential);
            }
            Console.WriteLine("Task_Dendrite {0} is done.", Id);
        }

        public void Produce() //tested
        {
            Console.WriteLine("Task_Dendrite {0} is producing...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(dendrite.ProductionFrequency); //simulate biology speed
                int difference = dendrite.GetMembranePotentialDifference();
                body.AddToBuffer(difference);

                //Console.WriteLine("Task_Dendrite {0} produced {1} to cell body buffer.", Id, difference);
            }//end while

            Console.WriteLine("Task_Dendrite {0} is done.", Id);
        }

        public override String ToString() //tested
        {
            return "Task_Dendrite{ id=" + Id + " }";
        }

       
        ////tests
        //public static void Main()
        //{
        //    Console.WriteLine("Test for Constructor 1");
        //    Task_Dendrite td = new Task_Dendrite(1, 
        //                                         new TimeSpan(0, 0, 2),
        //                                         new Dendrite(1,1,100,100,50,1,new TimeSpan(0,0,2),100,1),
        //                                         new CellBody(DateTime.Now,100,50));

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
