using TMPro;
using UnityEngine.UI;

public class OptionWindow : ViewBase
{
    public Slider musicSlider;
    public Slider audioSlider;
    public TextMeshProUGUI musicVolumeLabel;
    public TextMeshProUGUI audioVolumeLabel;

    private void Start()
    {
        float musicVolume = GameData.Instance.data.musicVolume;
        musicSlider.value = musicVolume;
        musicSlider.onValueChanged.AddListener(MusicValueChange);
        UpdateMusicLabel(musicVolume);

        float soundVolume = GameData.Instance.data.soundVolume;
        audioSlider.value = soundVolume;
        audioSlider.onValueChanged.AddListener(AudioValueChange);
        UpdateAudioLabel(soundVolume);
    }

    public void ClickBack()
    {
        Close();
    }

    private void UpdateMusicLabel(float value)
    {
        musicVolumeLabel.text = (value * 100).ToString("F0") + "%";
    }

    private void UpdateAudioLabel(float value)
    {
        audioVolumeLabel.text = (value * 100).ToString("F0") + "%";
    }

    private void MusicValueChange(float value)
    {
        GameData.Instance.SetMusicVolume(value);
        AudioSystem.Instance.onMusicVolumeChanged?.Invoke();
        UpdateMusicLabel(value);
    }

    private void AudioValueChange(float value)
    {
        GameData.Instance.SetSoundVolume(value);
        AudioSystem.Instance.onSoundVolumeChanged?.Invoke();
        UpdateAudioLabel(value);
    }
}