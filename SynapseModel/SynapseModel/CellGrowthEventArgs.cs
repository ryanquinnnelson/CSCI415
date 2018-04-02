using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace SynapseModel
{
    public class CellGrowthEventArgs : EventArgs
    {
        public Neuron neuron { get; set;  }
        public DendriteType type { get; set; }
        public TimeSpan timespan { get; set; }
        public List<Task> tasks { get; set; }
    }
}
