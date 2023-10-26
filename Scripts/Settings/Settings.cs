using System;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

public class Settings : MonoBehaviour
{
    [SerializeField] private List<ResItem> _resolutionItems;
    [SerializeField] private List<AntiAliasingMode> _antialiasingItems;

    [SerializeField] private TextMeshProUGUI _resText;
    [SerializeField] private TextMeshProUGUI _antialiasingText;
    [SerializeField] private Toggle _motionBlurTog;
    [SerializeField] private Toggle _fullScreenTog;
    [SerializeField] private Toggle _fpsTog;

    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _effetsSlider;

    [SerializeField] private AudioMixerGroup _music;
    [SerializeField] private AudioMixerGroup _effects;

    private AntiAliasingMode _antialiasing;
    private ResItem _resolutionItem;
    private bool _isFirstStartGame = true;

    private int _currentResIndex;
    private int _currentModeIndex;

    private void Start()
    {
        LoadSettings();
        SetSettings();
    }


    private void SetSettings()
    {
        SetResolutionItem();
        SetAntiAliasingMode();

        Screen.fullScreen = _fullScreenTog.isOn;
        FullScreenMode fullScreen = _fullScreenTog.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(_resolutionItem.Width, _resolutionItem.Height, fullScreen);

        _music.audioMixer.SetFloat("MusicVolume", Mathf.Lerp(-80, 0, _musicSlider.value));
        _effects.audioMixer.SetFloat("EffectsVolume", Mathf.Lerp(-80, 0, _effetsSlider.value));
    }

    public void SaveSettings()
    {
        SetSettings();

        SettingsData data = new SettingsData();

        data.ResItem = _resolutionItem;
        data.AntiAliasing = _antialiasing;
        data.MotionBlur = _motionBlurTog.isOn;
        data.FullScreen = _fullScreenTog.isOn;
        data.MusicVolume = _musicSlider.value;
        data.EffectsVolume = _effetsSlider.value;
        data.FPS = _fpsTog.isOn;
        data.FirstStartGame = _isFirstStartGame;

        SettingsSaveLoadUtils.SaveSettingsData(data);
    }

    private void LoadSettings()
    {
        SettingsData data = SettingsSaveLoadUtils.LoadSettingsData();

        if (data == null)
        {
            SetDefaultValue();
            return;
        }

        _resolutionItem = data.ResItem;
        _antialiasing = data.AntiAliasing;
        _fullScreenTog.isOn = data.FullScreen;
        _motionBlurTog.isOn = data.MotionBlur;
        _musicSlider.value = data.MusicVolume;
        _effetsSlider.value = data.EffectsVolume;
        _isFirstStartGame = data.FirstStartGame;
        _fpsTog.isOn = data.FPS;

        _currentResIndex = _resolutionItems.IndexOf(_resolutionItem);
        _currentModeIndex = _antialiasingItems.IndexOf(_antialiasing);
    }

    public void OnResolutionButtonClick(bool isLeftButton)
    {
        if (isLeftButton && _currentResIndex > 0)
            _currentResIndex--;
        else if (!isLeftButton && _currentResIndex < _resolutionItems.Count - 1)
            _currentResIndex++;

        SetResolutionItem();
    }

    public void OnAntiAliasingButtonClick(bool isLeftButton)
    {
        if (isLeftButton && _currentModeIndex > 0)
            _currentModeIndex--;
        else if (!isLeftButton && _currentModeIndex < _antialiasingItems.Count - 1)
            _currentModeIndex++;

        SetAntiAliasingMode();
    }

    private void SetResolutionItem()
    {
        _resolutionItem = _resolutionItems[_currentResIndex];
        _resText.text = $"{_resolutionItem.Width}" + " x " + $"{_resolutionItem.Height}";
    }
    private void SetAntiAliasingMode()
    {
        _antialiasing = _antialiasingItems[_currentModeIndex];
        _antialiasingText.text = _antialiasing.Name;
    }

    private void SetDefaultValue()
    {
        _antialiasing = _antialiasingItems[1];
        _resolutionItem = _resolutionItems[_currentResIndex];
        _currentResIndex = 7;
        _fullScreenTog.isOn = true;
        _musicSlider.value = 0.5f;
        _effetsSlider.value = 0.5f;
        _fpsTog.isOn = false;
        _motionBlurTog.isOn = true;
        _isFirstStartGame = true;

        SetSettings(); 
    }
}

[Serializable]
public struct ResItem
{
    public int Width;
    public int Height;
}

[Serializable]
public struct AntiAliasingMode 
{
    public string Name;
    public PostProcessLayer.Antialiasing Mode; 
}
