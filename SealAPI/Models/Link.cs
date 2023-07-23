using System.ComponentModel.DataAnnotations;

namespace SealAPI.Models
{
    public class Link : Base
    {
        [StringLength(maximumLength: 2000)]
        public string? GUID { get; set; }

        public int FileId { get; set; }

        public int ExpiryMins { get; set; }
    }
}
