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
        public Synapse(int id, Dendrite dendrite) //tested
        {
            this.id = id;
            this.dendrite = dendrite;
            axon = null;
        }

        public Synapse(int id, Dendrite dendrite, InputAxon axon) //tested
        {
            this.id = id;
            this.dendrite = dendrite;
            this.axon = axon;
        }


        //properties
        public Dendrite Dendrite //tested
        {
            get
            {
                return this.dendrite;
            }
            private set
            {
                dendrite = value;
            }
        }

        public InputAxon Axon //tested
        {
            get
            {
                return this.axon;
            }
            set
            {
                axon = value;
            }
        }

        public int Id //tested
        {
            get
            {
                return this.id;
            }
            private set
            {
                id = value;
            }
        }


        //public methods
        public bool IsConnectionAlreadyFormed() //tested
        {
            return !(axon == null);
        }

        public bool Connect(InputAxon axon) //tested
        {
            if (!IsConnectionAlreadyFormed())
            {
                this.axon = axon;
                return true;
            }
            return false;
        }

        public override string ToString() //tested
        {
            String axonString = "NULL";
            if(axon != null){
                axonString = axon.ToString();
            }
            return "Synapse{ id=" + id + ", dendrite=" + dendrite + ", inputAxon=" + axonString + " }";
		}


		//tests
		public static void Main()
        {
            Console.WriteLine("Test of Constructor 1");
            Synapse s1 = new Synapse(1, new Dendrite(1, 0, 100));
            Console.WriteLine(s1);

            //Console.WriteLine("Test of Constructor 2");
            //Synapse s2 = new Synapse(1, new Dendrite(1, 0, 100), new InputAxon(2, 2000, 1));
            //Console.WriteLine(s2);

            //Console.WriteLine("Test of GetDendrite()");
            //Console.WriteLine(s2.Dendrite);

            //Console.WriteLine("Test of GetInputAxon()");
            //Console.WriteLine(s2.Axon);

            //Console.WriteLine("Test of GetId()");
            //Console.WriteLine(s2.Id);

            //Console.WriteLine("Test of IsConnectionAlreadyFormed()");
            //Console.WriteLine(s1.IsConnectionAlreadyFormed());
            //Console.WriteLine(s2.IsConnectionAlreadyFormed());

            //Console.WriteLine("Test of Connect()");
            //Console.WriteLine(s1.Connect(new InputAxon(1, 1, 1)));
            //Console.WriteLine(s2.Connect(new InputAxon(2, 2, 2)));
        }

    }
}
