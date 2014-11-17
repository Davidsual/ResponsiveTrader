using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ResponsiveTrader.Shared
{
    [DataContract]
    public class RateDto
    {
        [DataMember]
        public int RateValue { get; set; }
        [DataMember]
        public string RateName { get; set; }
        [DataMember]
        public DateTime RateDate { get; set; }

    }
}
