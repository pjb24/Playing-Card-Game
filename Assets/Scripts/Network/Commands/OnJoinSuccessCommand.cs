using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnJoinSuccessCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnJoinSuccessDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnJoinSuccessDTO>(payload);

        Debug.Log("OnJoinSuccess, " + "���ӿ� �����Ͽ����ϴ�." + "���� �̸�: " + dto.userName + " PlayerGuid: " + dto.playerGuid);
    }
}
