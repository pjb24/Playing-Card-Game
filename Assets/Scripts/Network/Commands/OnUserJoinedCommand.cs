using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnUserJoinedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnUserJoinedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnUserJoinedDTO>(payload);

        Debug.Log("OnUserJoined, " + "������ ���ӿ� �����Ͽ����ϴ�." + " ���� �̸�: " + dto.userName);
    }
}
