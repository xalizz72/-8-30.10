using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ПР8
{
    public class CarConfig
    {
        public string Model { get; set; } = "Базовая";
        public double BasePrice { get; set; } = 1000000;

        public string Engine { get; set; } = "Бензин";
        public double EnginePrice { get; set; } = 0;

        public string Color { get; set; } = "Белый";
        public double ColorPrice { get; set; } = 0;

        public bool HasClimate { get; set; } = false;
        public bool HasAudio { get; set; } = false;

        public double InitialPaymentPercent { get; set; } = 20;
        public int LoanTerm { get; set; } = 12;

        public double GetTotalCarPrice()
        {
            double total = BasePrice + EnginePrice + ColorPrice;
            if (HasClimate) total += 50000;
            if (HasAudio) total += 30000;
            return total;
        }
    }
}