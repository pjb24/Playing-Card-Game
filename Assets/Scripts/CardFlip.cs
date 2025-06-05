using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CardFlip : MonoBehaviour
{
    // Flip 방향 enum
    public enum FlipDirection
    {
        Left,
        Right,
        Top,
        Bottom,
    };

    // Flip을 할 방향 지정
    public FlipDirection flipDirection = FlipDirection.Left;
    // Flip에 소요될 시간
    public float duration = 1f;

    // Flip 중복 동작 방지 플래그
    private bool isFlipping = false;

    // Inspector에서 테스트를 수행할 수 있음.
    [ContextMenu("Test Flip Function")]
    private void TestFlipFunction()
    {
        Flip(flipDirection);
    }

    // Flip 기능 구현 Right 방향 Flip이 기본 설정.
    private void Flip(FlipDirection direction = FlipDirection.Right)
    {
        if (isFlipping)
        {
            return;
        }
        isFlipping = true;

        // 현재 회전 쿼터니언
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

        // 추가 회전 쿼터니언
        Quaternion rotationDelta = Quaternion.Euler(angle3);

        // 목표 회전 = 현재 회전 * 추가 회전
        Quaternion targetRotation = currentRotation * rotationDelta;

        // DOTween으로 duration 동안 회전 수행.
        transform.DORotateQuaternion(targetRotation, duration)
            .SetEase(Ease.InOutQuad)
            // 회전이 끝나면 플래그 해제.
            .OnComplete(() => { isFlipping = false; });
    }
}
