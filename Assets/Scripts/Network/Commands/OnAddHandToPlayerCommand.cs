using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAddHandToPlayerCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnAddHandToPlayerDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnAddHandToPlayerDTO>(payload);

        Debug.Log("OnAddHandToPlayer, " + "플레이어 Guid: " + dto.playerGuid + "에 " + "핸드 ID: " + dto.handId + "를 추가합니다.");

        GameManager.Instance.HandleOnAddHandToPlayerMessage(dto);

        yield return null;
    }
}
