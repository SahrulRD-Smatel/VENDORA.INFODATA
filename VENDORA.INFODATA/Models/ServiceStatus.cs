using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VENDORA.INFODATA.Models
{
    public class ServiceStatus
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RequestId { get; set; }
        [ForeignKey("RequestId")]
        public Request Request { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
