namespace TrafficSystem
{
    /// <summary>
    /// Model class for a Traffic Signal.
    /// Each Signal has the number of directions they support and
    /// the number of directions they have active right now.
    /// It also has stores the current state of the signal as well as
    /// the time interval between each timebox.
    /// </summary>
    public struct TrafficSignalModel
    {
        public SignalDirectionID[] GreenSignalDirections;
        public SignalDirectionID[] CurrentActiveDirections;
        public TrafficSignalStateID CurrentSignalState;
        public int SignalChangeInterval;

        public TrafficSignalModel(SignalDirectionID[] supportedDirecions, int signalInterval)
        {
            GreenSignalDirections = supportedDirecions;
            SignalChangeInterval = signalInterval;
            CurrentSignalState = TrafficSignalStateID.None;
            CurrentActiveDirections = null;
        }
    }
}