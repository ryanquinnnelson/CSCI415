

            

            

            


            
            //neuron.Body.results.Sort(); //need comparator for this
            
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
