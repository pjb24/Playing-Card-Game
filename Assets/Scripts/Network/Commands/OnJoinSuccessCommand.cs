using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnJoinSuccessCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnJoinSuccessDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnJoinSuccessDTO>(payload);

        Debug.Log("OnJoinSuccess, " + "게임에 입장하였습니다." + "유저 이름: " + dto.userName + " PlayerGuid: " + dto.playerGuid);

        GameManager.Instance.characterManager.AddPlayer(new Player(dto.playerGuid, dto.userName));

        WorkForUI();
    }

    private void WorkForUI()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.uiManager.ChangeToBetPanel();

            GameManager.Instance.uiManager.button_Join.visible = true;
            GameManager.Instance.uiManager.button_Join.clicked += HandleJoin;
        });
    }

    private void HandleJoin()
    {
        StartGameDTO startGameDTO = new StartGameDTO();
        string startGameJson = Newtonsoft.Json.JsonConvert.SerializeObject(startGameDTO);
        GameManager.Instance.SignalRClient.Execute("StartGame", startGameJson);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.uiManager.button_Join.clicked -= HandleJoin;
        });
    }
}
