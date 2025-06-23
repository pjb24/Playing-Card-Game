using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer
{
    public DealerHand Hand { get; private set; } = new();

    public bool ShouldHit()
    {
        return Hand.ShouldHit();
    }

    public void ResetHand()
    {
        Hand.Clear();
    }
}
