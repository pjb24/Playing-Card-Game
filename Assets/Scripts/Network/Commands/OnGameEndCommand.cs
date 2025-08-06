using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGameEndCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnGameEndDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnGameEndDTO>(payload);

        Debug.Log("OnGameEnd");

        WorkForUI();

        yield return null;
    }

    private void WorkForUI()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.StartCoroutine(WaitAndExecute(2f));
        });
    }

    private void UpdateUI_PlayerInfos()
    {
        GameManager.Instance.uiManager.RemoveAllPlayerInfos();

        foreach (var player in GameManager.Instance.characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                int handIndex = GameManager.Instance.characterManager.GetHandIndex(hand);
                GameManager.Instance.uiManager.CreatePlayerInfo(handIndex);
                GameManager.Instance.uiManager.PlayerInfoVisible(handIndex);

                GameManager.Instance.uiManager.PlayerInfoBetAmountSetText(hand.BetAmount.ToString("N0"), handIndex);
                GameManager.Instance.uiManager.PlayerInfoNameSetText(player.DisplayName, handIndex);
                GameManager.Instance.uiManager.PlayerInfoChipSetText(player.Chips.ToString("N0"), handIndex);

                Vector3 targetPosition = GameManager.Instance.GetHandPosition(hand);
                GameManager.Instance.uiManager.RequestPlayerInfoPositionUpdate_Register(targetPosition, handIndex);
                GameManager.Instance.uiManager.RequestPlayerInfoPositionUpdate_Y_Register(handIndex);
            }
        }
    }

    private void UpdateUI_CardValues()
    {
        GameManager.Instance.uiManager.RemoveDealerCardValue();
        GameManager.Instance.uiManager.RemoveAllLabelCardValue();
    }

    private void HandleJoin()
    {
        StartGameDTO startGameDTO = new StartGameDTO();
        string startGameJson = Newtonsoft.Json.JsonConvert.SerializeObject(startGameDTO);
        GameManager.Instance.SignalRClient.Execute("StartGame", startGameJson);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.uiManager.button_Join.clicked -= HandleJoin;
        });
    }

    private IEnumerator WaitAndExecute(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        foreach (var player in GameManager.Instance.characterManager.Players)
        {
            player.ResetForNextRound_Network();
        }

        GameManager.Instance.characterManager.dealer.ResetHand();

        UpdateUI_PlayerInfos();
        UpdateUI_CardValues();

        GameManager.Instance.uiManager.ChangeToBetPanel();

        GameManager.Instance.uiManager.button_Join.visible = false;

        ReadyToNextRoundDTO readyToNextRoundDTO = new();
        string readyToNextRoundJson = Newtonsoft.Json.JsonConvert.SerializeObject(readyToNextRoundDTO);
        GameManager.Instance.SignalRClient.Execute("ReadyToNextRound", readyToNextRoundJson);
    }
}
