using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerBustedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnPlayerBustedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnPlayerBustedDTO>(payload);

        Debug.Log("OnPlayerBusted, " + "플레이어: " + dto.playerName + "의 " + "핸드 ID: " + dto.handId + "이/가 Bust 되었습니다." + " 플레이어 Guid: " + dto.playerGuid);
    }
}
