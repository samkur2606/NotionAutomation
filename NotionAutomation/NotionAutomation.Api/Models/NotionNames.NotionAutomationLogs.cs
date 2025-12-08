namespace NotionAutomation.Api.Models;

public static partial class NotionNames
{
    public static class NotionAutomationLogs
    {
        public const string Database = "Notion Automation Logs";

        public static class Properties
        {
            public const string Name = "Name";
            public const string LoggingTime = "Logging Time";
            public const string LogLevel = "Log Level";
            public const string Source = "Source";
            public const string Message = "Message";
            public const string Exception = "Exception";
            public const string AdditionalData = "Raw Json";
        }
    }
}