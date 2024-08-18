using ChatHub.Data;
using Microsoft.AspNetCore.SignalR;

namespace ChatHub.Models;

public class Chat : Hub {
    private readonly MessageDataBaseContext _context;

    public Chat(MessageDataBaseContext context) {
        _context = context;
    }

    public async Task SendMessage(string receiverId, string content) {
        var senderId = Context.UserIdentifier;

        var message = new Message {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            Time = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        await Clients.User(receiverId).SendAsync("Receive Message", senderId, content, message.Time);
    }
}