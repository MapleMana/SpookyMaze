using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : Item
{
    const float EFFECTIVENESS = 0.3f; // percentage of the total time to add
    public override ItemType Type => ItemType.Oil;

    public override void Activate()
    {
        LevelManager.Instance.AddTime(ratio: EFFECTIVENESS);
    }

    public override void Deactivate()
    {
        LevelManager.Instance.AddTime(ratio: -EFFECTIVENESS);
    }
}
