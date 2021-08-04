using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oil : Item
{
    const float EFFECTIVENESS = 100f; // percentage of the total time to add
    public override ItemType Type => ItemType.Oil;

    public override void Activate()
    {
        Player.Instance.SubtractTime(power: -EFFECTIVENESS);
        SoundManager.Instance.PlaySoundEffect(SoundEffect.OilDrumPickUp);
    }

    public override void Deactivate()
    {
        Player.Instance.SubtractTime(power: -EFFECTIVENESS);
    }
}
