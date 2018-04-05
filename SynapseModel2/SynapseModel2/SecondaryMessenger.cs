using System;
using System.Collections.Generic;

namespace SynapseModel2
{
    public class SecondaryMessenger
    {
        //fields
        private int frequencyTrigger;
        private TimeSpan window;
        private DateTime start;
        private List<DateTime> events;


        //constructors
        public SecondaryMessenger(DateTime start, int frequencyTrigger, TimeSpan window)
        {
            this.frequencyTrigger = frequencyTrigger;
            this.window = window;
            this.start = start;
            this.events = new List<DateTime>();
        }


        //public methods
        public void AddEvent(DateTime dt)
        {
            events.Add(dt);
        }

        public bool IsGrowthStateTriggered(DateTime now)
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
    }


}
