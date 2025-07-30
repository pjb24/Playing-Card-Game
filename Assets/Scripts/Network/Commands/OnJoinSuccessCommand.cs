using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnJoinSuccessCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnJoinSuccessDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnJoinSuccessDTO>(payload);

        Debug.Log("OnJoinSuccess, " + "게임에 입장하였습니다." + "유저 이름: " + dto.userName + " PlayerGuid: " + dto.playerGuid);
    }
}
