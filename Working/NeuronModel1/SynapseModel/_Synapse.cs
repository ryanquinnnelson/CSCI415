using System;
namespace SynapseModel
{
    public class Synapse
    {
        private int dendriteId;
        private int inputId;

        public Synapse(int dendriteId)
        {
            this.dendriteId = dendriteId;
        }

        public int InputId{
            get{
                return inputId;
            }
            set{
                inputId = value;
            }
        }

    }
}
