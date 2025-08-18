using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAddCardToDealerHandCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnAddCardToDealerHandDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnAddCardToDealerHandDTO>(payload);

        Debug.Log("OnAddCardToDealerHand, " + "딜러의 핸드에 " + dto.cardRank + " of " + dto.cardSuit + "를 추가합니다.");

        GameManager.Instance.HandleOnAddCardToDealerHandMessage(dto);

        yield return null;
    }
}
