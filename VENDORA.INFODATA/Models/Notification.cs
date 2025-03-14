using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VENDORA.INFODATA.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Client User { get; set; }

        public int? ChatId { get; set; }
        [ForeignKey("ChatId")]
        public Chat Chat { get; set; }

        [Required, MaxLength(50)]
        public string Type { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}