using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BettingState : IGameState
{
    public void Enter()
    {
        GameManager.Instance.ChangeState(new DealingState());
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }
}
