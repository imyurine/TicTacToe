using UnityEngine;

/// bgm播放器
public class BgmPlayer : MonoBehaviour
{
    public string audioId;
    private AudioSource auBgm;

    private void Awake()
    {
        auBgm = GetComponentInChildren<AudioSource>();
        AudioSystem.Instance.onMusicVolumeChanged += OnMusicVolumeChanged;
        OnMusicVolumeChanged();
    }

    private void OnEnable()
    {
        Play();
    }

    private void OnDestroy()
    {
        AudioSystem.Instance.onMusicVolumeChanged -= OnMusicVolumeChanged;
    }

    public void OnMusicVolumeChanged()
    {
        auBgm.volume = GameData.Instance.data.musicVolume;
    }

    public void Play()
    {
        if (string.IsNullOrEmpty(audioId))
        {
            return;
        }

        AudioClip clip = AudioSystem.Instance.GetAudioClip(audioId);
        if (clip != null)
        {
            auBgm.clip = clip;
            auBgm.loop = true;
            auBgm.Play();
        }
    }
}