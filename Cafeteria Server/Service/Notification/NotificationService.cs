﻿using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.UnitofWork;
using System;

namespace CafeteriaServer.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NotificationResponse>> GetUnreadNotifications(int userId)
        {
            var notifications = await _unitOfWork.UserNotifications.FindAll(n => n.UserId == userId && !n.IsRead);
            var notificationResponses = new List<NotificationResponse>();
            var notificationIds = new List<int>();

            foreach (var notification in notifications)
            {
                var notificationType = await _unitOfWork.NotificationTypes.GetById(notification.NotificationTypeId);
                string message = notificationType?.NotificationMessage ?? "Unknown notification";

                if (notification.NotificationTypeId == 1 || notification.NotificationTypeId == 2 || notification.NotificationTypeId == 4) 
                {
                    if (notification.MenuItemId.HasValue) 
                    {
                        var menuItem = await _unitOfWork.MenuItems.GetById(notification.MenuItemId.Value);
                        message = $"{message} : '{menuItem.ItemName}'";
                    }
                }

                notificationResponses.Add(new NotificationResponse
                {
                    NotificationId = notification.UserNotificationId,
                    Message = message,
                    CreatedAt = notification.CreatedAt,
                    IsRead = notification.IsRead
                });

                notificationIds.Add(notification.UserNotificationId);
            }
            if(notificationIds.Count > 0)
            {
                await MarkNotificationsAsRead(notificationIds);
            }

            return notificationResponses;
        }

        private async Task MarkNotificationsAsRead(List<int> notificationIds)
        {
            var notifications = await _unitOfWork.UserNotifications.FindAll(n => notificationIds.Contains(n.UserNotificationId));
            foreach (var notification in notifications)
            {
                //_unitOfWork.UserNotifications.Delete(notification);
                notification.IsRead = true;
            }
            _unitOfWork.Save();
        }

        public async Task NotifyEmployees(int notificationTypeId, int? menuItemId = null)
        {
            var employees = await _unitOfWork.Users.FindAll(u => u.RoleId == 3);
            foreach (var employee in employees)
            {
                var notification = new UserNotification
                {
                    UserId = employee.UserId,
                    NotificationTypeId = notificationTypeId,
                    MenuItemId = menuItemId,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                await _unitOfWork.UserNotifications.Add(notification);
            }
            _unitOfWork.Save();
        }

        public async Task RemoveNotifications(int menuItemId)
        {
            var notifications = await _unitOfWork.UserNotifications.FindAll(n => n.MenuItemId == menuItemId);
            foreach (var notification in notifications)
            {
                _unitOfWork.UserNotifications.Delete(notification);
                //notification.IsRead = true;
            }
            _unitOfWork.Save();
        }
    }
}
