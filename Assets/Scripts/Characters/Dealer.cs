using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer
{
    private DealerHand _hand = new();
    public DealerHand Hand => _hand;

    public bool ShouldHit()
    {
        return _hand.ShouldHit();
    }

    public void ResetHand()
    {
        _hand.Clear();
    }
}
