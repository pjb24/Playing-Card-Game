using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndState : IGameState
{
    public void Enter()
    {
        GameManager.Instance.StartCoroutine(WaitAndTransition());
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }

    private IEnumerator WaitAndTransition()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ChangeState(new GameStartState());
    }
}
