using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : Item
{
    public override ItemType Type => ItemType.Key;
    public override void Activate()
    {
        SoundManager.Instance.PlaySoundEffect(SoundEffect.KeyPickUp);
    }
    public override void Deactivate() { }
}
