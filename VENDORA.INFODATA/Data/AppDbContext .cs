using Microsoft.EntityFrameworkCore;
using VENDORA.INFODATA.Models;

namespace VENDORA.INFODATA.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSet untuk setiap tabel dalam database
        public DbSet<Client> Clients { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<ServiceStatus> ServiceStatuses { get; set; }
        public DbSet<ChatThread> ChatThreads { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMention> ChatMentions { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        
    }
}
