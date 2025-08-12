using System;
using UnityEngine;

/// 用于播放音频
public class AudioSystem : MonoBehaviour
{
    public static AudioSystem Instance = null;

    public AudioSource soundAudio;

    public Action onMusicVolumeChanged;
    public Action onSoundVolumeChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        onSoundVolumeChanged += OnSoundVolumeChanged;
        OnSoundVolumeChanged();
    }

    public void OnSoundVolumeChanged()
    {
        soundAudio.volume = GameData.Instance.data.soundVolume;
    }

    /// 播放音效
    public void PlayAudio(string audioId)
    {
        if (string.IsNullOrEmpty(audioId))
        {
            return;
        }

        AudioClip clip = GetAudioClip(audioId);
        if (clip != null)
        {
            soundAudio.PlayOneShot(clip);
        }
    }

    public AudioClip GetAudioClip(string audioId)
    {
        return Resources.Load<AudioClip>("Audios/" + audioId);
    }
}