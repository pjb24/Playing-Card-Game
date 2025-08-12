using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnJoinLobbySuccessCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnJoinLobbySuccessDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnJoinLobbySuccessDTO>(payload);

        Debug.Log("OnJoinLobbySuccess, " + "로비에 입장하였습니다." + " PlayerGuid: " + dto.playerGuid);

        GameManager.Instance.HandleOnJoinLobbySuccessMessage(dto);

        yield return null;
    }
}
