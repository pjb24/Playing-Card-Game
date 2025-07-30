using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserConnectedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        UserConnectedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<UserConnectedDTO>(payload);

        Debug.Log("UserConnected, " + "ConnectionId: " + dto.message + " 유저가 연결되었습니다.");
    }
}
