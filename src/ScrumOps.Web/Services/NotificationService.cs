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

    public async Task<bool> ShowConfirmation(string message, string? title = null)
    {
        // For now, just return true (would need a proper confirmation dialog implementation)
        // In a real implementation, this would show a modal dialog and wait for user input
        await Task.Delay(1); // Simulate async operation
        return true;
    }
}