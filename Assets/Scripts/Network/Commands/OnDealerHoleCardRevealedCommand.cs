using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDealerHoleCardRevealedCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnDealerHoleCardRevealedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerHoleCardRevealedDTO>(payload);

        Debug.Log("OnDealerHoleCardRevealed, " + "딜러의 숨겨진 카드: " + dto.cardRank + " of " + dto.cardSuit + "을/를 공개합니다.");

        GameManager.Instance.HandleOnDealerHoleCardRevealedMessage(dto);

        yield return null;
    }
}
