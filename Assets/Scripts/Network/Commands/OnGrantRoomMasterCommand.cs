using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using UnityEngine;

public class OnGrantRoomMasterCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnGrantRoomMasterDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnGrantRoomMasterDTO>(payload);

        Debug.Log("OnGrantRoomMaster, " + "당신이 룸마스터로 설정되었습니다.");

        WorkForUI();

        yield return null;
    }

    private void WorkForUI()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
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