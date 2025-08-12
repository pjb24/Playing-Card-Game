using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerRemainChipsCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnPlayerRemainChipsDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnPlayerRemainChipsDTO>(payload);

        Debug.Log("OnPlayerRemainChips, " + "플레이어 Guid: " + dto.playerGuid + "가 소지한 칩: " + dto.chips);

        GameManager.Instance.HandleOnPlayerRemainChipsMessage(dto);

        yield return null;
    }
}
