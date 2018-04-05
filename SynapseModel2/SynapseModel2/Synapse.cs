using System;
namespace SynapseModel2
{
    public class Synapse
    {
        //fields
        private Dendrite dendrite;
        private InputAxon axon;
        private int id;


        //constructors
        public Synapse(int id, Dendrite dendrite)
        {
            this.id = id;
            this.dendrite = dendrite;
            axon = null;
        }

        public Synapse(int id, Dendrite dendrite, InputAxon axon){
            this.id = id;
            this.dendrite = dendrite;
            this.axon = axon;
        }


        //properties
        public Dendrite Dendrite{
            get{
                return this.dendrite;
            }
            private set{
                dendrite = value;    
            }
        }

        public InputAxon Axon{
            get{
                return this.axon;
            }
            set{
                axon = value;
            }
        }

        public int Id{
            get{
                return this.id;
            }
            private set{
                id = value;
            }
        }


        //public methods
        public bool IsConnectionAlreadyFormed(){
            return !(axon == null);
        }

        public bool Connect(InputAxon axon){
            if(!IsConnectionAlreadyFormed()){
                this.axon = axon;
                return true;
            }
            return false;
        }

    }
}
