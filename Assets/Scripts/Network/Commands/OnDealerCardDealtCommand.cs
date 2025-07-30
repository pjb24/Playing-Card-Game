using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDealerCardDealtCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnDealerCardDealtDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerCardDealtDTO>(payload);

        Debug.Log("OnDealerCardDealt, " + "딜러에게 카드: " + dto.cardString + "을/를 분배합니다.");
    }
}
