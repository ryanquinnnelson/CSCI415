using System;
using System.Threading;
using System.Collections.Generic;

namespace SynapseModel
{
    public class SecondaryMessenger
    {
        
        private int frequencyTrigger;
        private TimeSpan window;
        private DateTime start;
        private List<DateTime> events;

        public SecondaryMessenger(DateTime start, int frequencyTrigger, TimeSpan window)
        {
            this.frequencyTrigger = frequencyTrigger;
            this.window = window;
            this.start = start;
            this.events = new List<DateTime>();
        }

        public void AddEvent(DateTime dt){

            //add event to list
            events.Add(dt);
        }

        public bool GrowthStateAchieved(DateTime now){

            //determine current observation window
            DateTime oldest = now - window;

            List<DateTime> temp = new List<DateTime>();
            //process list to remove events that are older than current observation window
            foreach(DateTime e in events){
                if(e >= oldest){
                    temp.Add(e); //this event occurred within the observation window
                }
            }

            events.Clear(); //remove old events from list
            events = temp; //store only events that occurred within the observation window


            //determine whether enough events have occurred in this window to trigger a growth event
            int numEvents = events.Count;

            if(numEvents >= frequencyTrigger){
                return true;
            }
            return false;
        }
    }
}
