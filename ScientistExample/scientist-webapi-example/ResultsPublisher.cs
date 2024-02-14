using System.Diagnostics;
using GitHub;

namespace scientist_webapi_example;

public class ResultsPublisher : IResultPublisher
{
    public Task Publish<T, TClean>(Result<T, TClean> result)
    {
        Debug.WriteLine($"Publishing results for experiment '{result.ExperimentName}'");
        Debug.WriteLine($"Result: {(result.Matched ? "MATCH" : "MISMATCH")}");
        Debug.WriteLine($"Control value: {result.Control.Value}");
        Debug.WriteLine($"Control duration: {result.Control.Duration}");
        foreach (var observation in result.Candidates)
        {
            Debug.WriteLine($"Candidate name: {observation.Name}");
            Debug.WriteLine($"Candidate value: {observation.Value}");
            Debug.WriteLine($"Candidate duration: {observation.Duration}");
        }

        if (result.Mismatched)
        {
            // saved mismatched experiments to DB

        }

        return Task.FromResult(0);
    }
}