using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBetPlacedCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnBetPlacedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnBetPlacedDTO>(payload);

        Debug.Log("OnBetPlaced, " + "플레이어 " + dto.playerName + "이/가 핸드 ID: " + dto.handId + "에 " + dto.betAmount + "를 베팅하였습니다." + " 플레이어 Guid: " + dto.playerGuid);

        GameManager.Instance.HandleOnBetPlacedMessage(dto);

        yield return null;
    }
}
