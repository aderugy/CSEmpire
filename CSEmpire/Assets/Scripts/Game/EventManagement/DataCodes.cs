namespace Game.EventManagement
{
    /// <summary>
    /// Keys of the dictionary serialized and sent to all the clients when an event is raised.
    /// Allows to accurately retrieve the value in the event data and improves readability.
    /// </summary>
    public enum DataCodes : byte
    {
        GoldAmount = 1,
        CurrentRound = 2,
        KillerViewID = 3,
        KilledViewID = 4,
        TimerTitle = 5,
        TimerCount = 6,
        SetUntouchable
    }
}