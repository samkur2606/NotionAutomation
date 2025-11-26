namespace NotionAutomation.Api.Models;

public readonly record struct DateRange(DateTime Start, DateTime End)
{
    public int Days { get; } = (End.Date - Start.Date).Days + 1;

    public bool IsInRange(DateTime date)
    {
        var isInRange = date.Date >= Start.Date && date.Date <= End.Date;
        return isInRange;
    }

    public override string ToString()
    {
        var rangeAsString = $"{Start:yyyy-MM-dd} → {End:yyyy-MM-dd}";
        return rangeAsString;
    }
}