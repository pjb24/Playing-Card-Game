using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnUserJoinedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnUserJoinedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnUserJoinedDTO>(payload);

        Debug.Log("OnUserJoined, " + "유저가 게임에 입장하였습니다." + " 유저 이름: " + dto.userName);
    }
}
