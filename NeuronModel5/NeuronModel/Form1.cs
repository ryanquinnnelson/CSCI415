using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace NeuronModel
{
    public partial class ProcessManagerForm : Form
    {
        //neuron model variables
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
        private static int nextCellBodyTaskId = 0;
        private static int nextDendriteTaskId = 0;
        private static int nextInputAxonTaskId = 0;


        //output variables
        private static double[] xValues;
        private static int[] yValues;

        public ProcessManagerForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //run neuron model
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
                                100,              //*neuron secondary messenger frequency trigger
                                2,              //*number of dendrites to add in growth event
                                new int[] { 0,0 },   //*types of dendrites to add in growth event
                                10,             //*dendrite decay frequency
                                100,            //*dendrite production frequency
                                51,             //*dendrite restore increment
                                1,              //*number of synapses to add in growth event
                                window,         //*dendrite secondary messenger window
                                3810,              //*dendrite secondary messenger frequency trigger
                                4,              //*number of starting synapses per dendrite
                                25,              //*dendrite significant voltage change amount //at 25, too frequent; at 26, no occurrence
                                1,              //*number of starting dendrites
                                new int[] { 0 });  //*types of dendrites to start

            tasks = new List<Task>();
            runLength = new TimeSpan(0, 0, 12);
            start = DateTime.Now;
            nextInputAxonId = 0;
            inputs = new List<InputAxon>();


            //listen for CellGrowthEvent from Neuron
            neuron.CellGrowthEvent += RespondToCellGrowthEvent;

            //listen for DendriteGrowthEvent from each Dendrite
            foreach (Dendrite d in neuron.Dendrites)
            {
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
                    new Task_CellBody(id, runLength, neuron.Body, start).Consume();
                }, nextCellBodyTaskId++);
                tasks.Add(newest);
            }

            //Decayers to decay membrane potential of cell body
            for (int i = 0; i < NUM_CELLBODY_DECAYERS; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_CellBody(id, runLength, neuron.Body, start).Decay();
                }, nextCellBodyTaskId++);
                tasks.Add(newest);
            }

            //====================================================================//
            //                             dendrite                               //
            //====================================================================//
            //Consumers to retrieve neurotransmitters from dendrite buffer
            for (int k = 0; k < neuron.Dendrites.Count; k++)
            {
                for (int i = 0; i < NUM_DENDRITE_CONSUMERS; i++)
                {
                    Task newest = Task.Factory.StartNew((Object obj) =>
                    {
                        var data = (dynamic)obj;
                        new Task_Dendrite(data.id, runLength, neuron.GetDendrite(data.index), neuron.Body, start).Consume();
                    }, new { id = nextDendriteTaskId++, index = k });
                    tasks.Add(newest);
                }

                //Producers to send electrical potential to cell body buffer
                for (int i = 0; i < NUM_DENDRITE_PRODUCERS; i++)
                {
                    Task newest = Task.Factory.StartNew((Object obj) =>
                    {
                        var data = (dynamic)obj;
                        new Task_Dendrite(data.id, runLength, neuron.GetDendrite(data.index), neuron.Body, start).Produce();
                    }, new { id = nextDendriteTaskId++, index = k });
                    tasks.Add(newest);
                }

                //Decayers to decay membrane potential of dendrites
                for (int i = 0; i < NUM_DENDRITE_DECAYERS; i++)
                {
                    Task newest = Task.Factory.StartNew((Object obj) =>
                    {
                        var data = (dynamic)obj;
                        new Task_Dendrite(data.id, runLength, neuron.GetDendrite(data.index), neuron.Body, start).Decay();
                    }, new { id = nextDendriteTaskId++, index = k });
                    tasks.Add(newest);
                }


                //====================================================================//
                //                               input                                //
                //====================================================================//
                //Producers to send neurotransmitters to dendrites
                for (int i = 0; i < NUM_INPUTAXON_PRODUCERS; i++)
                {
                    Task newest = Task.Factory.StartNew((Object obj) =>
                    {
                        var data = (dynamic)obj;
                        InputAxon axon = new InputAxon(nextInputAxonId++, INPUTAXON_PRODUCTION_FREQUENCY, 0);
                        inputs.Add(axon);
                        new Task_InputAxon(data.id, runLength, axon, INPUT_MAGNITUDE, start, neuron.GetDendrite(data.index)).Produce();
                    }, new { id = nextInputAxonTaskId++, index = k });
                    tasks.Add(newest);
                }
            }


            Task.WaitAll(tasks.ToArray(), cts.Token);

            //output results
            ConvertEventRecordsToArrays();
            //PrintXValuesArray();
            //PrintYValuesArray();
            Console.WriteLine("Stopped Neuron Model.");
        




            //display chart
            for (int i = 0; i < xValues.Length; i++)
            {
                chart1.Series[0].Points.AddXY(xValues[i], yValues[i]);
            }
            //chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;

            //color of grid background
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.LineColor = Color.Gainsboro;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.Gainsboro;

            
        }

        //static methods
        private static void OutputResults()
        {
            Console.WriteLine();
            foreach (EventRecord r in neuron.Body.GetOutputsAsList())
            {
                Console.WriteLine(r);
            }
        }

        private static void ConvertEventRecordsToArrays()
        {
            List<EventRecord> eventRecords = neuron.Body.GetOutputsAsList();
            int numberOfRecords = eventRecords.Count;

            xValues = new double[numberOfRecords];
            yValues = new int[numberOfRecords];

            for(int i = 0; i < numberOfRecords; i++)
            {
                xValues[i] = eventRecords[i].Time;
                yValues[i] = eventRecords[i].MembranePotential;
            }
        }

        private static void PrintXValuesArray()
        {
            for(int i = 0; i < xValues.Length; i++)
            {
                Console.WriteLine(xValues[i]);
            }
        }

        private static void PrintYValuesArray()
        {
            for(int i = 0; i < yValues.Length; i++)
            {
                Console.WriteLine(yValues[i]);
            }
        }

        private static void RespondToCellGrowthEvent(object sender, EventArgs_CellGrowth e)
        {
            TimeSpan difference = DateTime.Now.Subtract(start);
            String s = difference.Seconds + "." + difference.Milliseconds;
            Console.WriteLine("ProcessManager received cell growth event at " + s);
            List<Dendrite> added = e.DendritesAdded;

            //add Tasks to manipulate dendrites added
            //====================================================================//
            //                             dendrite                               //
            //====================================================================//
            //Consumers to retrieve neurotransmitters from dendrite buffer
            for (int i = 0; i < NUM_DENDRITE_CONSUMERS; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, runLength, added[0], neuron.Body, start).Consume();
                }, nextDendriteTaskId++);
                tasks.Add(newest);
            }

            //Producers to send electrical potential to cell body buffer
            for (int i = 0; i < NUM_DENDRITE_PRODUCERS; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, runLength, added[0], neuron.Body, start).Produce();
                }, nextDendriteTaskId++);
                tasks.Add(newest);
            }

            //Decayers to decay membrane potential of dendrites
            for (int i = 0; i < NUM_DENDRITE_DECAYERS; i++)
            {
                Task newest = Task.Factory.StartNew((val) =>
                {
                    int id = (int)val;
                    new Task_Dendrite(id, runLength, added[0], neuron.Body, start).Decay();
                }, nextDendriteTaskId++);
                tasks.Add(newest);
            }





            //add Tasks to produce to dendrites added
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
                    new Task_InputAxon(id, runLength, axon, INPUT_MAGNITUDE, start, added[0]).Produce();
                }, nextInputAxonTaskId++);
                tasks.Add(newest);
            }


        }

        private static void RespondToDendriteGrowthEvent(object sender, EventArgs_DendriteGrowth e)
        {

            TimeSpan difference = DateTime.Now.Subtract(start);
            String s = difference.Seconds + "." + difference.Milliseconds;
            Console.WriteLine("ProcessManager received dendrite growth event at " + s);
            //Dendrite dendrite = neuron.GetDendrite(e.DendriteId);

            //add Tasks to produce to dendrites added
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
                    new Task_InputAxon(id, runLength, axon, INPUT_MAGNITUDE, start).ConnectAndProduce(neuron);
                }, nextInputAxonTaskId++);
                tasks.Add(newest);
            }
        }
    }
}
