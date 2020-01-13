﻿using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using taohi_backend.Data;
using taohi_backend.Models;

namespace taohi_backend.Hubs
{
    public class MessageHub : Hub
    {
        private readonly AppDbContext _context;
        public MessageHub(AppDbContext context)
        {
            _context = context;
        }

        public async Task SendMessage(Message message)
        {
            //var senderID = ConnectionUserId(Context);
            //if (senderID != message.SenderId)
            //    return;

            var sender = await _context.Users.FindAsync(message.SenderId);
            var receiver = await _context.Users.FindAsync(message.ReceiverId);
            if (sender == null || !sender.IsActive ||
                receiver == null || !receiver.IsActive ||
                string.IsNullOrWhiteSpace(message.MessageContent))
                return;

            message = UpdateTimeStamp(message);
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            await Clients.All.SendAsync("ReceiveMessage", message.MessageId, sender.UserName, message.MessageContent);
        }

        public async Task DeleteMessage(Guid messageId)
        {
            var userID = ConnectionUserId(Context);
            var user = await _context.Users.FindAsync(userID);
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null || user == null || !user.IsActive) // || message.SenderId != userID
                return;

            _context.Messages.Remove(message);
            _context.SaveChanges();

            await Clients.All.SendAsync("DeleteSuccess", messageId);
        }

        private Guid ConnectionUserId(HubCallerContext context)
        {
            var userID = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Guid.Parse(userID);
        }

        private Message UpdateTimeStamp(Message message)
        {
            var currentTime = DateTime.Now;
            if (message.CreatedDateTime == null)
                message.CreatedDateTime = currentTime;
            else
                message.UpdatedDateTime = currentTime;
            return message;
        }
    }
}