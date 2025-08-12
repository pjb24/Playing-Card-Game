using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDealerHiddenCardDealtCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnDealerHiddenCardDealtDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerHiddenCardDealtDTO>(payload);

        Debug.Log("OnDealerHiddenCardDealt, " + "딜러에게 숨겨진 카드를 분배합니다.");

        GameManager.Instance.HandleOnDealerHiddenCardDealtMessage(dto);
        
        yield return null;
    }
}
