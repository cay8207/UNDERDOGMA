using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class FSM
{
    public FSM(BaseState initState)
    {
        _curState = initState;
        ChangeState(_curState);
    }

    private BaseState _curState;

    public void ChangeState(BaseState nextState)
    {
        Debug.Log("Current State: " + _curState + " NextState: " + nextState);

        if (nextState == _curState)
        {
            return;
        }

        if (_curState != null)
        {
            _curState.OnStateExit();
        }

        _curState = nextState;
        _curState.OnStateEnter();
    }

    public void UpdateState()
    {
        if (_curState != null)
        {
            _curState.OnStateUpdate();
        }
    }
}
