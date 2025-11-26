using System.ComponentModel;

namespace NotionAutomation.Api.Models;

public enum VacationStatus
{
    [Description("Visited")] Visited,
    [Description("Bucket List")] BucketList
}