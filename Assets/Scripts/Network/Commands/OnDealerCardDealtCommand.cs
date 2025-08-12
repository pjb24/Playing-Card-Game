using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDealerCardDealtCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnDealerCardDealtDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerCardDealtDTO>(payload);

        Debug.Log("OnDealerCardDealt, " + "딜러에게 카드: " + dto.cardRank + " of " + dto.cardSuit + "을/를 분배합니다.");

        GameManager.Instance.HandleOnDealerCardDealtMessage(dto);

        yield return null;
    }
}
