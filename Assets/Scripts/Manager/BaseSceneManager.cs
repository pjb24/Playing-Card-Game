using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSceneManager : MonoBehaviour
{
    public abstract void InitManager();

    public virtual void ClearManager()
    {

    }
}
