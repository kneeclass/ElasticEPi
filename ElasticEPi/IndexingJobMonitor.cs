using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticEPi {
    
    public sealed class IndexingJobMonitor {
        private static readonly IndexingJobMonitor _instance = new IndexingJobMonitor();

        private IndexingJobMonitor() { }

        public static IndexingJobMonitor Instance {
            get {
                return _instance;
            }
        }

        public string CurrentStatus { get; set; }

    }

}
