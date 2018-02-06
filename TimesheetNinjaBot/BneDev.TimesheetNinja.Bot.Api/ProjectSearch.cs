using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DuoVia.FuzzyStrings;
using BneDev.TimesheetNinja.Bot.Api.Models;

namespace BneDev.TimesheetNinja.Bot.Api
{
    public static class ProjectSearch
    {

        public static IEnumerable<IProject> FindAll(this IEnumerable<IProject> projects, string searchQuery)
        {
            // A multi-faceted search (implementation could probably be simplified by assigning numeric scores to give precedence to best matches)

            var matches = from project in projects
                          let codeExatchMatch = String.Equals(project.Code, searchQuery, StringComparison.OrdinalIgnoreCase)
                          let codeStartsWithMatch = project.Code.StartsWith(searchQuery.Substring(0, Math.Min(4, searchQuery.Length)), StringComparison.OrdinalIgnoreCase)
                          let descriptionContainsMatch = project.Description.ToUpperInvariant().Contains(searchQuery.ToUpperInvariant())
                          let levenshteinEditDistance = searchQuery.LevenshteinDistance(project.Code)
                          let fuzzyCoefficient = searchQuery.FuzzyMatch(project.Description)
                          orderby codeExatchMatch, codeStartsWithMatch, fuzzyCoefficient descending
                          select (project, codeExatchMatch, codeStartsWithMatch, descriptionContainsMatch, levenshteinEditDistance, fuzzyCoefficient);

            var highScoringMatches = matches.Where(p => p.codeExatchMatch || p.codeStartsWithMatch || p.descriptionContainsMatch || p.levenshteinEditDistance <= 1 || p.fuzzyCoefficient >= 0.75).ToArray();
            if (highScoringMatches.Any())
            {
                // Narrow to the best result

                var exactMatches = highScoringMatches
                        .Where(x => x.codeExatchMatch || x.codeStartsWithMatch || x.descriptionContainsMatch || x.levenshteinEditDistance <= 1);

                var max = highScoringMatches.Max(x => x.fuzzyCoefficient);
                var average = highScoringMatches.Average(x => x.fuzzyCoefficient);

                if (exactMatches.Any())
                {
                    var exactMatch = exactMatches.Where(x => x.codeExatchMatch);
                    if (exactMatch.Count() == 1)
                    {
                        return new[] { exactMatch.Single().project };
                    }
                    else
                    {
                        return exactMatches.Select(x => x.project).ToArray();
                    }
                }
                else if (max - average > 0.03)
                {
                    return new[] { highScoringMatches.First(x => x.fuzzyCoefficient == max).project };
                }
                else
                {
                    return highScoringMatches.Select(p => p.project).ToArray();
                }
            }
            else
            {
                return projects.ToArray();
            }
        }

    }
}
