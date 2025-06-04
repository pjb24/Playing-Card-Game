using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
    public enum FlipDirection
    {
        Left,
        Right,
        Top,
        Bottom,
    };

    public FlipDirection flipDirection = FlipDirection.Left;
    public float duration = 0.5f;

    [ContextMenu("Test Flip Function")]
    private void TestFlipFunction()
    {
        Flip(flipDirection);
    }

    private void Flip(FlipDirection direction = FlipDirection.Right)
    {
        Quaternion currentRotation = transform.rotation;

        Vector3 angle3 = new Vector3(0, 0, 0);

        switch (direction)
        {
            case FlipDirection.Left:
                angle3.y = -180f;
                break;
            case FlipDirection.Right:
                angle3.y = 180f;
                break;
            case FlipDirection.Top:
                angle3.x = -180f;
                break;
            case FlipDirection.Bottom:
                angle3.x = 180f;
                break;
        }

        Quaternion rotationDelta = Quaternion.Euler(angle3);

        Quaternion targetRotation = currentRotation * rotationDelta;

        transform.DORotateQuaternion(targetRotation, duration).SetEase(Ease.InOutQuad);
    }
}
