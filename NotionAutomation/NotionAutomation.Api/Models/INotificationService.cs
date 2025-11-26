namespace NotionAutomation.Api.Models;

public interface INotificationService
{
    Task NotifySuccess(string message);
    Task NotifyError(string message, Exception? ex = null);
}