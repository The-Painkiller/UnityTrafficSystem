namespace TrafficSystem
{
    /// <summary>
    /// Represents the current state of a single signal block.
    /// It can be either off, on, or always turned on (for example for turning to one-way roads.)
    /// </summary>
    public enum SignalBlockStateID
    {
        Off,
        On,
        AlwaysOn
    }
}