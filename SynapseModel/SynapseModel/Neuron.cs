using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SynapseModel
{
    public class Neuron
    {
        private DateTime start;
        private int nextDendriteId = 0;
        private int nextAxonId = 0;
        private CellGrowthState state;
        private CellBody body;
        private List<Dendrite> dendrites;
        private List<Axon> axons;
        private List<Task> main_tasks;

        public Neuron(DateTime start, List<Task> tasks)
        {
            state = CellGrowthState.NoGrowth;
            body = new CellBody(start);
            dendrites = new List<Dendrite>();
            axons = new List<Axon>();
            this.start = start;
            main_tasks = tasks;

            InitializeDendrites();
            InitializeAxons();
        }

        public List<Dendrite> Dendrites{
            get{
                return dendrites;
            }
            private set{
                dendrites = value;
            }
        }

        public CellBody Body{
            get{
                return body;
            }
            private set{
                body = value;
            }

        }

        public List<Axon> Axons{
            get{
                return axons;
            }
            set{
                axons = value;
            }
        }

        public CellGrowthState State{
            get{
                return state;
            }
            private set{
                state = value;
            }
        }

        public Dendrite GetDendrite(int index){
            return dendrites[index];
        }


        private void InitializeDendrites()
        {
            Dendrite d1 = new Dendrite(nextDendriteId++, DendriteType.Proximal);
            dendrites.Add(d1);
        }

        private void InitializeAxons()
        {
            //to be implemented
        }

        public Dendrite AddDendrite(DendriteType type){
            Dendrite d = new Dendrite(nextDendriteId++, type);
            dendrites.Add(d);
            return d;
        }





        //test for event
        public void TestEvent(){
            Console.WriteLine("test event");

            CellGrowthEventArgs args = new CellGrowthEventArgs();
            args.neuron = this;
            args.type = DendriteType.Proximal;
            args.timespan = new TimeSpan(0, 0, 2);
            args.tasks = main_tasks;

            OnCellGrowthTriggered(args);
        }

        protected virtual void OnCellGrowthTriggered(CellGrowthEventArgs e){
            EventHandler<CellGrowthEventArgs> handler = CellGrowthTriggered;
            if(handler != null){
                handler(this, e);
            }
        }

        public event EventHandler<CellGrowthEventArgs> CellGrowthTriggered;
    }
}
