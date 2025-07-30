using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDealerHiddenCardDealtCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnDealerHiddenCardDealtDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerHiddenCardDealtDTO>(payload);

        Debug.Log("OnDealerHiddenCardDealt, " + "�������� ������ ī�带 �й��մϴ�.");
    }
}
