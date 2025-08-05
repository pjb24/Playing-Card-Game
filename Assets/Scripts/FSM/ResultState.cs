using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_EvaluationResult
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

    public E_EvaluationResult Evaluate(Hand playerHand, Hand dealerHand)
    {
        E_EvaluationResult result;

        bool isBlackjackPlayer = playerHand.IsBlackjack();
        bool isBlackjackDealer = dealerHand.IsBlackjack();

        int playerValue = playerHand.GetValue();
        int dealerValue = dealerHand.GetValue();

        if (playerValue == dealerValue)
        {
            result = E_EvaluationResult.Push;
        }
        else if (playerValue < dealerValue)
        {
            result = E_EvaluationResult.Lose;
        }
        else
        {
            result = E_EvaluationResult.Win;
        }

        if (dealerHand.IsBust())
        {
            result = E_EvaluationResult.Win;
        }

        if (playerHand.IsBust())
        {
            result = E_EvaluationResult.Lose;
        }

        if (isBlackjackDealer && isBlackjackPlayer)
        {
            result = E_EvaluationResult.Push;
        }
        else if (isBlackjackDealer)
        {
            result = E_EvaluationResult.Lose;
        }
        else if (isBlackjackPlayer)
        {
            result = E_EvaluationResult.Blackjack;
        }

        return result;
    }

    public void ApplyPayout(Player player, PlayerHand hand, E_EvaluationResult result)
    {
        switch (result)
        {
            case E_EvaluationResult.Win:
                {
                    player.Win(hand);
                    break;
                }
            case E_EvaluationResult.Lose:
                {
                    player.Lose(hand);
                    break;
                }
            case E_EvaluationResult.Push:
                {
                    player.Push(hand);
                    break;
                }
            case E_EvaluationResult.Blackjack:
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
                E_EvaluationResult result = Evaluate(hand, dealerHand);

                ApplyPayout(player, hand, result);

                // Animate chip
            }
        }
    }
}
