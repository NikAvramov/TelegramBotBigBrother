using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBotBigBrother
{
  internal class User
  {
    public int ID { get; set; }
    [Column("TelegramID")]
    public long TelegramId { get; set; }
    [Column("FirstName")]
    public string FirstName { get; set; }
    [Column("LastName")]
    public string? LastName { get; set; }
    [Column("UserName")]
    public string? UserName { get; set; }
    [Column("Like")]
    public int Like { get; set; }
    [Column("DisLike")]
    public int DisLike { get; set; }
  }
}
