using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [Header("make sure to set the minimum value of \nthe sliders to 0.0001 to avoid log10(0)!!!")]
    [Header("UI Elements")]
    [SerializeField] private Slider m_generalSlider;
    [SerializeField] private Slider m_sfxSlider;
    [SerializeField] private Slider m_musicSlider;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer m_audioMixer;

    const string GENERAL_VOLUME_PARAM = "GeneralVolume";
    const string SFX_VOLUME_PARAM = "SFXVolume";
    const string MUSIC_VOLUME_PARAM = "MusicVolume";

    private void Awake()
    {
        m_generalSlider.onValueChanged.AddListener(SetGeneralVolume);
        m_sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        m_musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    void SetGeneralVolume(float _value)
    {
        m_audioMixer.SetFloat(GENERAL_VOLUME_PARAM, Mathf.Log10(_value) * 20.0f);
    }

    void SetSFXVolume(float _value)
    {
        m_audioMixer.SetFloat(SFX_VOLUME_PARAM, Mathf.Log10(_value) * 20.0f);
    }

    void SetMusicVolume(float _value)
    {
        m_audioMixer.SetFloat(MUSIC_VOLUME_PARAM, Mathf.Log10(_value) * 20.0f);
    }
}
