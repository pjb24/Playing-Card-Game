using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerHand : Hand
{
    // 소프트 17일 때 히트 할지 여부
    public bool DealerHitOnSoftCount => true;

    public int SoftCount = 17;

    // 딜러가 히트해야하는지 확인
    public bool ShouldHit()
    {
        int value = GetValue();

        if (value < SoftCount)
        {
            return true;
        }

        if (value == SoftCount && DealerHitOnSoftCount)
        {
            return IsSoftCount();
        }

        return false;
    }

    // 핸드가 Soft 17 인지 확인
    // Ace를 11로 사용한 17을 의미
    public bool IsSoftCount()
    {
        int total = 0;
        int aceCount = 0;

        foreach (var card in Cards)
        {
            int value = card.GetValue();
            total += value;

            if (card.Rank == E_CardRank.Ace)
            {
                aceCount++;
            }
        }

        return total == SoftCount && aceCount > 0;
    }
}
