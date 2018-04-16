using System;
using System.Threading;

namespace SynapseModel3
{
    public class Task_InputAxon
    {
        //fields
        private InputAxon axon;
        private Dendrite dendrite;
        private int id;
        private TimeSpan runLength;
        private int neurotransmitterMagnitude;

        //constructors
        public Task_InputAxon(int id, TimeSpan runLength, InputAxon axon, int magnitude, Dendrite dendrite) //tested
        {
            Console.WriteLine("Task_AxonInput " + id + " is created.");
            this.id = id;
            this.runLength = runLength;
            bool success = dendrite.TryConnect(axon);
            this.axon = axon;
            this.neurotransmitterMagnitude = magnitude;

            if (success)
            {
                this.dendrite = dendrite;
            }
        }

        public Task_InputAxon(int id, TimeSpan runLength, InputAxon axon, int magnitude) //tested
        {
            Console.WriteLine("Task_AxonInput " + id + " is created.");
            this.id = id;
            this.runLength = runLength;
            this.dendrite = null;
            this.axon = axon;
            this.neurotransmitterMagnitude = magnitude;
        }


        //properties
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
        public void ConnectAndProduce(Neuron neuron) //tested
        {
            //find an open synapse
            Dendrite result = neuron.SearchForOpenSynapse(axon);

            if (result != null)
            {
                Console.WriteLine("InputAxon " + id + " found dendrite " + result);
                this.dendrite = result;
                Produce();
            }
        }

        public void Produce() //tested
        {
            if (dendrite == null)
            {
                Console.WriteLine("Dendrite is NULL.");
                return; //nothing to produce to
            }

            DateTime start = DateTime.Now;
            Console.WriteLine("Task_InputAxon {0} is producing...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(axon.ProductionFrequency);
                Neurotransmitter newest = new Neurotransmitter(neurotransmitterMagnitude);
                dendrite.AddToBuffer(newest);

                //Console.WriteLine("Task_InputAxon {0} added {1} to dendrite {2} buffer.", Id, newest.Charge, dendrite.Id);

            }//end while

            Console.WriteLine("Task_InputAxon {0} is done.", Id);
        }

        public override String ToString() //tested
        {
            return "Task_InputAxon{ id=" + Id + ", runLength=" + runLength
                + ", inputAxon=" + axon + ", dendrite=" + dendrite + " }";
        }


        ////tests
        //public static void Main()
        //{
        //    Neuron neuron = new Neuron(10,                          //*cell body decay frequency
        //                               50,                          //*cell body restore increment
        //                               new TimeSpan(0,0,2),         //*neuron secondary messenger window
        //                               100,                         //*neuron secondary messenger frequency trigger
        //                               1,                           //*number of dendrites to add in growth event
        //                               new int[] { 0 },             //*types of dendrites to add in growth event
        //                               10,                          //*dendrite decay frequency
        //                               100,                         //*dendrite production frequency
        //                               96,                          //*dendrite restore increment
        //                               1,                           //*number of synapses to add in growth event
        //                               new TimeSpan(0,0,2),         //*dendrite secondary messenger window
        //                               100,                         //*dendrite secondary messenger frequency trigger
        //                               2,                           //*number of starting synapses per dendrite
        //                               1,                           //*number of starting dendrites
        //                               new int[] { 0 });            //*types of dendrites to start
            
        //    Dendrite dendrite = new Dendrite(1, 1, 100, 100, 50, 1, new TimeSpan(0, 0, 2), 100, 1);

        //    Console.WriteLine("Test for Constructor 1");
        //    Task_InputAxon tia = new Task_InputAxon(1, new TimeSpan(0, 0, 2), new InputAxon(1, 100, 1), 10);
        //    Console.WriteLine(tia);
        //    Console.WriteLine();

        //    Console.WriteLine("Test for Constructor 2");
        //    Task_InputAxon tia2 = new Task_InputAxon(1, new TimeSpan(0, 0, 2), new InputAxon(1, 100, 1), 10, dendrite);
        //    Console.WriteLine(tia2);
        //    Console.WriteLine();

        //    Console.WriteLine("Test for Produce() fail");
        //    tia.Produce();
        //    Console.WriteLine();

        //    Console.WriteLine("Test for Produce() succeed");
        //    tia2.Produce();
        //    Console.WriteLine();

        //    Console.WriteLine("Test for ConnectAndProduce()");
        //    tia.ConnectAndProduce(neuron);
        //}

    }
}
