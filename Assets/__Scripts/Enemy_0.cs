using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_0 : Enemy
{
    public override int ScorePerKill
    {
        get { return base.ScorePerKill / 2; } // Override the getter, for example
        set { base.ScorePerKill = value; }    // You can override the setter as well if needed
    }

}
