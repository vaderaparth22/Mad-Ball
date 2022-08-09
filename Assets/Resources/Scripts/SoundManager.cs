using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundManager
{
    public AudioClip safeHitSound;
    public AudioClip[] hitWallSounds;
    public AudioClip enemiesHitSound;
    public AudioClip playerExplodeSound;

    AudioSource soundEffectObject;

    public void Initialize()
    {
        soundEffectObject = GameObject.FindObjectOfType<AudioSource>();
    }

    public void PlaySafeHitSound()
    {
        soundEffectObject.PlayOneShot(safeHitSound, soundEffectObject.volume);
    }

    public void PlayWallHitSound()
    {
        soundEffectObject.PlayOneShot(hitWallSounds[GetRandomInteger(0, hitWallSounds.Length-1)], 0.8f);
    }

    public void PlayEnemiesHitSound()
    {
        soundEffectObject.PlayOneShot(enemiesHitSound, soundEffectObject.volume);
    }

    public void PlayExplosionSound()
    {
        soundEffectObject.PlayOneShot(playerExplodeSound, soundEffectObject.volume);
    }

    int GetRandomInteger(int from, int to)
    {
        return Random.Range(from, to);
    }
}
