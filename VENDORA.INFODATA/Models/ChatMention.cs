using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VENDORA.INFODATA.Models
{
    public class ChatMention
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ChatId { get; set; }
        [ForeignKey("ChatId")]
        public Chat Chat { get; set; }

        [Required]
        public int MentionedUserId { get; set; }
        [ForeignKey("MentionedUserId")]
        public Client MentionedUser { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
