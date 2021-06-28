using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public List<SoundFXDefinition> SoundFX;
    public AudioSource SoundFXSource;

    public void PlaySoundEffect(SoundEffect soundEffect)
    {
        AudioClip effect = SoundFX.Find(sfx => sfx.Effect == soundEffect).Clip;
        SoundFXSource.PlayOneShot(effect);
    }
}