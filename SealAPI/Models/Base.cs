using System.ComponentModel.DataAnnotations;

namespace SealAPI.Models
{
    public class Base
    {
        // ID
        public int Id { get; set; }

        // Created Timestamp
        [Required]
        public DateTime Cts { get; set; }
    }
}
