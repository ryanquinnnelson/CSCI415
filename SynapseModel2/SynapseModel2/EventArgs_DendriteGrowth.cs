using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SynapseModel2
{
    public class EventArgs_DendriteGrowth : EventArgs
    {
        //fields
        private DateTime when;
        private Dendrite dendrite;


        //constructor
        public EventArgs_DendriteGrowth(DateTime when, Dendrite dendrite)
        {
            this.when = when;

            this.dendrite = dendrite;
        }


        //properties
        public Dendrite Dendrite
        {
            get
            {
                return this.dendrite;
            }
            set
            {
                dendrite = value;
            }
        }

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
