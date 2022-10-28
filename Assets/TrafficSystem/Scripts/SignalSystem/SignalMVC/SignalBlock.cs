namespace TrafficSystem
{
    /// <summary>
    /// Single Signal Block Model.
    /// One block represents one single light on a signal.
    /// Ex. Red light is one Signal Block.
    /// </summary>
    public struct SignalBlock
    {
        public SignalBlockStateID CurrentState;
        public SignalDirectionID AssignedDirection;

        public SignalBlock(SignalBlockStateID state, SignalDirectionID direction)
        {
            CurrentState = state;
            AssignedDirection = direction;
        }
    }
}