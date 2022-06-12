/// <summary>
/// Single Signal Block Model.
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
