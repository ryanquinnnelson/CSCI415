using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseModel3
{
    public class SecondaryMessenger
    {
        //fields
        private List<DateTime> events;
        private int frequencyTrigger;
        private DateTime start;
        private TimeSpan window;


        //constructors
        public SecondaryMessenger(DateTime start, int frequencyTrigger, TimeSpan window) //tested
        {
            this.frequencyTrigger = frequencyTrigger;
            this.window = window;
            this.start = start;
            this.events = new List<DateTime>();
        }


        //public methods
        public void AddEvent(DateTime dt) //needs fixing so it can work concurrently
        {
            events.Add(dt);
        }

        public bool IsGrowthStateTriggered(DateTime now) //temporary??
        { 
            return false;
        }

        public bool IsGrowthStateTriggered2(DateTime now) //needs fixing so it can be read from and written to concurrently??
        {
            //determine current observation window
            DateTime oldest = now - window;

            //process list to remove events that are older than current observation window
            List<DateTime> temp = new List<DateTime>();
            foreach (DateTime e in events)
            {
                if (e >= oldest)
                {
                    temp.Add(e); //this event occurred within the observation window
                }
            }

            events.Clear(); //remove old events from list
            events = temp; //store only events that occurred within the observation window


            //determine whether enough events have occurred in this window to trigger a growth event
            if (events.Count >= frequencyTrigger)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override string ToString() //tested
        {
            return "SecondaryMessenger{ frequencyTrigger=" + frequencyTrigger
                + ", window=" + window + ", start=" + start + ", events=" + OutputEventsList() + " }";
        }


        //private helper methods
        private string OutputEventsList() //tested
        {
            StringBuilder sb = new StringBuilder("[");
            foreach (DateTime dt in events)
            {
                sb.Append(dt);
                sb.Append(",");

            }
            if (events.Count > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("]");
            return sb.ToString();
        }


        ////tests
        //public static void Main(){
        //    Console.WriteLine("Test of Constructor 1");
        //    SecondaryMessenger m = new SecondaryMessenger(DateTime.Now, 1, new TimeSpan(0, 0, 5));
        //    Console.WriteLine(m);
        //    Console.WriteLine("Test of AddEvent()");
        //    m.AddEvent(DateTime.Now);
        //    m.AddEvent(new DateTime(2017, 12, 1));
        //    Console.WriteLine(m.OutputEventsList());
        //    Console.WriteLine("Test of IsGrowthStateTriggered()");
        //    Console.WriteLine(m.IsGrowthStateTriggered(DateTime.Now));
        //    Console.WriteLine(m.OutputEventsList());
        //}


    }
}
