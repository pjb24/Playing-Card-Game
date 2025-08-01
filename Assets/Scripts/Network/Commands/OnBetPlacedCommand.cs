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

        Debug.Log("OnBetPlaced, " + "�÷��̾� " + dto.playerName + "��/�� �ڵ� ID: " + dto.handId + "�� " + dto.betAmount + "�� �����Ͽ����ϴ�." + " �÷��̾� Guid: " + dto.playerGuid);

        _player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = _player.GetHandByGuid(dto.handId);

        WorkForUI(hand);
    }

    private void WorkForUI(PlayerHand hand)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.chipFactory.CreateChipsToFitValue(hand.BetAmount, hand);

            // ��� Ĩ�� ��ġ�� ����
            GameManager.Instance.chipFactory.UpdateAllChipsPosition();

            // UI ������Ʈ. Player Info, Card Value
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
