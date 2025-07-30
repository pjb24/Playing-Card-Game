using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDealerHoleCardRevealedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnDealerHoleCardRevealedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerHoleCardRevealedDTO>(payload);

        Debug.Log("OnDealerHoleCardRevealed, " + "������ ������ ī��: " + dto.cardString + "��/�� �����մϴ�.");
    }
}
