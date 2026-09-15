using System;

namespace CandyCrush.Core
{
    /// <summary>
    /// Candy types in the game
    /// </summary>
    public enum CandyType
    {
        None,
        Red,
        Orange,
        Yellow,
        Green,
        Blue,
        Purple
    }

    /// <summary>
    /// Special candy types
    /// </summary>
    public enum SpecialCandyType
    {
        None,
        Striped,        // 4-match: clears row/column
        Wrapped,        // L-shape: creates explosion
        ColorBomb       // 5-match: clears all matching colors
    }

    /// <summary>
    /// Blocker types on the grid
    /// </summary>
    public enum BlockerType
    {
        None,
        Chocolate,      // Spreads if not cleared in 2 moves
        Licorice,       // Blocks candy movement
        Marmalade       // Needs 2 hits to clear
    }

    /// <summary>
    /// Level types
    /// </summary>
    public enum LevelType
    {
        Jelly,          // Clear all jelly squares
        Ingredient,     // Collect specific ingredients
        Timed,          // Beat target score in time limit
        Order,          // Collect specific candies
        Monkling        // Defeat boss/monkling
    }

    /// <summary>
    /// Level difficulty
    /// </summary>
    public enum Difficulty
    {
        Easy,           // Levels 1-30
        Medium,         // Levels 31-100
        Hard,           // Levels 101-200
        Expert          // Levels 201+
    }

    /// <summary>
    /// Game states
    /// </summary>
    public enum GameState
    {
        Loading,
        Playing,
        AnimatingCascade,
        LevelComplete,
        LevelFailed,
        Paused
    }

    /// <summary>
    /// Booster types
    /// </summary>
    public enum BoosterType
    {
        Lollipop,           // Clear one candy
        ColorBomb,          // Clear all matching colors
        Striped,            // Clear row/column
        ExtraMoves,         // Add moves
        PreGameStriped,     // Pre-level booster
        PreGameColorBomb
    }

    /// <summary>
    /// Task types
    /// </summary>
    public enum TaskType
    {
        Daily,
        Weekly,
        Achievement,
        GroupTask
    }

    /// <summary>
    /// Task status
    /// </summary>
    public enum TaskStatus
    {
        Active,
        Completed,
        Claimed
    }

    /// <summary>
    /// Currency types
    /// </summary>
    public enum CurrencyType
    {
        Coins,          // Soft currency
        Gems            // Hard currency (IAP)
    }

    /// <summary>
    /// Event types
    /// </summary>
    public enum EventType
    {
        SugarRushTournament,    // 48 hour tournament
        ChampionshipLeague,     // Weekly league
        LiveDuelMode            // 1v1 real-time
    }

    /// <summary>
    /// Player rank in events
    /// </summary>
    public enum PlayerRank
    {
        Diamond,
        Platinum,
        Gold,
        Silver,
        Bronze
    }

    /// <summary>
    /// Team member roles
    /// </summary>
    public enum TeamRole
    {
        Member,
        Leader
    }

    /// <summary>
    /// Ad types
    /// </summary>
    public enum AdType
    {
        Banner,
        Interstitial,
        Rewarded
    }

    /// <summary>
    /// IAP product types
    /// </summary>
    public enum IAPProductType
    {
        CoinsPack,
        LivesPack,
        BoosterBundle,
        Subscription
    }
}
