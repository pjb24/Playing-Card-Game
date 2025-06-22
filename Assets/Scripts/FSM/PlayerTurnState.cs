using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : IGameState
{
    public void Enter()
    {
        GameManager.Instance.ChangeState(new DealerTurnState());
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }
}
