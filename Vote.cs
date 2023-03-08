using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotBigBrother
{
  internal class Vote
  {
    public int Id { get; set; }
    public string VoteValue { get; set; }
    public long VoterId { get; set; }
    public int MessageId { get; set; }
  }
}
