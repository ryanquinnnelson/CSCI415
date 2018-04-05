using System;
namespace SynapseModel3
{
    public class InputAxon
    {
        //fields
        private int id;
        private int productionFrequency;
        private int inputType; //0 feedforward; 1 context; 2 feedback


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


        //public methods
        public override string ToString() //tested
        {
            return "InputAxon{ id=" + id + ", productionFrequency=" + productionFrequency + ", inputType=" + inputType + " }";
        }


        //tests
        //public static void Main(){
        //    Console.WriteLine("Test of Constructor 1");
        //    InputAxon i = new InputAxon(1, 100, 0);
        //    Console.WriteLine(i);
        //    Console.WriteLine("Test of GetId()");
        //    Console.WriteLine(i.Id);
        //    Console.WriteLine("Test of GetProductionFrequency()");
        //    Console.WriteLine(i.ProductionFrequency);
        //    Console.WriteLine("Test of InputType()");
        //    Console.WriteLine(i.InputType);
        //}
    }
}
