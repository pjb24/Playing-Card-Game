using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDealerCardDealtCompleteCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnDealerCardDealtCompleteDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerCardDealtCompleteDTO>(payload);

        Debug.Log("OnDealerCardDealtComplete, " + "딜러의 행동이 완료되었습니다.");

        GameManager.Instance.HandleOnDealerCardDealtCompleteMessage(dto);

        yield return null;
    }
}
