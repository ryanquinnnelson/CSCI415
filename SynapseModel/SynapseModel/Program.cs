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
        private const int SECONDS = 10;
        private static DateTime start;

        public static void Main(string[] args)
        {
            Console.WriteLine("Running Neuron Model...");

            //setup
            start = DateTime.Now;
            TimeSpan runLength = new TimeSpan(0, 0, SECONDS);
            TimeSpan runLength_long = new TimeSpan(0, 0, SECONDS * 2);
            List<Task> tasks = new List<Task>();
            Neuron neuron = new Neuron();


            //create tasks
            ////Record membrane potential of cell body
            //Task t_recorder = Task.Factory.StartNew(() =>
            //{
            //    new Recorder(start).Work(neuron.Body, runLength);
            //});
            //tasks.Add(t_recorder);

            //====================================================================//
            //                            cell body                               //
            //====================================================================//
            //Consumers to retrieve electrical potential from cell body buffer
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_CellBody(id, 10).Consume(neuron.Body, runLength);
                }, i);
                tasks.Add(newest);
            }

            //Decayers to decay membrane potential of cell body
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_CellBody(id, 10).Decay(neuron.Body, runLength);
                }, i);
                tasks.Add(newest);
            }

            //====================================================================//
            //                             dendrite                               //
            //====================================================================//
            //Consumers to retrieve neurotransmitters from dendrite buffer
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, 10).Consume(neuron.GetDendrite(0), runLength);
                }, i);
                tasks.Add(newest);
            }

            //Producers to send electrical potential to cell body buffer
            for (int i = 0; i < 2; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, 10).Produce(neuron.GetDendrite(0), neuron.Body, runLength);
                }, i);
                tasks.Add(newest);
            }

            //Decayers to decay membrane potential of dendrites
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, 10).Decay(neuron.GetDendrite(0), runLength);
                }, i);
                tasks.Add(newest);
            }


            //====================================================================//
            //                               input                                //
            //====================================================================//
            //Producers to send neurotransmitters to dendrites
            for (int i = 0; i < 2; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Input(id, 10).Produce(neuron.GetDendrite(0), runLength);
                }, i);
                tasks.Add(newest);
            }




            Task.WaitAll(tasks.ToArray());
            Console.WriteLine("Stopped Neuron Model.");
        }
    }
}
