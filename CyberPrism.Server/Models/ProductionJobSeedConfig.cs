namespace CyberPrism.Server.Models
{
    /// <summary>
    /// Configuration model for initial production job seed data
    /// </summary>
    public class ProductionJobSeedConfig
    {
        public int Id { get; set; }
        public string JobId { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Completed { get; set; }
        public string Status { get; set; } = string.Empty;
        public int DueDaysOffset { get; set; }
        public int CreatedDaysOffset { get; set; } = 0;
    }
}

