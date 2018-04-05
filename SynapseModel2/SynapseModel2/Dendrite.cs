﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace SynapseModel2
{
    public class Dendrite
    {
        //constants
        public const int RESTING_POTENTIAL = -70000; //Volts
        public const int RESTORE_INCREMENT = 500;


        //fields
        private int state; //0 no growth; 1 growth
        private int id;
        private int type; //0 is proximal; 1 is basal; 2 is apical
        private int membranePotential; //Volts
        private BlockingCollection<Neurotransmitter> buffer; //shared
        private SecondaryMessenger messenger;
        private int numAvailableSynapses;
        //private ConcurrentDictionary<int, Synapse> synapses; //shared
        private List<Synapse> synapses;
        private int nextSynapseId;





		//constructors
		public Dendrite(int id, int type, int numAvailableSynapses)
        {
            this.id = id;
            this.type = type;
            this.numAvailableSynapses = numAvailableSynapses;
            state = 0;
            membranePotential = RESTING_POTENTIAL;
            buffer = new BlockingCollection<Neurotransmitter>(new ConcurrentQueue<Neurotransmitter>());

            //look at the previous 2 seconds
            int days = 0;
            int hours = 0;
            int minutes = 0;
            int seconds = 2;
            int milliseconds = 0;
            TimeSpan window = new TimeSpan(days, hours, minutes, seconds, milliseconds);

            messenger = new SecondaryMessenger(DateTime.Now, 100, window);
            //synapses = new ConcurrentDictionary<int, Synapse>();
            synapses = new List<Synapse>();
            nextSynapseId = 0;

            AddOrUpdateSynapses(numAvailableSynapses);
        }


        //properties
        public int NumAvailableSynapses{
            get{
                return numAvailableSynapses;
            }
            private set{
                numAvailableSynapses = value;
            }
        }

        public int Type{
            get{
                return type;
            }
            private set{
                type = value;
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

        public int State{
            get{
                return this.state;
            }
            private set{
                state = value;
            }
        }


        //public methods
        public void AddToBuffer(Neurotransmitter nt)
        {
            buffer.Add(nt);
            messenger.AddEvent(DateTime.Now);

            //check whether dendrite growth state threshold reached
            if (state == 0 && messenger.IsGrowthStateTriggered(DateTime.Now))
            {
                SetGrowthState();
            }
        }

        public void DecayMembranePotential()
        {
            if (membranePotential < RESTING_POTENTIAL)
            {
                Interlocked.Add(ref membranePotential, RESTORE_INCREMENT);
            }
            else if (membranePotential > RESTING_POTENTIAL)
            {
                Interlocked.Add(ref membranePotential, -RESTORE_INCREMENT);
            }
            else
            {
                //do nothing
            }
        }

        public int TryRemoveFromBuffer()
        {
            Neurotransmitter removed = buffer.Take();
            int charge = removed.Charge;

            //affect local membrane potential
            Interlocked.Add(ref membranePotential, charge);

            //Console.WriteLine("Dendrite {0} Membrane Potential is \t\t\t{1}", Id, membranePotential);
            return charge;
        }

        public Synapse GetOpenSynapse(){
            Synapse open = null;

            //foreach(int key in synapses.Keys){
            //    Synapse current;
            //    synapses.TryGetValue(key, out current);

            //    if(!current.IsConnectionAlreadyFormed()){
            //        open = current;
            //        break; //stop searching
            //    }
            //}

            return open;
        }

        public override string ToString()
        {
            return "Dendrite\n{ \n\tid=" + id + ", \n\ttype=" + type + ", \n\tstate="
                + state + ", \n\tmembranePotential=" + membranePotential +
                ", \n\tbuffer=" + buffer + ", \n\tmessenger=" + messenger
                + ", \n\tnumAvailableSynapses=" + numAvailableSynapses
                + ", \n\tnextSynapseId=" + nextSynapseId
                + ", \n\tsynapses=" + OutputSynapses() + " \n}";
        }






        //private helper methods
        private void AddOrUpdateSynapses(int number)
        {
            for (int i = 0; i < number; i++)
            {
                Synapse newest = new Synapse(nextSynapseId++, this);
                synapses.Add(newest);
            }
        }

        private Synapse GetSynapse(int targetId)
        {
            Synapse target = null;

            foreach(Synapse s in synapses){
                if(s.Id == targetId){
                    target = s;
                    break;
                }
            }

            return target;
        }

        private void SetGrowthState(){
            int original = Interlocked.CompareExchange(ref state, 1, 0);
            if (original == 0)
            {
                RaiseDendriteGrowthState(DateTime.Now); //raise event to alert ProcessManager
                AddOrUpdateSynapses(1);
                SetNoGrowthState();
            }
        }

        private void SetNoGrowthState(){
            Interlocked.CompareExchange(ref state, 0, 1);
        }

        private string OutputSynapses()
        {
            StringBuilder sb = new StringBuilder("[");
            sb.Append(synapses[0].ToString());
            //foreach (Synapse s in synapses)
            //{
            //    sb.Append(s);
            //    sb.Append(" ,");
            //}
            //if (synapses.Count > 0)
            //{
            //    sb.Remove(sb.Length - 1, 1);
            //}

            sb.Append("]");
            return sb.ToString();
        }




        //??need ProcessManager to listen for this
        //DendriteGrowthEvent code
        private void RaiseDendriteGrowthState(DateTime when){
            Console.WriteLine("Dendrite Growth State event raised.");

            EventArgs_DendriteGrowth args = new EventArgs_DendriteGrowth(when, this);

            OnDendriteGrowthEvent(args);
        }

        protected virtual void OnDendriteGrowthEvent(EventArgs_DendriteGrowth e){
            EventHandler<EventArgs_DendriteGrowth> handler = DendriteGrowthEvent;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler<EventArgs_DendriteGrowth> DendriteGrowthEvent;


        ////tests
        //public static void Main(){
        //    Console.WriteLine("Test of Constructor 1");
        //    Dendrite d = new Dendrite(1, 1, 5);
        //    Console.WriteLine(d);
        //    Console.WriteLine();
        //}
    }
}
