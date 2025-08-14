using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserLeftCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        UserLeftDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<UserLeftDTO>(payload);

        Debug.Log("UserLeft, " + "Player Guid: " + dto.playerGuid + " 유저가 게임에서 퇴장하였습니다.");

        GameManager.Instance.HandleUserLeftMessage(dto);

        yield return null;
    }
}
