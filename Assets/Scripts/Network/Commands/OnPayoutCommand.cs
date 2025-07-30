using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPayoutCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnPayoutDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnPayoutDTO>(payload);

        Debug.Log("OnPayout, " + "�ڵ� ID: " + dto.handId + "�� " + "���: " + dto.evaluationResult + "�� ������ �����մϴ�.");
    }
}
