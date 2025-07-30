using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnActionDoneCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnActionDoneDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnActionDoneDTO>(payload);

        Debug.Log("OnActionDone, " + "�÷��̾�: " + dto.playerName + "�� " + "�ڵ� ID: " + dto.handId + "�� Action�� �Ϸ��մϴ�." + " �÷��̾� Guid: " + dto.playerGuid);
    }
}
