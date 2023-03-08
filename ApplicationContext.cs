using Microsoft.EntityFrameworkCore;

namespace TelegramBotBigBrother
{
  internal class ApplicationContext : DbContext
  {
    public DbSet<User> Users { get; set; }
    public DbSet<Vote> Messages { get; set; }
    public ApplicationContext()
    {
      Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlite("Data Source=BigBrotherBot.db");
    }
  }
}
