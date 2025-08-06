using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBetPlacedCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnBetPlacedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnBetPlacedDTO>(payload);

        Debug.Log("OnBetPlaced, " + "플레이어 " + dto.playerName + "이/가 핸드 ID: " + dto.handId + "에 " + dto.betAmount + "를 베팅하였습니다." + " 플레이어 Guid: " + dto.playerGuid);

        Player player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        hand.Bet(dto.betAmount);

        WorkForUI(hand);

        yield return null;
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
                UpdateUiChips(hand, player);
            }
        }
    }

    private void UpdateUiChips(PlayerHand hand, Player player)
    {
        int handIndex = GameManager.Instance.characterManager.GetHandIndex(hand);
        GameManager.Instance.uiManager.PlayerInfoVisible(handIndex);

        Vector3 targetPosition = GameManager.Instance.GetHandPosition(hand);

        GameManager.Instance.uiManager.PlayerInfoBetAmountSetText(hand.BetAmount.ToString("N0"), handIndex);
        GameManager.Instance.uiManager.PlayerInfoNameSetText(player.DisplayName, handIndex);
        GameManager.Instance.uiManager.PlayerInfoChipSetText(player.Chips.ToString("N0"), handIndex);
    }
}
