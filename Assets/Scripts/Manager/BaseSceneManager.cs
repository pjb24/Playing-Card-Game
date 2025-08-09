using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSceneManager : MonoBehaviour
{
    protected virtual void Start()
    {
        // GameManager가 존재하고, 이벤트에 등록하여 씬이 준비되었을 때 초기화 로직을 실행
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSceneLoaded += InitializeScene;
        }
    }

    protected virtual void OnDestroy()
    {
        // 씬이 파괴될 때 이벤트에서 등록 해제
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSceneLoaded -= InitializeScene;
        }
    }

    protected abstract void InitializeScene();
}
