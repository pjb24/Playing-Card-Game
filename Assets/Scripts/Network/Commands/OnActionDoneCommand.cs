using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnActionDoneCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnActionDoneDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnActionDoneDTO>(payload);

        Debug.Log("OnActionDone, " + "플레이어: " + dto.playerName + "의 " + "핸드 ID: " + dto.handId + "의 Action을 완료합니다." + " 플레이어 Guid: " + dto.playerGuid);
    }
}
