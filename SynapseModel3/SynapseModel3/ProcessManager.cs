using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace SynapseModel3
{
    public class ProcessManager
    {

        private static Neuron neuron;
        private static TimeSpan runLength;
        private static DateTime start;
        private static List<Task> tasks;
        private static List<InputAxon> inputs;
        private static int nextInputAxonId;
        private static CancellationTokenSource cts = new CancellationTokenSource();

        public static void Main()
        {
            Console.WriteLine("Running Neuron Model...");

            //initialize variables
            neuron = new Neuron(10, 10, 10); //maybe move secondary messenger configuration out here too??
            tasks = new List<Task>();
            runLength = new TimeSpan(0, 0, 2);
            start = DateTime.Now;
            nextInputAxonId = 0;
            inputs = new List<InputAxon>();


            //listen for CellGrowthEvent from Neuron
            neuron.CellGrowthEvent += RespondToCellGrowthEvent;

            //listen for DendriteGrowthEvent from each Dendrite
            List<Dendrite> dendrites = neuron.Dendrites;
            foreach(Dendrite d in dendrites){
                d.DendriteGrowthEvent += RespondToDendriteGrowthEvent;
            }


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
                    new Task_CellBody(id, runLength, neuron.Body).Consume();
                }, i);
                tasks.Add(newest);
            }

            //Decayers to decay membrane potential of cell body
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_CellBody(id, runLength, neuron.Body).Decay();
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
                    new Task_Dendrite(id, runLength, neuron.GetDendrite(0), neuron.Body).Consume();
                }, i);
                tasks.Add(newest);
            }

            //Producers to send electrical potential to cell body buffer
            for (int i = 0; i < 2; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, runLength, neuron.GetDendrite(0), neuron.Body).Produce();
                }, i);
                tasks.Add(newest);
            }

            //Decayers to decay membrane potential of dendrites
            for (int i = 0; i < 1; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, runLength, neuron.GetDendrite(0), neuron.Body).Decay();
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
                    InputAxon axon = new InputAxon(nextInputAxonId++, 10, 0);
                    inputs.Add(axon);
                    new Task_InputAxon(id, runLength, axon, neuron.GetDendrite(0)).Produce();
                }, i);
                tasks.Add(newest);
            }


            //currently if any consumers are trying to consume an empty buffer, they block the entire program??



            Task.WaitAll(tasks.ToArray(), cts.Token);

            //output results
            OutputResults();
            Console.WriteLine("Stopped Neuron Model.");
        }




        //static methods
        private static void OutputResults(){
            Console.WriteLine();
            foreach (EventRecord r in neuron.Body.GetOutputsAsList())
            {
                Console.WriteLine(r);
            }
        }

        private static void RespondToCellGrowthEvent(object sender, EventArgs_CellGrowth e){
            //to be implemented
        }

        private static void RespondToDendriteGrowthEvent(object sender, EventArgs_DendriteGrowth eventArgs){
            //to be implemented
        }
    }
}
