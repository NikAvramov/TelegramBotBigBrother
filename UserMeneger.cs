namespace TelegramBotBigBrother
{
  internal class UserMeneger
  {
    public void AddToDB(User user)
    {
      using (ApplicationContext db = new())
      {
        if (!db.Users.Any(u => u.TelegramId == user.TelegramId))
        {
          db.Users.Add(user);
          db.SaveChanges();
        }
      }
    }
    public void AddAndUpdate(User candidat, User voter, int messageId, string grade)
    {
      using (ApplicationContext db = new())
      {
        if (!db.Users.Any(u => u.TelegramId == candidat.TelegramId))
        {
          db.Users.Add(candidat);
          db.SaveChanges();
        }
        if (!db.Users.Any(u => u.TelegramId == voter.TelegramId))
        {
          db.Users.Add(voter);
          db.SaveChanges();
        }
        var allVotesForThisMessage = db.Messages.Where(v => v.MessageId == messageId).ToList();
        if (!allVotesForThisMessage.Any(v => v.VoterId == voter.TelegramId))
        {
          var vote = new Vote() { VoterId = voter.TelegramId, MessageId = messageId, VoteValue = grade };
          db.Messages.Add(vote);
          var targetUser = db.Users.FirstOrDefault(u => u.TelegramId == candidat.TelegramId);
          targetUser.UserName = candidat.UserName;
          targetUser.FirstName = candidat.FirstName;
          targetUser.LastName = candidat.LastName;
          targetUser.UserName = candidat.UserName;
          if (grade == "+")
          { targetUser.Like += 1; }
          else
          { targetUser.DisLike += 1; }
          db.SaveChanges();
        }
      }
    }
    public List<User> GetAllUsersFromDB()
    {
      using (ApplicationContext db = new())
      {
        return db.Users.ToList();
      }
    }
    public void DeleteFromDB(User user)
    {
      using (ApplicationContext db = new())
      {
        var targetUser = db.Users.FirstOrDefault(u => u.TelegramId == user.TelegramId);
        if (targetUser != null)
        {
          db.Users.Remove(targetUser);
          db.SaveChanges();
        }
      }
    }
  }
}
