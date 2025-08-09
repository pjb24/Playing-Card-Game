using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDealerCardDealtCompleteCommand : IGameCommand
{
    OnDealerCardDealtCommand _onDealerCardDealtCommand;

    public OnDealerCardDealtCompleteCommand(OnDealerCardDealtCommand command)
    {
        _onDealerCardDealtCommand = command;
    }

    public IEnumerator Execute(string payload)
    {
        OnDealerCardDealtCompleteDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerCardDealtCompleteDTO>(payload);

        Debug.Log("OnDealerCardDealtComplete, " + "딜러의 행동이 완료되었습니다.");

        while (_onDealerCardDealtCommand.Queue.Count != 0)
        {
            yield return null;
        }

        DealerBehaviorDoneDTO dealerBehaviorDoneDTO = new DealerBehaviorDoneDTO();
        string dealerBehaviorDoneJson = Newtonsoft.Json.JsonConvert.SerializeObject(dealerBehaviorDoneDTO);
        NetworkManager.Instance.SignalRClient.Execute("DealerBehaviorDone", dealerBehaviorDoneJson);

        yield return null;
    }
}
