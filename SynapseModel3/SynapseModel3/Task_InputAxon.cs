using System;
using System.Threading;

namespace SynapseModel3
{
    public class Task_InputAxon
    {
        //fields
        private Dendrite dendrite;
        private InputAxon axon;
        private int id;
        private TimeSpan runLength;

        //constructors
        public Task_InputAxon(int id, TimeSpan runLength, InputAxon axon, Dendrite dendrite) //tested
        {
            Console.WriteLine("Task_AxonInput " + id + " is created.");
            this.id = id;
            this.runLength = runLength;
            this.dendrite = dendrite;
            this.axon = axon;
        }

        public Task_InputAxon(int id, TimeSpan runLength, InputAxon axon) //tested
        {
            Console.WriteLine("Task_AxonInput " + id + " is created.");
            this.id = id;
            this.runLength = runLength;
            this.dendrite = null;
            this.axon = axon;
        }


        //properties
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

        public InputAxon Axon //tested
        {
            get
            {
                return this.axon;
            }
            private set
            {
                axon = value;
            }
        }

        public Dendrite Dendrite //tested
        {
            get
            {
                return this.dendrite;
            }
            set
            {
                if (dendrite == null)
                {
                    dendrite = value;
                }
            }
        }

        //public methods
        public override String ToString() //tested
        {
            return "Task_InputAxon{ id=" + Id + ", runLength=" + runLength
                + ", inputAxon=" + axon + ", dendrite=" + dendrite + " }";
        }


        public void Produce() //tested
        {
            if (dendrite == null)
            {
                Console.WriteLine("Dendrite was NULL.");
                return; //nothing to produce to
            }

            DateTime start = DateTime.Now; //??maybe this needs to be passed to object
            Console.WriteLine("Task_InputAxon {0} is producing...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(axon.ProductionFrequency);
                Neurotransmitter newest = new Neurotransmitter(50);
                dendrite.AddToBuffer(newest);

                //Console.WriteLine("Task_InputAxon {0} added {1} to dendrite {2} buffer.", Id, newest.Charge, dendrite.Id);

            }//end while

            Console.WriteLine("Task_InputAxon {0} is done.", Id);
        }

        public void ConnectAndProduce()
        {
            //find an open synapse if possible, otherwise terminate
            //??to be implemented
        }


        ////tests
        //public static void Main()
        //{
        //    Console.WriteLine("Test for Constructor 1");
        //    Task_InputAxon tia = new Task_InputAxon(1, new TimeSpan(0, 0, 2), new InputAxon(1, 100, 1));
        //    Console.WriteLine(tia);
        //    Console.WriteLine();

        //    Console.WriteLine("Test for Constructor 2");
        //    Task_InputAxon tia2 = new Task_InputAxon(1, new TimeSpan(0, 0, 2), new InputAxon(1, 100, 1), new Dendrite(1,1,3));
        //    Console.WriteLine(tia2);
        //    Console.WriteLine();

        //    Console.WriteLine("Test for SetDendrite()");
        //    Dendrite d = new Dendrite(0, 0, 5);
        //    Synapse s = d.GetOpenSynapse();
        //    d.FormSynapseConnection(s, tia.axon);
        //    tia.Dendrite = d;
        //    Console.WriteLine(tia);
        //    Console.WriteLine();


        //    Console.WriteLine("Test for Produce()");
        //    tia.Produce();
        //    Console.WriteLine();
        //}

    }
}
