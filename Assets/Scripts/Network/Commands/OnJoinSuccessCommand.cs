using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnJoinSuccessCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnJoinSuccessDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnJoinSuccessDTO>(payload);

        Debug.Log("OnJoinSuccess, " + "게임에 입장하였습니다." + "유저 이름: " + dto.userName + " PlayerGuid: " + dto.playerGuid);

        Player player = new Player(dto.playerGuid, dto.userName);
        GameManager.Instance.characterManager.AddPlayer(player);

        GameManager.Instance.characterManager.SetClientPlayer(player);

        WorkForUI();

        yield return null;
    }

    private void WorkForUI()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.uiManager.ChangeToBetPanel();

            GameManager.Instance.uiManager.button_Join.visible = false;
        });
    }
}
