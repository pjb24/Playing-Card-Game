using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_ChipType
{
    Green,
    Red,
    Blue,
    Purple,
    Black,
}

public enum E_ChipValue
{
    Bet1 = 500,
    Bet2 = 2_000,
    Bet3 = 10_000,
    Bet4 = 50_000,
    BetMax = 250_000,
}

public class ChipView : MonoBehaviour
{
    public int ChipValue;
    public E_ChipType chipType;

    public void SetValue(int value)
    {
        ChipValue = value;
    }
}
