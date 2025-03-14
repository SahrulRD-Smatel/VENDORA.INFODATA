using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VENDORA.INFODATA.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        [Required]
        public int ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string FilePath { get; set; } // Untuk dokumen terkait request

        [Required, MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Default status

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔹 Relasi ke ServiceStatus (History Perubahan Status)
        public List<ServiceStatus> ServiceStatuses { get; set; }
    }
}
