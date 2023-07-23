using SealAPI.Resources;
using System.ComponentModel.DataAnnotations;

namespace SealAPI.Models
{
    public class File : Base
    {
        [StringLength(maximumLength: 255)]
        public string Name { get; set; }
        
        public FileType Type { get; set; }

        public long Size { get; set; }

        [Range(0, int.MaxValue)]
        public int DownloadCount { get; set; } = 0;

        public byte[]? Data { get; set; }
    }
}
