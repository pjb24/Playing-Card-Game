using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHandSplitCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnHandSplitDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnHandSplitDTO>(payload);

        Debug.Log("OnHandSplit, " + "�÷��̾�: " + dto.playerName + "�� " + "�ڵ� ID: " + dto.handId + "�� Split�Ͽ� ���ڵ� ID: " + dto.newHandId + "�� �����մϴ�.");
    }
}
