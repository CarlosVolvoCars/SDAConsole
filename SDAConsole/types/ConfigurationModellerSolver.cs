using SDAConsole.types.ConfigHub;

namespace SDAConsole.types.ConfigurationModeller;

public class Result
{
    public List<ECUListApplicableSubBaseline> Solved { get; set; } = new List<ECUListApplicableSubBaseline>();
    public List<ECUListApplicableSubBaseline> Unsolved { get; set; } = new List<ECUListApplicableSubBaseline>();
}

public static class ConfigurationFilter
{
    public static Result FilterAndScoreConfigurations(
        List<ECUListApplicableSubBaseline> configurations,
        List<string> steeringCoMoExpressions)
    {
        var solved = new List<ECUListApplicableSubBaseline>();
        var unsolved = new List<ECUListApplicableSubBaseline>();

        // Group by EcuName and SWType
        var grouped = configurations
            .OrderBy(c => c.EcuName)
            .ThenBy(c => c.SWType)
            .GroupBy(c => new { c.EcuName, c.SWType });

        foreach (var group in grouped)
        {
            ECUListApplicableSubBaseline best = null;
            int bestScore = -1;
            int invalidCount = 0;

            foreach (var config in group)
            {

                // Find the best scoring variant expression
                var bestVariant = config.VariantExpressions
                .Select(ve => new
                {
                    Expression = ve,
                    Score =
                        // Positive score for matches
                        ve.ConfigurationParameterValues.Count(val => steeringCoMoExpressions.Contains(val))
                        // Negative score for mismatches
                        - ve.ConfigurationParameterValues.Count(val => !steeringCoMoExpressions.Contains(val))
                        // Additional scoring
                        + (steeringCoMoExpressions.Contains(ve.Context) ? 1 : 0)
                        + (steeringCoMoExpressions.Contains(ve.ecuSolutionParameter) ? 1 : 0)
                })
                .OrderByDescending(x => x.Score)
                .FirstOrDefault();

                // Check that the best variant does not contain any invalid values
                // An invalid value is defined as a match for the first part of a steeringCoMoExpression (XXXX)
                // but not the second part (YYY). If this happens, the score of invalidCount is set.
                invalidCount = 0;

                // Print the current best variant for debugging
                // Console.WriteLine($"Best Variant: {bestVariant?.Expression.Comment} - Score: {bestVariant?.Score}");

                if (bestVariant != null && bestVariant.Score > 0)
                {

                    foreach (var expr in steeringCoMoExpressions)
                    {
                        var parts = expr.Split('-');
                        if (parts.Length == 2)
                        {
                            var prefix = parts[0];
                            var suffix = parts[1];


                            // Check if the prefix matches any of the bestVariant fields
                            if (bestVariant.Expression.ecuSolutionParameter.Contains(prefix)||
                                bestVariant.Expression.Context.Contains(prefix) ||
                                bestVariant.Expression.ConfigurationParameterValues.Any(val => val.Contains(prefix)))

                            {

                                // If the matched by prefix-items have a suffix that does not match, it's invalid
                                if (!bestVariant.Expression.ecuSolutionParameter.Contains(suffix) &&
                                    !bestVariant.Expression.Context.Contains(suffix) &&
                                    !bestVariant.Expression.ConfigurationParameterValues.Any(val => val.Contains(suffix)))
                                {
                                    invalidCount++;
                                    // Console.WriteLine($"Invalid Count Incremented: {invalidCount}");
                                    break; // No need to check further, we found an invalid Expression
                                }

                            }
                        }
                    }
                }

                // Print the invalid count for debugging
                // Console.WriteLine($"Invalid Count: {invalidCount}");
                // Print the Score for debugging
                // Console.WriteLine($"Score: {bestVariant?.Score}");

                if (bestVariant != null && bestVariant.Score > bestScore && invalidCount == 0)
                {
                    bestScore = bestVariant.Score;
                    best = new ECUListApplicableSubBaseline
                    {
                        SWType = config.SWType,
                        SWPartNumber = config.SWPartNumber,
                        EcuName = config.EcuName,
                        EcuAddress = config.EcuAddress,
                        EcuHardwarePartNumber = config.EcuHardwarePartNumber,
                        EcuHardwarePartVersion = config.EcuHardwarePartVersion,
                        SubBaselineID = config.SubBaselineID,
                        VariantExpressions = new List<VariantExpression> { bestVariant.Expression }
                    };
                }
            }

            if (best != null && bestScore > 0)
            {
                solved.Add(best);
            }
            else
            {
                // No matching configuration found
                unsolved.AddRange(group);
            }
        }

        return new Result
        {
            Solved = solved,
            Unsolved = unsolved
        };
    }
}
