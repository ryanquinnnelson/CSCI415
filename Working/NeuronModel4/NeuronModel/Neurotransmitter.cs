using System;
namespace NeuronModel
{
    public class Neurotransmitter
    {
        //fields
        int charge;


        //constructors
        public Neurotransmitter(int charge)//tested
        {
            this.charge = charge;
        }


        //properties
        public int Charge//tested
        {
            get
            {
                return this.charge;
            }
            private set
            {
                charge = value;
            }
        }


		//public methods
		public override string ToString() //tested
		{
            return "Neurotransmitter{ charge=" + charge + " }";
		}


        ////tests
        //public static void Main(){
        //    Console.WriteLine("Test of Constructor 1");
        //    Neurotransmitter n = new Neurotransmitter(-10);
        //    Console.WriteLine(n);
        //    Console.WriteLine("Test of GetCharge()");
        //    Console.WriteLine(n.Charge);
        //}
	}
}
