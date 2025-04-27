using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertJournalCSV
{
    internal class Trade
    {
        public TimeOnly Time { get; set; }
        public string? TimeString { get; set; }
        public string? Symbol { get; set; }
        public string? BuySell { get; set; }
        public string? Price { get; set; }
        public string? Quantity { get; set; }
        public string? Account { get; set; }
    }


    internal class TradeSide
    {
        public string? Symbol { get; set; }
        public Side Side { get; set; }
        public int Quanity { get; set; }

    }

    internal enum Side {
        Short,
        Long,
    }
}
