using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

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
        
        private const int SECONDS = 8;
        private static DateTime start;
        private static CancellationTokenSource cts = new CancellationTokenSource();

        public static void Main(string[] args)
        {
            Console.WriteLine("Running Neuron Model...");

            //setup
            start = DateTime.Now;
            TimeSpan runLength = new TimeSpan(0, 0, SECONDS);
            TimeSpan runLength_long = new TimeSpan(0, 0, SECONDS * 2);
            List<Task> tasks = new List<Task>();
            Neuron neuron = new Neuron(start, tasks);
            neuron.CellGrowthEvent += main_CellGrowthTriggered; //add event handler to event
            //neuron.Body.ActionPotentialEventTest();

            //create tasks
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


            Task.WaitAll(tasks.ToArray(),cts.Token);
            //neuron.Body.results.Sort(); //need comparator for this
            foreach (Record r in neuron.Body.results)
            {
                Console.WriteLine(r);
            }
            Console.WriteLine("Stopped Neuron Model.");
        }//end Main()







        public static void main_CellGrowthTriggered(object sender, CellGrowthEventArgs e){

            Neuron n = e.neuron;
            DendriteType type = e.type;
            TimeSpan runLength = e.timespan;
            List<Task> tasks = e.tasks;

            Console.WriteLine("Triggered.");

           

            Dendrite d = n.AddDendrite(type);

            //Consumer to consume neurotransmitters from dendrite buffer
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, 10).Consume(d, runLength);
                }, i);
                tasks.Add(newest);
            }

            //Producers to send electrical potential to cell body buffer
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, 10).Produce(d, n.Body, runLength);
                }, i);
                tasks.Add(newest);
            }

            ////Decayers to decay membrane potential of dendrites
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, 10).Decay(d, runLength);
                }, i);
                tasks.Add(newest);
            }
            //Console.WriteLine("Added.");

            //reset Token so system waits for new tasks too
            //do
            //{
                //try
                //{
                //    cts = new CancellationTokenSource();
                //    Task.WaitAll(tasks.ToArray(), cts.Token);
                //}
                //catch (OperationCanceledException)
                //{
                //    // start over and wait for new tasks
                //}
            //}
            //while (cts.IsCancellationRequested);
        }//end BuildDendrite()
    }
}
