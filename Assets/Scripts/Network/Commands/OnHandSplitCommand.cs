using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHandSplitCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnHandSplitDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnHandSplitDTO>(payload);

        Debug.Log("OnHandSplit, " + "플레이어: " + dto.playerName + "의 " + "핸드 ID: " + dto.handId + "를 Split하여 새핸드 ID: " + dto.newHandId + "를 생성합니다. " + "플레이어 Guid: " + dto.playerGuid);

        Player player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        // 새로운 핸드를 현재 핸드의 오른편에 추가
        PlayerHand newHand = player.InsertHand(player.Hands.IndexOf(hand) + 1, dto.newHandId);

        WorkForUI(newHand);

        // 현재 핸드의 2번째 카드를 새로운 핸드로 나눔
        Card splitCard = hand.Cards[1];
        hand.RemoveCard(splitCard);
        newHand.AddCard(splitCard);

        // 카드 오브젝트 나눔
        GameObject splitCardObj = hand.cardObjects[1];
        hand.cardObjects.Remove(splitCardObj);
        newHand.cardObjects.Add(splitCardObj);

        yield return null;
    }

    private void WorkForUI(PlayerHand hand)
    {
        // 핸드에 맞는 UI Insert
        int newHandIndex = GameManager.Instance.characterManager.GetHandIndex(hand);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.uiManager.CreateLabelCardValuePlayer(newHandIndex);
            GameManager.Instance.uiManager.CreatePlayerInfo(newHandIndex);

            // 모든 카드 위치 갱신
            GameManager.Instance.UpdateAllPlayerHandPositions();

            Vector3 targetPosition = GameManager.Instance.GetHandPosition(hand);
            GameManager.Instance.uiManager.RequestCardValueUIPositionUpdate_Register(targetPosition, newHandIndex);
            GameManager.Instance.uiManager.RequestPlayerInfoPositionUpdate_Register(targetPosition, newHandIndex);

            GameManager.Instance.uiManager.RequestCardValueUIPositionUpdate_Y_Register(newHandIndex);
            GameManager.Instance.uiManager.RequestPlayerInfoPositionUpdate_Y_Register(newHandIndex);
            
            UpdateUICardValue();
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
        GameManager.Instance.uiManager.RequestPlayerInfoPositionUpdate(targetPosition, handIndex);

        GameManager.Instance.uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
    }
}
