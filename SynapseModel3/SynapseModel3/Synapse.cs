using System;
namespace SynapseModel3
{
    public class Synapse
    {
        //fields
        private InputAxon axon;
        private int id;


        //constructors
        public Synapse(int id) //tested
        {
            this.id = id;
            axon = null;
        }

        public Synapse(int id, InputAxon axon) //tested
        {
            this.id = id;
            this.axon = axon;
        }


        //properties
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
        public bool Connect(InputAxon axon) //tested
        {
            if (!IsConnectionAlreadyFormed())
            {
                this.axon = axon;
                return true;
            }
            return false;
        }

        public bool IsConnectionAlreadyFormed() //tested
        {
            return !(axon == null);
        }

        public override string ToString() //tested
        {
            String axonString = "NULL";
            if(axon != null){
                axonString = axon.ToString();
            }
            return "Synapse{ id=" + id + ", inputAxon=" + axonString + " }";
		}


		////tests
		//public static void Main()
        //{
        //    Console.WriteLine("Test of Constructor 1");
        //    Synapse s1 = new Synapse(1);
        //    Console.WriteLine(s1);

        //    Console.WriteLine("Test of Constructor 2");
        //    Synapse s2 = new Synapse(1, new InputAxon(2, 2000, 1));
        //    Console.WriteLine(s2);

        //    Console.WriteLine("Test of GetInputAxon()");
        //    Console.WriteLine(s2.Axon);

        //    Console.WriteLine("Test of GetId()");
        //    Console.WriteLine(s2.Id);

        //    Console.WriteLine("Test of IsConnectionAlreadyFormed()");
        //    Console.WriteLine(s1.IsConnectionAlreadyFormed());
        //    Console.WriteLine(s2.IsConnectionAlreadyFormed());

        //    Console.WriteLine("Test of Connect()");
        //    Console.WriteLine(s1.Connect(new InputAxon(1, 1, 1)));
        //    Console.WriteLine(s2.Connect(new InputAxon(2, 2, 2)));
        //}

    }
}
