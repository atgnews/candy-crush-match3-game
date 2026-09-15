using System.Collections.Generic;
using CandyCrush.Core;
using CandyCrush.Events;

namespace CandyCrush.Gameplay
{
    /// <summary>
    /// Detects candy matches and calculates scores
    /// </summary>
    public class MatchDetectionEngine
    {
        private GridSystem gridSystem;
        private const int MIN_MATCH_LENGTH = 3;

        public MatchDetectionEngine(GridSystem grid)
        {
            gridSystem = grid;
        }

        /// <summary>
        /// Detect all matches and calculate score
        /// </summary>
        public MatchResult DetectMatches()
        {
            MatchResult result = new MatchResult();
            List<List<Candy>> matches = gridSystem.FindMatches();

            if (matches.Count == 0)
                return result;

            result.MatchedCandies = matches;
            result.BaseScore = CalculateScore(matches);
            result.SpecialCandiesCreated = GetSpecialCandies(matches);

            return result;
        }

        /// <summary>
        /// Calculate score from matches
        /// </summary>
        private int CalculateScore(List<List<Candy>> matches)
        {
            int score = 0;

            foreach (var match in matches)
            {
                // 3 match = 100 points
                // 4 match = 400 points (+ striped candy)
                // 5 match = 1000 points (+ color bomb)
                // 6+ match = 1000 + (count - 5) * 200

                int matchLength = match.Count;
                if (matchLength == 3)
                    score += 100;
                else if (matchLength == 4)
                    score += 400;
                else if (matchLength == 5)
                    score += 1000;
                else if (matchLength > 5)
                    score += 1000 + (matchLength - 5) * 200;
            }

            return score;
        }

        /// <summary>
        /// Determine special candies created from matches
        /// </summary>
        private List<(Candy candy, SpecialCandyType type)> GetSpecialCandies(List<List<Candy>> matches)
        {
            List<(Candy, SpecialCandyType)> specialCandies = new List<(Candy, SpecialCandyType)>();

            foreach (var match in matches)
            {
                if (match.Count == 4)
                {
                    // Create striped candy at match position
                    specialCandies.Add((match[0], SpecialCandyType.Striped));
                }
                else if (match.Count == 5)
                {
                    // Create color bomb at match center
                    int centerIdx = match.Count / 2;
                    specialCandies.Add((match[centerIdx], SpecialCandyType.ColorBomb));
                }
            }

            return specialCandies;
        }
    }

    /// <summary>
    /// Result of match detection
    /// </summary>
    public class MatchResult
    {
        public List<List<Candy>> MatchedCandies { get; set; } = new List<List<Candy>>();
        public int BaseScore { get; set; }
        public List<(Candy candy, SpecialCandyType type)> SpecialCandiesCreated { get; set; } = new List<(Candy, SpecialCandyType)>();
        public bool HasMatches => MatchedCandies.Count > 0;
    }
}
