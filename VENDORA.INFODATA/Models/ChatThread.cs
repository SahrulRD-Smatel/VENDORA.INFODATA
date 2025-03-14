using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace VENDORA.INFODATA.Models
{
    public class ChatThread
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Title { get; set; }

        [Required]
        public int CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public Client Client { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
