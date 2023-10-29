using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SoundManager : MonoSingleton<SoundManager>
{
    private AudioSource audioSound;
    private Dictionary<string, AudioSource> dicAudioLoops;

    public AudioClip sndButton;
    public bool offSound;

    private const string OffSoundKey = "OffSound";
    public UnityEvent<bool> onSoundChanged;

    private float volume;

    protected override void Awake()
    {
        base.Awake();
        Debug.LogError(gameObject.name);

        GameObject sound = new GameObject();
        sound.transform.parent = this.transform;
        sound.transform.localPosition = Vector3.zero;
        sound.name = "Sound";
        this.audioSound = sound.AddComponent<AudioSource>();
        this.audioSound.loop = false;
        this.offSound = PlayerPrefs.GetInt(OffSoundKey, 0) == 0 ? false : true;
    }

    private void Start()
    {
        SetOffSound(this.offSound);
    }

    public void SetOffSound(bool _offSound)
    {
        this.offSound = _offSound;
        PlayerPrefs.SetInt(OffSoundKey, this.offSound ? 1 : 0);
        this.volume = this.offSound ? 0 : 1;

        if (this.dicAudioLoops != null)
        {
            foreach (var loop in dicAudioLoops.Values)
            {
                loop.volume = this.volume;
            }
        }

        this.audioSound.volume = volume;

        onSoundChanged?.Invoke(!this.offSound);
    }

    public void PlayOneShot(AudioClip clip)
    {
        this.audioSound.PlayOneShot(clip);
    }

    public void PlayButtonSound()
    {
        PlayOneShot(this.sndButton);
    }

    public AudioSource PlayLoop(AudioClip clip, float volume=1f)
    {
        if (this.dicAudioLoops == null) this.dicAudioLoops = new Dictionary<string, AudioSource>();
        if (!this.dicAudioLoops.ContainsKey(clip.name))
        {
            GameObject loop = new GameObject();
            loop.transform.parent = this.transform;
            loop.transform.localPosition = Vector3.zero;
            loop.name = "Loop " + clip.name;
            this.dicAudioLoops.Add(clip.name, loop.AddComponent<AudioSource>());
        }

        AudioSource audio = this.dicAudioLoops[clip.name];
        audio.loop = true;
        audio.clip = clip;
        audio.volume = volume;
        audio.Play();
        return audio;
    }

    public void PauseLoop(AudioClip clip)
    {
        if (this.dicAudioLoops != null)
        {
            if (this.dicAudioLoops.ContainsKey(clip.name))
            {
                AudioSource audio = this.dicAudioLoops[clip.name];
                audio.Pause();
            }
        }
    }
}
