using System;

namespace CyberPrism.Modules.Production.Models
{
    public class ProductionJob
    {
        public string JobId { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public int Completed { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public double Progress => Quantity > 0 ? (double)Completed / Quantity * 100 : 0;
    }
}

