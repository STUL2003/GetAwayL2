using System.ComponentModel;
namespace GetAwayL2.Models
{
    //public enum string
    //{
    //    [Description("Camera")]
    //    Camera = 1,
    //    [Description("PLK")]
    //    PLK = 2
    //}
    public class EquipLogs
    {
        

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime timeSend { get; set; }
        
        public string rawData { get; set; }
        public string status { get; set; }
        public string? errorMessage { get; set; } = null;
    }
}
