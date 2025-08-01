using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHandSplitCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnHandSplitDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnHandSplitDTO>(payload);

        Debug.Log("OnHandSplit, " + "�÷��̾�: " + dto.playerName + "�� " + "�ڵ� ID: " + dto.handId + "�� Split�Ͽ� ���ڵ� ID: " + dto.newHandId + "�� �����մϴ�. " + "�÷��̾� Guid: " + dto.playerGuid);

        Player player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        // ���ο� �ڵ带 ���� �ڵ��� ������ �߰�
        PlayerHand newHand = player.InsertHand(player.Hands.IndexOf(hand) + 1, dto.newHandId);

        WorkForUI(newHand);

        // ���� �ڵ��� 2��° ī�带 ���ο� �ڵ�� ����
        Card splitCard = hand.Cards[1];
        hand.RemoveCard(splitCard);
        newHand.AddCard(splitCard);

        // ī�� ������Ʈ ����
        GameObject splitCardObj = hand.cardObjects[1];
        hand.cardObjects.Remove(splitCardObj);
        newHand.cardObjects.Add(splitCardObj);
    }

    private void WorkForUI(PlayerHand hand)
    {
        // �ڵ忡 �´� UI Insert
        int newHandIndex = GameManager.Instance.characterManager.GetHandIndex(hand);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.uiManager.CreateLabelCardValuePlayer(newHandIndex);
            GameManager.Instance.uiManager.CreatePlayerInfo(newHandIndex);

            // ��� ī�� ��ġ ����
            GameManager.Instance.UpdateAllPlayerHandPositions();

            UpdateUICardValue();

            GameManager.Instance.uiManager.RequestCardValueUIPositionUpdate_Y_Register(newHandIndex);
            GameManager.Instance.uiManager.RequestPlayerInfoPositionUpdate_Y_Register(newHandIndex);
        });
    }

    private void UpdateUICardValue()
    {
        foreach (var player in GameManager.Instance.characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                UpdateUICardValue(hand);
            }
        }
    }

    private void UpdateUICardValue(PlayerHand hand)
    {
        int handIndex = GameManager.Instance.characterManager.GetHandIndex(hand);
        GameManager.Instance.uiManager.CardValuePlayerVisible(handIndex);

        Vector3 targetPosition = GameManager.Instance.GetHandPosition(hand);
        GameManager.Instance.uiManager.RequestCardValueUIPositionUpdate(targetPosition, handIndex);

        GameManager.Instance.uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
    }
}
