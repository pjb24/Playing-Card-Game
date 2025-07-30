using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDisconnectedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        UserDisconnectedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<UserDisconnectedDTO>(payload);

        Debug.Log("UserDisconnected, " + "ConnectionId: " + dto.message + " ������ ������ �����Ǿ����ϴ�.");
    }
}
