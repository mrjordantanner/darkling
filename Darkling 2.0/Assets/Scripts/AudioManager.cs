using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    #region Singleton
    public static AudioManager Instance;

    private void Awake()
    {

        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    bool fadingMusicOut, fadingMusicDown, fadingMusicUp;
    public float fastMusicFadeSpeed = 0.25f, slowMusicFadeSpeed = 2.0f;

    public Sound[] sounds;

    public AudioSource musicSource;
    public float minMusicVolume = 0.3f;
    public float maxMusicVolume = 0.6f;

    public AudioSource[] allAudioSources;
   // List<AudioSource> musicAudioSources = new List<AudioSource>();
    List<AudioSource> ambientAudioSources = new List<AudioSource>();

    public AudioMixerGroup musicVolumeMixer, ambientVolumeMixer, sfxVolumeMixer;

    private void Start()
    {
        // Music and Ambient Noise
        allAudioSources = GetComponents<AudioSource>();

        foreach (var source in allAudioSources)
        {
            //if (source.outputAudioMixerGroup == musicVolumeMixer)
            //{
            //    musicAudioSources.Add(source);
            //}

            if (source.outputAudioMixerGroup == ambientVolumeMixer)
            {
                ambientAudioSources.Add(source);
            }

        }

        // SFX
        foreach (Sound s in sounds)
        {
            s.source = Instance.gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = sfxVolumeMixer;
        }
    }

    public IEnumerator PlayWithDelay(string track, float delay)
    {
        yield return new WaitForSeconds(delay);
        Play(track);
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.PlayOneShot(s.clip);
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Stop();
    }

    public void StopAll()
    {
        var allSounds = GetComponents<AudioSource>();
        foreach (var sound in allSounds)
        {
            sound.Stop();
        }

    }


    public void ReduceMusicVolume()
    {
        //foreach (var source in musicAudioSources)
        //{
        //    source.volume = 0.5f;
        //}

        musicSource.volume = minMusicVolume;
    }

    public void RestoreMusicVolume()
    {
        //foreach (var source in musicAudioSources)
        //{
        //    source.volume = 1f;
        //}

        musicSource.volume = maxMusicVolume;

    }

    public void StartMusic()
    {
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void FadeMusicDown()
    {
        fadingMusicDown = true;
        fadingMusicUp = false;
        fadingMusicOut = false;

    }

    public void FadeMusicUp()
    {
        fadingMusicDown = false;
        fadingMusicUp = true;
        fadingMusicOut = false;
    }

    public void FadeMusicOut()
    {
        fadingMusicDown = false;
        fadingMusicUp = false;
        fadingMusicOut = true;
    }


    private void Update()
    {

        if (fadingMusicUp)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, maxMusicVolume, fastMusicFadeSpeed * Time.unscaledDeltaTime);
        }

        if (fadingMusicDown)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, minMusicVolume, fastMusicFadeSpeed * Time.unscaledDeltaTime);
        }

        if (fadingMusicOut)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, 0f, slowMusicFadeSpeed * Time.unscaledDeltaTime);
        }



    }

}
