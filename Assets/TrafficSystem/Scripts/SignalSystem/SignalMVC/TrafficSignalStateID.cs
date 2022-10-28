namespace TrafficSystem
{
    /// <summary>
    /// Represents a Signal State. 
    /// None state wouldn't do changes.
    /// Red state stops the vehicles.
    /// Yellow state is just a changing light.
    /// Green state will allow vehicles to move, given that the direction they want to move in, is also active.
    /// </summary>
    public enum TrafficSignalStateID
    {
        None,
        Red,
        Yellow,
        Green
    }
}