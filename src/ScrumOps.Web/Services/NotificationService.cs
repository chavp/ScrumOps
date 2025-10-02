namespace ScrumOps.Web.Services;

public class NotificationService : INotificationService
{
    public event Action<NotificationMessage>? OnNotification;

    public void ShowSuccess(string message, string? title = null)
    {
        var notification = new NotificationMessage
        {
            Message = message,
            Title = title ?? "Success",
            Type = NotificationType.Success
        };
        OnNotification?.Invoke(notification);
    }

    public void ShowError(string message, string? title = null)
    {
        var notification = new NotificationMessage
        {
            Message = message,
            Title = title ?? "Error",
            Type = NotificationType.Error
        };
        OnNotification?.Invoke(notification);
    }

    public void ShowWarning(string message, string? title = null)
    {
        var notification = new NotificationMessage
        {
            Message = message,
            Title = title ?? "Warning",
            Type = NotificationType.Warning
        };
        OnNotification?.Invoke(notification);
    }

    public void ShowInfo(string message, string? title = null)
    {
        var notification = new NotificationMessage
        {
            Message = message,
            Title = title ?? "Information",
            Type = NotificationType.Info
        };
        OnNotification?.Invoke(notification);
    }
}