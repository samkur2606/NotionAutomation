using System.Diagnostics;

namespace NotionAutomation.Api.Jobs;

public class TestJob
{
    public void Run()
    {
        Debug.WriteLine("TestJob wurde ausgeführt!");
    }
}