namespace SealAPI.Models
{
    public class Config
    {
        /// appsettings.json Section
        public const string Section = "Config";

        public int LinkExpiryMins { get; set; } = 60;
    }
}
