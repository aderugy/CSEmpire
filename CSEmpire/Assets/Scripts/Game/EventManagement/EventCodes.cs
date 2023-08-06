namespace Game.EventManagement
{
    /// <summary>
    /// All the event codes.
    /// Used as event IDs when raised.
    /// </summary>
    public enum EventCodes : byte
    {
        AddGoldToAllPlayers = 1,
        UpdateRounds = 2,
        PlayerKilled = 3,
        SpawnPlayers = 4,
        StartTimer = 5
    }
}