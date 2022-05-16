using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficSignal : MonoBehaviour
{
    [SerializeField]
    private SignalDirection _redSignal;
    [SerializeField]
    private SignalDirection _yellowSignal;
    [SerializeField]
    private SignalDirection[] _greenSignals = null;
}

[Serializable]
public struct SignalDirection
{
    public SignalDirectionID Direction;
    public SignalBlock SignalObject;
}
