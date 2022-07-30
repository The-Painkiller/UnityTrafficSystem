public struct TrafficSignalModel
{
    //public SignalBlock RedSignal;
    //public SignalBlock YellowSignal;
    //public SignalBlock[] GreenSignals;
    public SignalDirectionID[] GreenSignalDirections;
    public SignalDirectionID[] CurrentActiveDirections;
    public TrafficSignalStateID CurrentSignalState;
    public int SignalChangeInterval;

    public TrafficSignalModel(SignalDirectionID[] supportedDirecions, int signalInterval)
    {
        GreenSignalDirections = supportedDirecions;
        SignalChangeInterval = signalInterval;
        CurrentSignalState = TrafficSignalStateID.None ;
        CurrentActiveDirections = null;
    }
    //public TrafficSignalModel(SignalBlock redSignal, SignalBlock yellowSignal, SignalBlock[] greenSignals, int signalInterval, TrafficSignalStateID currentState)
    //{
    //    RedSignal = redSignal;
    //    YellowSignal = yellowSignal;
    //    GreenSignals = greenSignals;
    //    CurrentSignalState = currentState;
    //    SignalChangeInterval = signalInterval;
    //}
}
