using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartTask.Core.Models.Notification;
using SmartTask.DataAccess.Data;
using SmartTask.Core.ViewModels;
//using SignalRProject.Models;
//using SignalRProject.ViewModels;
using System.Security.Claims;

namespace SignalRProject.Service.Hubs
{
    
    public class NotificationHub : Hub
    {
        SmartTaskContext db;
        public NotificationHub(SmartTaskContext _db)
        {
            db = _db;
        }

        public void sendNotification(Notification notification)
        {
            //db.Notifications.Add(notification);
            //db.SaveChanges();

            Clients.All.SendAsync("newnotifcation", notification);
        }

        public void sendToGroup(NotificationVM notification,string groupName)
        {
            Clients.Group(groupName).SendAsync("groupTask", notification);
        }

        public override async Task<Task> OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            db.UserConnections.Add(new UserConnection
            {
                UserID = userId,
                connectionID = Context.ConnectionId
            });

            db.SaveChanges();

            // Get all group names this user belongs to
            var groupNames =  db.UserGroups
                                .Include(ug => ug.groups)
                                .Where(ug => ug.UserID == userId)
                                .Select( ug => ug.groups.Name)
                                .ToList();

            // Add connection to all groups
            foreach (var groupName in groupNames)
            {
                 Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            }

            //if (string.IsNullOrEmpty(userId))
            //    return;

            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connection = await db.UserConnections
                .FirstOrDefaultAsync(uc => uc.connectionID == Context.ConnectionId);

            if (connection != null)
            {
                var groupNames = await db.UserGroups
                    .Include(ug => ug.groups)
                    .Where(ug => ug.UserID == connection.UserID)
                    .Select(ug => ug.groups.Name)
                    .ToListAsync();

                // Remove from SignalR groups
                foreach (var groupName in groupNames)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                }

                // Remove the connection from DB
                db.UserConnections.Remove(connection);
                await db.SaveChangesAsync();
            }
        }

        #region comment
        //public override async Task OnConnectedAsync()
        //{
        //    var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(userId))
        //        return;

        //    // Save connection ID to UserConnection
        //    db.UserConnections.Add(new UserConnection
        //    {
        //        UserID = userId,
        //        connectionID = Context.ConnectionId
        //    });


        //    await db.SaveChangesAsync();

        //    // Get all group names this user belongs to
        //    var groupNames = await db.UserGroups
        //        .Include(ug => ug.groups)
        //        .Where(ug => ug.UserID == userId)
        //        .Select(ug => ug.groups.Name)
        //        .ToListAsync();

        //    // Add connection to all groups
        //    foreach (var groupName in groupNames)
        //    {
        //        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //    }

        //    await base.OnConnectedAsync();
        //}

        //public override async Task OnDisconnectedAsync(Exception exception)
        //{
        //    var connection = await db.UserConnections
        //        .FirstOrDefaultAsync(uc => uc.connectionID == Context.ConnectionId);

        //    if (connection != null)
        //    {
        //        var groupNames = await db.UserGroups
        //            .Include(ug => ug.groups)
        //            .Where(ug => ug.UserID == connection.UserID)
        //            .Select(ug => ug.groups.Name)
        //            .ToListAsync();

        //        // Remove from SignalR groups
        //        foreach (var groupName in groupNames)
        //        {
        //            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        //        }

        //        // Remove the connection from DB
        //        db.UserConnections.Remove(connection);
        //        await db.SaveChangesAsync();
        //    }

        //    await base.OnDisconnectedAsync(exception);
        //}
        #endregion
    }
}
