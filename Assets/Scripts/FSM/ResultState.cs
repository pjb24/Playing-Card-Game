using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_EvaluateResult
{
    Win = 0,
    Lose = 1,
    Push = 2,
    Blackjack = 3,
}

public class ResultState : IGameState
{
    public void Enter()
    {
        EvaluateResults();

        GameManager.Instance.ChangeState(new GameEndState());
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }

    public E_EvaluateResult Evaluate(Hand playerHand, Hand dealerHand)
    {
        E_EvaluateResult result;

        bool isBlackjackPlayer = playerHand.IsBlackjack();
        bool isBlackjackDealer = dealerHand.IsBlackjack();

        int playerValue = playerHand.GetValue();
        int dealerValue = dealerHand.GetValue();

        if (playerValue == dealerValue)
        {
            result = E_EvaluateResult.Push;
        }
        else if (playerValue < dealerValue)
        {
            result = E_EvaluateResult.Lose;
        }
        else
        {
            result = E_EvaluateResult.Win;
        }

        if (dealerHand.IsBust())
        {
            result = E_EvaluateResult.Win;
        }

        if (playerHand.IsBust())
        {
            result = E_EvaluateResult.Lose;
        }

        if (isBlackjackDealer && isBlackjackPlayer)
        {
            result = E_EvaluateResult.Push;
        }
        else if (isBlackjackDealer)
        {
            result = E_EvaluateResult.Lose;
        }
        else if (isBlackjackPlayer)
        {
            result = E_EvaluateResult.Blackjack;
        }

        return result;
    }

    public void ApplyPayout(Player player, PlayerHand hand, E_EvaluateResult result)
    {
        switch (result)
        {
            case E_EvaluateResult.Win:
                {
                    player.Win(hand);
                    break;
                }
            case E_EvaluateResult.Lose:
                {
                    player.Lose(hand);
                    break;
                }
            case E_EvaluateResult.Push:
                {
                    player.Push(hand);
                    break;
                }
            case E_EvaluateResult.Blackjack:
                {
                    player.Blackjack(hand);
                    break;
                }
        }
    }

    private void EvaluateResults()
    {
        DealerHand dealerHand = GameManager.Instance.characterManager.dealer.Hand;

        foreach (Player player in GameManager.Instance.characterManager.Players)
        {
            foreach (PlayerHand hand in player.Hands)
            {
                E_EvaluateResult result = Evaluate(hand, dealerHand);

                ApplyPayout(player, hand, result);

                // Animate chip
            }
        }
    }
}
