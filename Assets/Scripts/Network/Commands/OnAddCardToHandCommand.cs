using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAddCardToHandCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnAddCardToHandDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnAddCardToHandDTO>(payload);

        Debug.Log("OnAddCardToHand, " + "플레이어 Guid: " + dto.playerGuid + "의 " + "핸드 ID: " + dto.handId + "에 " + dto.cardRank + " of " + dto.cardSuit + "를 추가합니다.");

        GameManager.Instance.HandleOnAddCardToHandMessage(dto);

        yield return null;
    }
}
