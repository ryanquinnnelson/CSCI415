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

    public enum CellGrowthState{
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
            Neuron n1 = new Neuron();






            //this.start = start;
            //List<Task> tasks = new List<Task>();

            ////create tasks
            ////Record membrane potential of cell body
            //Task t_recorder = Task.Factory.StartNew(() =>
            //{
            //    new Recorder(start).Work(body, runLength);
            //});
            //tasks.Add(t_recorder);

            ////Consumers to retrive electrical potential from cell body buffer
            //for (int i = 0; i < numCellBodyConsumers; i++)
            //{
            //    Task newest = Task.Factory.StartNew((val) =>
            //    {
            //        int id = (int)val;
            //        new Consumer_CellBody(id).Work(body, runLength);
            //    }, i);
            //    tasks.Add(newest);
            //}

            ////For each dendrite
            //foreach (Dendrite d in dendrites)
            //{
            //    //Consumers to retrieve neurotransmitters from dendrite buffer
            //    for (int i = 0; i < numDendriteConsumers; i++)
            //    {
            //        Task newest = Task.Factory.StartNew((val) =>
            //        {
            //            int id = (int)val;
            //            new Consumer_Dendrite(id).Work(d, runLength);
            //        }, i);
            //        tasks.Add(newest);
            //    }

            //    //Producers to send electrical potential to cell body buffer
            //    for (int i = 0; i < numDendriteProducers; i++)
            //    {
            //        Task newest = Task.Factory.StartNew((val) =>
            //        {
            //            int id = (int)val;
            //            new Producer_Dendrite(id, 100).Work(d, body, runLength);
            //        }, i);
            //        tasks.Add(newest);
            //    }
            //}//end foreach

            //Task.WaitAll(tasks.ToArray());
            ////wait for those tasks to finish
            //return tasks;








            //for (int i = 0; i < 1; i++)
            //{
            //    Task newest = Task.Factory.StartNew((val) =>
            //    {
            //        int id = (int)val;
            //        new Producer_Axon(id, 100).Work(dt1, runLength);
            //    }, i);
            //    tasks.Add(newest);
            //}



            //Task.WaitAll(neuronTasks.ToArray());
            Console.WriteLine("Stopped Neuron Model.");
        }
    }
}
