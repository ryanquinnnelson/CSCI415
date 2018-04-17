using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace SynapseModel3
{
    public class SecondaryMessenger
    {
        //fields
        private ConcurrentBag<DateTime> events;
        private int frequencyTrigger;
        private DateTime start;
        private TimeSpan window;


        //constructors
        public SecondaryMessenger(DateTime start, int frequencyTrigger, TimeSpan window) //tested
        {
            this.frequencyTrigger = frequencyTrigger;
            this.window = window;
            this.start = start;
            this.events = new ConcurrentBag<DateTime>();
        }


        //public methods
        public void AddEvent(DateTime dt)
        {
            events.Add(dt);
        }

        public bool IsGrowthStateTriggered2(DateTime now) //use to turn off secondary messenger
        { 
            return false;
        }

        //may need locking
        public bool IsGrowthStateTriggered(DateTime now) //tested
        {
            bool triggered = false;

            //determine current observation window
            DateTime oldest = now - window;

            //process bag to remove events that are older than current observation window
            DateTime[] eventsArray = events.ToArray(); //copy current events slice
            ConcurrentBag<DateTime> eventsToKeep = new ConcurrentBag<DateTime>();

            foreach (DateTime e in eventsArray)
            {
                if (e >= oldest)
                {
                    eventsToKeep.Add(e); //this event occurred within the observation window
                }
            }

            //determine whether enough events have occurred in this window to trigger a growth event
            if (eventsToKeep.Count >= frequencyTrigger)
            {
                triggered = true;
            }

            //store only the events to keep in the "events" collection
            events = eventsToKeep; //??atomic

            return triggered;
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
            foreach (DateTime dt in events.ToArray())
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
        //    Console.WriteLine();

        //    Console.WriteLine("Test of AddEvent()");
        //    m.AddEvent(DateTime.Now);
        //    m.AddEvent(new DateTime(2017, 12, 1));
        //    Console.WriteLine(m.OutputEventsList());
        //    Console.WriteLine();

        //    Console.WriteLine("Test of IsGrowthStateTriggered()");
        //    Console.WriteLine(m.IsGrowthStateTriggered2(DateTime.Now));
        //    Console.WriteLine(m.OutputEventsList());
        //    Console.WriteLine();
        //}


    }
}
