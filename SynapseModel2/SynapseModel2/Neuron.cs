using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SynapseModel2
{
    public class Neuron
    {
        private int nextDendriteId;
        private int state; //0 for no growth; 1 for growth
        private CellBody body;
        private List<Dendrite> dendrites;
        private SecondaryMessenger messenger;

        public Neuron()
        {
            nextDendriteId = 0;
            state = 0;
            body = new CellBody(DateTime.Now);
            dendrites = new List<Dendrite>();

            //look at the previous 2 seconds
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 2;
            int milliseconds = 0;
            TimeSpan window = new TimeSpan(days, hours, minutes, seconds, milliseconds);

            messenger = new SecondaryMessenger(DateTime.Now, 100, window);

            AddDendrites(1, new int[] {0}, 1);
        }


        //properties
        public int State
        {
            get
            {
                return this.state;
            }
            private set
            {
                state = value;
            }
        }

        public CellBody Body{
            get{
                return this.body;
            }
            private set{
                body = value;
            }
        }

        public List<Dendrite> Dendrites{
            get{
                return this.dendrites;
            }
            private set{
                dendrites = value;
            }
        }


        //public methods
        public Synapse SearchForOpenSynapse(int dendriteType){
            Synapse openSynapse = null;

            foreach(Dendrite d in dendrites){
                if(d.NumAvailableSynapses > 0){
                    openSynapse = d.GetOpenSynapse();
                }
            }

            return openSynapse;
        }


        //private helper methods
        private void AddDendrites(int num, int[] typeList, int numAvailableSynapses){
            if(typeList.Length != num){
                throw new ArgumentException("Number of dendrites to be created doesn't match number in type list.");
            }

            for (int i = 0; i < num; i++){
                Dendrite newest = new Dendrite(nextDendriteId, typeList[i], numAvailableSynapses);
            }
        }


        //ActionPotentialEvent code
        public void ReceiveActionPotentialEvent(object sender, EventArgs_ActionPotential e)
        {
            Console.WriteLine("received action potential event");

            //add to secondary messenger for checking
            messenger.AddEvent(DateTime.Now);
            if (messenger.IsGrowthStateTriggered(DateTime.Now))
            {
                state = 1;
                SignalCellGrowthEvent(); //add new dendrite(s)
            }
        }


        //CellGrowthEvent code
        public void SignalCellGrowthEvent()
        {
            Console.WriteLine("Cell growth event raised.");
            EventArgs_CellGrowth args = new EventArgs_CellGrowth(DateTime.Now, this);
            OnCellGrowthTriggered(args);
        }

        protected virtual void OnCellGrowthTriggered(EventArgs_CellGrowth e)
        {
            EventHandler<EventArgs_CellGrowth> handler = CellGrowthEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<EventArgs_CellGrowth> CellGrowthEvent;
    }
}
