using System;
using System.ComponentModel.DataAnnotations;

namespace VENDORA.INFODATA.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [Required, MaxLength(50)]
        public string Role { get; set; } = "Client"; // Default role

        [Required]
        public string PasswordHash { get; set; }  // ⬅️ Hash password, bukan teks biasa!

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
