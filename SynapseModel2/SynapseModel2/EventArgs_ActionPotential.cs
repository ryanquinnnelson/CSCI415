using System;
namespace SynapseModel2
{
    public class EventArgs_ActionPotential : EventArgs
    {
        //fields
        private DateTime when;


        //constructors
        public EventArgs_ActionPotential(DateTime when)
        {
            this.when = when;
        }


        //properties
        public DateTime When
        {
            get
            {
                return this.when;
            }
            set
            {
                when = value;
            }
        }

    }
}
