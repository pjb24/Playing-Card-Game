using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_AspectOption
{
    None = 0,
    FixedHorizontal = 1,
    FixedVertical = 2,
}

public class FOVManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float defaultFOV = 60f;
    [SerializeField] private float targetAspect = 20f / 9f;

    [SerializeField] private E_AspectOption aspectOption = E_AspectOption.FixedHorizontal;

    private void Start()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        if (aspectOption == E_AspectOption.FixedHorizontal)
        {
            float scaleHeight = currentAspect / targetAspect;

            if (scaleHeight < 1.0f)
            {
                Rect rect = cam.rect;
                rect.width = 1.0f;
                rect.height = scaleHeight;
                rect.x = 0;
                rect.y = (1.0f - scaleHeight) / 2.0f;
                cam.rect = rect;
            }
            else
            {
                Rect rect = cam.rect;
                rect.width = 1.0f / scaleHeight;
                rect.height = 1.0f;
                rect.x = (1.0f - rect.width) / 2.0f;
                rect.y = 0;
                cam.rect = rect;
            }
        }
        
        if (aspectOption == E_AspectOption.FixedVertical)
        {
            float aspectRatioFactor = targetAspect / currentAspect;

            cam.fieldOfView = 2f * Mathf.Atan(Mathf.Tan(defaultFOV * Mathf.Deg2Rad / 2f) * aspectRatioFactor) * Mathf.Rad2Deg;
        }
    }
}
