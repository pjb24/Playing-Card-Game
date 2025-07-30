using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTimeToActionCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnTimeToActionDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnTimeToActionDTO>(payload);

        Debug.Log("OnTimeToAction, " + "플레이어: " + dto.playerName + "의 " + "핸드 ID: " + dto.handId + "이/가 동작을 수행할 차례입니다." + " 플레이어 Guid: " + dto.playerGuid);
    }
}
