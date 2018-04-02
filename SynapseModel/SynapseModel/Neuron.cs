using System;
using System.Collections.Generic;


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

        public Neuron(DateTime start)
        {
            state = CellGrowthState.NoGrowth;
            body = new CellBody(start);
            dendrites = new List<Dendrite>();
            axons = new List<Axon>();
            this.start = start;

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
    }
}
