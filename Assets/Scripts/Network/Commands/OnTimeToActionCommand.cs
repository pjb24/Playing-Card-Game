using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTimeToActionCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnTimeToActionDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnTimeToActionDTO>(payload);

        Debug.Log("OnTimeToAction, " + "�÷��̾�: " + dto.playerName + "�� " + "�ڵ� ID: " + dto.handId + "��/�� ������ ������ �����Դϴ�." + " �÷��̾� Guid: " + dto.playerGuid);
    }
}
