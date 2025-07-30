using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerBustedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnPlayerBustedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnPlayerBustedDTO>(payload);

        Debug.Log("OnPlayerBusted, " + "�÷��̾�: " + dto.playerName + "�� " + "�ڵ� ID: " + dto.handId + "��/�� Bust �Ǿ����ϴ�." + " �÷��̾� Guid: " + dto.playerGuid);
    }
}
