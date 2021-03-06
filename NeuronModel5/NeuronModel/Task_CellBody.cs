﻿//Ryan Nelson and Brian Engelbrecht

using System;
using System.Threading;

namespace NeuronModel
{
    public class Task_CellBody
    {
        //fields
        private CellBody body;
        private int id;
        private TimeSpan runLength;
        private DateTime start;


        //constructors
        public Task_CellBody(int id, TimeSpan runLength, CellBody body, DateTime start) //tested
        {
            Console.WriteLine("Task_CellBody " + id + " is created.");

            this.id = id;
            this.runLength = runLength;
            this.body = body;
            this.start = start;
        }


        //properties
        public CellBody Body //tested
        {
            get
            {
                return this.body;

            }
            private set
            {
                body = value;
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
        public void Consume() //tested
        {
            Console.WriteLine("Task_CellBody {0} is consuming...", Id);

            while (DateTime.Now - start < runLength)
            {
                int voltage = body.TryRemoveFromBuffer();
                //Console.WriteLine("Task_CellBody {0} removed {1} from buffer.", Id, voltage);
            }//end while

            Console.WriteLine("Task_CellBody {0} is done.", Id);
        }

        public void Decay() //tested
        {
            Console.WriteLine("Task_CellBody {0} is decaying...", Id);
            while (DateTime.Now - start < runLength)
            {
                Thread.Sleep(body.DecayFrequency); //simulate biology speed
                body.DecayMembranePotential();
                //Console.WriteLine("Task_CellBody {0} decayed cell body membrane potential to {1}.", Id, body.MembranePotential);
            }
            Console.WriteLine("Task_CellBody {0} is done.", Id);
        }

        public override String ToString() //tested
        {
            return "Task_CellBody{ id=" + Id + " }";
        }


        ////tests
        //public static void Main()
        //{
        //    Console.WriteLine("Test for Constructor 1");
        //    Task_CellBody tcb = new Task_CellBody(1, new TimeSpan(0, 0, 2), new CellBody(DateTime.Now, 100));
        //    Console.WriteLine(tcb);
        //    Console.WriteLine();

        //    Console.WriteLine("Test for Constructor 1");
        //    tcb.Decay();
        //    Console.WriteLine();

        //}
    }
}
