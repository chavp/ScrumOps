namespace ScrumOps.Web.Services;

public interface INotificationService
{
    event Action<NotificationMessage>? OnNotification;
    
    void ShowSuccess(string message, string? title = null);
    void ShowError(string message, string? title = null);
    void ShowWarning(string message, string? title = null);
    void ShowInfo(string message, string? title = null);
}

public class NotificationMessage
{
    public string Message { get; set; } = string.Empty;
    public string? Title { get; set; }
    public NotificationType Type { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public Guid Id { get; set; } = Guid.NewGuid();
}

public enum NotificationType
{
    Success,
    Error,
    Warning,
    Info
}