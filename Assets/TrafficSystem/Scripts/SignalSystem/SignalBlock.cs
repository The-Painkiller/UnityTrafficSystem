using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalBlock : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer = null;
    [SerializeField]
    private Sprite _OffStateVisual = null;
    [SerializeField]
    private Sprite _OnStateVisual = null;

    private SignalBlockStateID _stateID = SignalBlockStateID.Off;

    private void Awake()
    {
        if (_renderer == null
            || _OffStateVisual == null
            || _OnStateVisual == null)
        {
            this.enabled = false;
            gameObject.SetActive(false);
        }
    }

    public void ChangeState(SignalBlockStateID state)
    {
        _stateID = state;
        ChangeVisuals();
    }

    private void ChangeVisuals()
    {
        switch (_stateID)
        {
            case SignalBlockStateID.On:
            case SignalBlockStateID.AlwaysOn:
                _renderer.sprite = _OnStateVisual;
                break;

            case SignalBlockStateID.Off:
                _renderer.sprite = _OffStateVisual;
                break;
        }
    }
}
