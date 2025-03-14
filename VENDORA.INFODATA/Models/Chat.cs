using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VENDORA.INFODATA.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        public int? ThreadId { get; set; }
        [ForeignKey("ThreadId")]
        public ChatThread Thread { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        public int? ParentMessageId { get; set; }
        [ForeignKey("ParentMessageId")]
        public Chat ParentMessage { get; set; }

        [Required]
        public string Message { get; set; }

        [MaxLength(255)]
        public string FilePath { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}
