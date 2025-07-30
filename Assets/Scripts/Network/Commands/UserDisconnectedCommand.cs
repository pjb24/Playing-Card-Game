using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDisconnectedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        UserDisconnectedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDisconnectedDTO>(payload);

        Debug.Log("UserDisconnected, " + "ConnectionId: " + dto.message + " 유저의 연결이 해제되었습니다.");
    }
}
