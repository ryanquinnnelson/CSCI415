using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace SynapseModel3
{
    public class ProcessManager
    {
        const int NUM_CELLBODY_CONSUMERS = 1;
        const int NUM_CELLBODY_DECAYERS = 1;
        const int NUM_DENDRITE_CONSUMERS = 1;
        const int NUM_DENDRITE_PRODUCERS = 2;
        const int NUM_DENDRITE_DECAYERS = 1;
        const int NUM_INPUTAXON_PRODUCERS = 2;
        const int INPUT_MAGNITUDE = 26; //Volts
        const int INPUTAXON_PRODUCTION_FREQUENCY = 10; //ms

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

            //look at the previous 2 seconds
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 2;
            int milliseconds = 0;
            TimeSpan window = new TimeSpan(days, hours, minutes, seconds, milliseconds);


            //initialize variables
            neuron = new Neuron(10,             //*cell body decay frequency
                                50,             //*cell body restore increment
                                window,         //*neuron secondary messenger window
                                2,              //*neuron secondary messenger frequency trigger
                                1,              //*number of dendrites to add in growth event
                                new int[]{0},   //*types of dendrites to add in growth event
                                10,             //*dendrite decay frequency
                                100,            //*dendrite production frequency
                                50,             //*dendrite restore increment
                                1,              //*number of synapses to add in growth event
                                window,         //*dendrite secondary messenger window
                                2,              //*dendrite secondary messenger frequency trigger
                                2,              //*number of starting synapses per dendrite
                                50,            //*dendrite significant voltage change amount
                                1,              //*number of starting dendrites
                                new int[]{0});  //*types of dendrites to start
            
            tasks = new List<Task>();
            runLength = new TimeSpan(0, 0, 10);
            start = DateTime.Now;
            nextInputAxonId = 0;
            inputs = new List<InputAxon>();


            //listen for CellGrowthEvent from Neuron
            neuron.CellGrowthEvent += RespondToCellGrowthEvent;

            //listen for DendriteGrowthEvent from each Dendrite
            foreach(Dendrite d in neuron.Dendrites){
                d.DendriteGrowthEvent += RespondToDendriteGrowthEvent;
            }


            //create tasks
            //====================================================================//
            //                            cell body                               //
            //====================================================================//
            //Consumers to retrieve electrical potential from cell body buffer
            for (int i = 0; i < NUM_CELLBODY_CONSUMERS; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_CellBody(id, runLength, neuron.Body).Consume();
                }, i);
                tasks.Add(newest);
            }

            //Decayers to decay membrane potential of cell body
            for (int i = 0; i < NUM_CELLBODY_DECAYERS; i++)
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
            for (int i = 0; i < NUM_DENDRITE_CONSUMERS; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, runLength, neuron.GetDendrite(0), neuron.Body).Consume();
                }, i);
                tasks.Add(newest);
            }

            //Producers to send electrical potential to cell body buffer
            for (int i = 0; i < NUM_DENDRITE_PRODUCERS; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, runLength, neuron.GetDendrite(0), neuron.Body).Produce();
                }, i);
                tasks.Add(newest);
            }

            //Decayers to decay membrane potential of dendrites
            for (int i = 0; i < NUM_DENDRITE_DECAYERS; i++)
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
            for (int i = 0; i < NUM_INPUTAXON_PRODUCERS; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    InputAxon axon = new InputAxon(nextInputAxonId++, INPUTAXON_PRODUCTION_FREQUENCY, 0);
                    inputs.Add(axon);
                    new Task_InputAxon(id, runLength, axon, INPUT_MAGNITUDE, neuron.GetDendrite(0)).Produce();
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
            Console.WriteLine("ProcessManager received cell growth event.");
        }

        private static void RespondToDendriteGrowthEvent(object sender, EventArgs_DendriteGrowth eventArgs){
            //to be implemented
            Console.WriteLine("ProcessManager received dendrite growth event.");
        }
    }
}
