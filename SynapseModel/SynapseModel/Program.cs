using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SynapseModel
{

    public enum DendriteType
    {
        Proximal,
        Basal,
        Apical
    }

    public enum DendriteGrowthState
    {
        NoGrowth,
        Growth
    }

    public enum CellGrowthState
    {
        NoGrowth,
        Growth
    }


    class MainClass
    {
        private const int SECONDS = 1;
        private static DateTime start;

        public static void Main(string[] args)
        {
            Console.WriteLine("Running Neuron Model...");

            //setup
            start = DateTime.Now;
            TimeSpan runLength = new TimeSpan(0, 0, SECONDS);
            List<Task> tasks = new List<Task>();
            Neuron neuron = new Neuron();


            //create tasks
            //Record membrane potential of cell body
            Task t_recorder = Task.Factory.StartNew(() =>
            {
                new Recorder(start).Work(neuron.Body, runLength);
            });
            tasks.Add(t_recorder);

            //Consumers to retrieve electrical potential from cell body buffer
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Consumer_CellBody(id).Work(neuron.Body, runLength);
                }, i);
                tasks.Add(newest);
            }

            //Consumers to retrieve neurotransmitters from dendrite buffer
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, 100).Consume(neuron.GetDendrite(0), runLength);
                }, i);
                tasks.Add(newest);
            }

            //Producers to send electrical potential to cell body buffer
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, 100).Produce(neuron.GetDendrite(0), neuron.Body, runLength);
                }, i);
                tasks.Add(newest);
            }

            //Decayers to decay membrane potential of dendrites
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, 100).Decay(neuron.GetDendrite(0), runLength);
                }, i);
                tasks.Add(newest);
            }

            //Producers to send neurotransmitters to dendrites
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Producer_Axon(id, 100).Work(neuron.GetDendrite(0), runLength);
                }, i);
                tasks.Add(newest);
            }




            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Stopped Neuron Model.");
        }
    }
}
