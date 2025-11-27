using System.ComponentModel;

namespace NotionAutomation.Api.Models;

public enum NotionLogLevel
{
    [Description("Debug")] Debug,
    [Description("Information")] Information,
    [Description("Warning")] Warning,
    [Description("Critical")] Critical,
    [Description("Error")] Error
}