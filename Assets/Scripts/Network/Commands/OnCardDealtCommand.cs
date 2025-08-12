using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCardDealtCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnCardDealtDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnCardDealtDTO>(payload);

        Debug.Log("OnCardDealt, " + "플레이어: " + dto.playerName + "의 " + "핸드 ID: " + dto.handId + "에 " + "카드 " + dto.cardRank + " of " + dto.cardSuit + "을/를 분배합니다." + " 플레이어 Guid: " + dto.playerGuid);

        GameManager.Instance.HandleOnCardDealtMessage(dto);

        yield return null;
    }
}
