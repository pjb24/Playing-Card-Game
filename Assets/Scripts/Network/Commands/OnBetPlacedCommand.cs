using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBetPlacedCommand : IGameCommand
{
    Player _player;

    public void Execute(string payload)
    {
        OnBetPlacedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnBetPlacedDTO>(payload);

        Debug.Log("OnBetPlaced, " + "플레이어 " + dto.playerName + "이/가 핸드 ID: " + dto.handId + "에 " + dto.betAmount + "를 베팅하였습니다." + " 플레이어 Guid: " + dto.playerGuid);

        _player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = _player.GetHandByGuid(dto.handId);

        hand.Bet(dto.betAmount);

        WorkForUI(hand);
    }

    private void WorkForUI(PlayerHand hand)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.chipFactory.CreateChipsToFitValue(hand.BetAmount, hand);

            // 모든 칩의 위치를 갱신
            GameManager.Instance.chipFactory.UpdateAllChipsPosition();

            // UI 업데이트. Player Info, Card Value
            UpdateUIChips();
        });
    }

    private void UpdateUIChips()
    {
        foreach (var player in GameManager.Instance.characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                UpdateUiChips(hand);
            }
        }
    }

    private void UpdateUiChips(PlayerHand hand)
    {
        int handIndex = GameManager.Instance.characterManager.GetHandIndex(hand);
        GameManager.Instance.uiManager.PlayerInfoVisible(handIndex);

        Vector3 targetPosition = GameManager.Instance.GetHandPosition(hand);
        GameManager.Instance.uiManager.RequestPlayerInfoPositionUpdate(targetPosition, handIndex);

        GameManager.Instance.uiManager.PlayerInfoBetAmountSetText(hand.BetAmount.ToString("N0"), handIndex);
        GameManager.Instance.uiManager.PlayerInfoNameSetText(_player.DisplayName, handIndex);
        GameManager.Instance.uiManager.PlayerInfoChipSetText(_player.Chips.ToString("N0"), handIndex);
    }
}
