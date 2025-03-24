using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currency_Exchange.Service.Domain
{
    public class ExchangeRate
    {
        public int R030 { get; set; }
        public string Txt { get; set; }
        public decimal Rate { get; set; }
        public string Cc { get; set; }
        public string ExchangeDate { get; set; }
    }
}
