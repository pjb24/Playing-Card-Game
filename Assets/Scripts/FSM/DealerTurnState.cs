using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerTurnState : IGameState
{
    public void Enter()
    {
        GameManager.Instance.ChangeState(new ResultState());
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }
}
