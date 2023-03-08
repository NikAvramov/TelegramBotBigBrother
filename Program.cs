using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotBigBrother
{
  internal class Program
  {
    private static readonly string token = "****************************";
    private static readonly string like = "+";
    private static readonly string dislike = "-";
    static async Task Main()
    {
      var client = new TelegramBotClient(token);
      using CancellationTokenSource cts = new();
      ReceiverOptions receiverOptions = new() { AllowedUpdates = Array.Empty<UpdateType>() };
      client.StartReceiving(
        updateHandler: HandleUpdateAsync,
        pollingErrorHandler: HandlePollingErrorAsync,
        receiverOptions: receiverOptions,
        cancellationToken: cts.Token
      );
      var me = await client.GetMeAsync();
      Console.WriteLine($"Начинаем слушать @{me.Username}");
      Console.ReadLine();
      cts.Cancel();

      async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
      {
        if (update.Message is not { } message)
          return;
        var chatId = message.Chat.Id;
        var messageId = message.MessageId;
        if (message.ReplyToMessage != null && (message.Text == like || message.Text == dislike))
        {
          var parentMessageId = message.ReplyToMessage.MessageId;
          var parentMessageAutorID = message.ReplyToMessage.From.Id;
          var parentMessageAutorFirstName = message.ReplyToMessage.From.FirstName;
          var parentMessageAutorLastName = message.ReplyToMessage.From.LastName;
          var parentMessageAutorUserName = message.ReplyToMessage.From.Username;
          var candidat = new User()
          {
            TelegramId = parentMessageAutorID,
            FirstName = parentMessageAutorFirstName,
            LastName = parentMessageAutorLastName,
            UserName = parentMessageAutorUserName
          };
          var messageAutorID = message.From.Id;
          var messageAutorFirstName = message.From.FirstName;
          var messageAutorLastName = message.From.LastName;
          var messageAutorUserName = message.From.Username;
          var voter = new User()
          {
            TelegramId = messageAutorID,
            FirstName = messageAutorFirstName,
            LastName = messageAutorLastName,
            UserName = messageAutorUserName
          };
          var um = new UserMeneger();
          um.AddAndUpdate(candidat, voter, parentMessageId, message.Text);
          botClient.MakeRequestAsync(new DeleteMessageRequest(chatId, messageId));
        }
        else if (message.Text != null)
        {
          if (message.Text.Contains("привет", StringComparison.OrdinalIgnoreCase))
          {
            Message sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: $"И тебе не хворать {message.From.FirstName} {message.From.LastName}",
            replyToMessageId: messageId,
            cancellationToken: cancellationToken);
          }
          if (message.Text == "rating")
          {
            var um = new UserMeneger();
            var users = um.GetAllUsersFromDB();
            var top10 = users.Where(u => u.Like > 0).Take(10).OrderBy(u => u.Like).Reverse();
            var topOut10 = users.Where(u => u.DisLike > 0).Take(10).OrderBy(u => u.DisLike).Reverse();


            string result = "ВЫСОКИЙ РЕЙТИНГ\n\n";
            foreach (var user in top10)
            {
              int count = user.LastName != null ? user.LastName.Length : 0;
              result += $"`{user.FirstName} {user.LastName}" +
                        $"{new string(' ', 15 - (user.FirstName.Length + count))}" +
                        $" {user.Like}👍{user.DisLike}👎`" + "\n";
            }
            result += "\nНИЗКИЙ РЕЙТИНГ\n\n";
            foreach (var user in topOut10)
            {
              int count = user.LastName != null ? user.LastName.Length : 0;
              result += $"`{user.FirstName} {user.LastName}" +
                        $"{new string(' ', 15 - (user.FirstName.Length + count))}" +
                        $" {user.DisLike}👎{user.Like}👍`" + "\n";
            }
            Message sentMessage = await botClient.SendTextMessageAsync(
              chatId: chatId,
              text: result,
              parseMode: ParseMode.MarkdownV2,
              cancellationToken: cancellationToken);
          }
        }
      }
      Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
      {
        var ErrorMessage = exception switch
        {
          ApiRequestException apiRequestException
              => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
          _ => exception.ToString()
        };
        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
      }
    }
  }
}