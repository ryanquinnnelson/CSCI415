using System;
namespace NeuronModel
{
    public class InputAxon
    {
        //fields
        private int id;
        private int inputType; //0 feedforward; 1 context; 2 feedback
        private int productionFrequency;


        //constructors
        public InputAxon(int id, int productionFrequency, int inputType) //tested
        {
            this.id = id;
            this.productionFrequency = productionFrequency;
            this.inputType = inputType;
        }


        //properties
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

        public int InputType //tested
        {
            get
            {
                return this.inputType;
            }
            private set
            {
                inputType = value;
            }
        }

        public int ProductionFrequency //tested
        {
            get
            {
                return this.productionFrequency;
            }
            private set
            {
                productionFrequency = value;
            }
        }


        //public methods
        public override string ToString() //tested
        {
            return "InputAxon{ id=" + id + ", productionFrequency=" + productionFrequency + ", inputType=" + inputType + " }";
        }

        public override bool Equals(object obj) //tested
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            InputAxon a = (InputAxon)obj;
            return (this.id == a.id) && (this.inputType == a.inputType) && (this.productionFrequency == a.productionFrequency);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        //tests
        //public static void Main()
        //{
        //    Console.WriteLine("Test of Constructor 1");
        //    InputAxon i = new InputAxon(1, 100, 0);
        //    Console.WriteLine(i);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetId()");
        //    Console.WriteLine(i.Id);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of GetProductionFrequency()");
        //    Console.WriteLine(i.ProductionFrequency);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of InputType()");
        //    Console.WriteLine(i.InputType);
        //    Console.WriteLine();

        //    Console.WriteLine("Test of Equals()");
        //    InputAxon j = new InputAxon(1, 100, 0);
        //    Console.WriteLine(i.Equals(j));
        //    Console.WriteLine();
        //}
    }
}
