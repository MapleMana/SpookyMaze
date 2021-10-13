using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    public List<MusicDefinition> Songs;
    public AudioSource MusicSource;

    public void PlayMusic(Music song)
    {
        AudioClip selectedSong = Songs.Find(s => s.music == song).Clip;
        if (!MusicSource.isPlaying || MusicSource.clip != selectedSong)
        {
            MusicSource.clip = selectedSong;
            MusicSource.Play();
        }        
    }

    public void StopMusic()
    {
        MusicSource.Stop();
    }

    private void Start()
    {
        PlayMusic(Music.MenuMusic);
        MusicSource.volume = PlayerPrefs.GetFloat("musicVol", 0.5f);
    }
}
