using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FORNILLOS_FinalProject
{
    public class Expense
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Notes { get; internal set; }
        public string Subscription { get; set; }
        public DateTime NextUpdateDate { get; set; }

    }
}
