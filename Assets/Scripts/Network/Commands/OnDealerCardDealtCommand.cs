using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDealerCardDealtCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnDealerCardDealtDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerCardDealtDTO>(payload);

        Debug.Log("OnDealerCardDealt, " + "�������� ī��: " + dto.cardString + "��/�� �й��մϴ�.");
    }
}
