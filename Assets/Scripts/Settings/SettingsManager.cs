using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private Dropdown displayModeDropdown;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown maxFpsDropdown;
    [SerializeField] private Toggle vSyncToggle;

    [Header("Game")]
    [SerializeField] private Dropdown languageDropdown;
    [SerializeField] private Toggle lockCursorToggle;
    [SerializeField] private Slider cameraShakeSlider;

    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake; // Reference to your camera shake system
    
    // PlayerPrefs Keys
    private const string DISPLAY_MODE_KEY = "DisplayMode";
    private const string RESOLUTION_KEY = "Resolution";
    private const string MAX_FPS_KEY = "MaxFPS";
    private const string VSYNC_KEY = "VSync";
    private const string LANGUAGE_KEY = "Language";
    private const string LOCK_CURSOR_KEY = "LockCursor";
    private const string CAMERA_SHAKE_KEY = "CameraShake";
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    private const string SOUND_VOLUME_KEY = "SoundVolume";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    
    // Resolution options
    private Resolution[] availableResolutions;
    private List<string> resolutionOptions = new List<string>();
    
    // Language options
    private string[] languageOptions = { "English", "Tiếng Việt" };
    
    private void Awake()
    {
        // Get available resolutions
        availableResolutions = Screen.resolutions;
        PopulateResolutionDropdown();
        
        // Load settings
        LoadSettings();
        
        // Apply settings to UI
        ApplySettingsToUI();
    }
    
    private void PopulateResolutionDropdown()
    {
        resolutionOptions.Clear();
        
        // Add common resolutions
        resolutionOptions.Add("1920x1080");
        resolutionOptions.Add("2560x1080");
        resolutionOptions.Add("2560x1440");
        resolutionOptions.Add("3440x1440");
        resolutionOptions.Add("Native");
        resolutionOptions.Add("640x480");
        resolutionOptions.Add("800x600");
        resolutionOptions.Add("1280x720");
        
        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resolutionOptions);
        }
    }
    
    private void LoadSettings()
    {
        // Load settings from PlayerPrefs with defaults
        PlayerPrefs.SetInt(DISPLAY_MODE_KEY, PlayerPrefs.GetInt(DISPLAY_MODE_KEY, 0));
        PlayerPrefs.SetInt(RESOLUTION_KEY, PlayerPrefs.GetInt(RESOLUTION_KEY, 0));
        PlayerPrefs.SetInt(MAX_FPS_KEY, PlayerPrefs.GetInt(MAX_FPS_KEY, 1)); // Default to 60 FPS
        PlayerPrefs.SetInt(VSYNC_KEY, PlayerPrefs.GetInt(VSYNC_KEY, 1)); // Default V-Sync on
        PlayerPrefs.SetInt(LANGUAGE_KEY, PlayerPrefs.GetInt(LANGUAGE_KEY, 0)); // Default English
        PlayerPrefs.SetInt(LOCK_CURSOR_KEY, PlayerPrefs.GetInt(LOCK_CURSOR_KEY, 1)); // Default locked
        PlayerPrefs.SetFloat(CAMERA_SHAKE_KEY, PlayerPrefs.GetFloat(CAMERA_SHAKE_KEY, 50f)); // Default 50%
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 100f)); // Default 100%
        PlayerPrefs.SetFloat(SOUND_VOLUME_KEY, PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, 100f)); // Default 100%
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 100f)); // Default 100%
    }
    
    private void ApplySettingsToUI()
    {
        if (displayModeDropdown != null)
            displayModeDropdown.value = PlayerPrefs.GetInt(DISPLAY_MODE_KEY, 0);
        
        if (resolutionDropdown != null)
            resolutionDropdown.value = PlayerPrefs.GetInt(RESOLUTION_KEY, 0);
        
        if (maxFpsDropdown != null)
            maxFpsDropdown.value = PlayerPrefs.GetInt(MAX_FPS_KEY, 1);
        
        if (vSyncToggle != null)
            vSyncToggle.isOn = PlayerPrefs.GetInt(VSYNC_KEY, 1) == 1;
        
        if (languageDropdown != null)
            languageDropdown.value = PlayerPrefs.GetInt(LANGUAGE_KEY, 0);
        
        if (lockCursorToggle != null)
            lockCursorToggle.isOn = PlayerPrefs.GetInt(LOCK_CURSOR_KEY, 1) == 1;
        
        if (cameraShakeSlider != null)
            cameraShakeSlider.value = PlayerPrefs.GetFloat(CAMERA_SHAKE_KEY, 50f);
        
        if (masterVolumeSlider != null)
            masterVolumeSlider.value = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 100f);
        
        if (soundVolumeSlider != null)
            soundVolumeSlider.value = PlayerPrefs.GetFloat(SOUND_VOLUME_KEY, 100f);
        
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 100f);
    }
    
    // Video Settings Methods
    public void OnDisplayModeChanged(int value)
    {
        PlayerPrefs.SetInt(DISPLAY_MODE_KEY, value);
        
        switch (value)
        {
            case 0: // Full Screen
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 1: // Windowed
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
        
        PlayerPrefs.Save();
    }
    
    public void OnResolutionChanged(int value)
    {
        PlayerPrefs.SetInt(RESOLUTION_KEY, value);
        
        string resolution = resolutionOptions[value];
        int width, height;
        
        switch (resolution)
        {
            case "1920x1080":
                width = 1920; height = 1080;
                break;
            case "2560x1080":
                width = 2560; height = 1080;
                break;
            case "2560x1440":
                width = 2560; height = 1440;
                break;
            case "3440x1440":
                width = 3440; height = 1440;
                break;
            case "Native":
                width = Screen.currentResolution.width;
                height = Screen.currentResolution.height;
                break;
            case "640x480":
                width = 640; height = 480;
                break;
            case "800x600":
                width = 800; height = 600;
                break;
            case "1280x720":
                width = 1280; height = 720;
                break;
            default:
                width = 1920; height = 1080;
                break;
        }
        
        Screen.SetResolution(width, height, Screen.fullScreenMode);
        PlayerPrefs.Save();
    }
    
    public void OnMaxFpsChanged(int value)
    {
        PlayerPrefs.SetInt(MAX_FPS_KEY, value);
        
        switch (value)
        {
            case 0: // Unlimited
                Application.targetFrameRate = -1;
                break;
            case 1: // 30 FPS
                Application.targetFrameRate = 30;
                break;
            case 2: // 60 FPS
                Application.targetFrameRate = 60;
                break;
            case 3: // 120 FPS
                Application.targetFrameRate = 120;
                break;
            case 4: // 144 FPS
                Application.targetFrameRate = 144;
                break;
        }
        
        PlayerPrefs.Save();
    }
    
    public void OnVSyncChanged(bool value)
    {
        PlayerPrefs.SetInt(VSYNC_KEY, value ? 1 : 0);
        QualitySettings.vSyncCount = value ? 1 : 0;
        PlayerPrefs.Save();
    }
    
    // Game Settings Methods
    public void OnLanguageChanged(int value)
    {
        PlayerPrefs.SetInt(LANGUAGE_KEY, value);
        // Implement your localization system here
        // Example: LocalizationManager.SetLanguage(languageOptions[value]);
        PlayerPrefs.Save();
    }
    
    public void OnLockCursorChanged(bool value)
    {
        PlayerPrefs.SetInt(LOCK_CURSOR_KEY, value ? 1 : 0);
        
        if (value)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        PlayerPrefs.Save();
    }
    
    public void OnCameraShakeChanged(float value)
    {
        PlayerPrefs.SetFloat(CAMERA_SHAKE_KEY, value);
        
        if (cameraShake != null)
        {
            cameraShake.SetIntensity(value / 100f); // Convert percentage to 0-1 range
        }
        
        PlayerPrefs.Save();
    }
    
    // Audio Settings Methods
    public void OnMasterVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, value);
        
        if (audioMixer != null)
        {
            float volume = value > 0 ? Mathf.Log10(value / 100f) * 20f : -80f;
            audioMixer.SetFloat("MasterVolume", volume);
        }
        
        PlayerPrefs.Save();
    }
    
    public void OnSoundVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(SOUND_VOLUME_KEY, value);
        
        if (audioMixer != null)
        {
            float volume = value > 0 ? Mathf.Log10(value / 100f) * 20f : -80f;
            audioMixer.SetFloat("SoundVolume", volume);
        }
        
        PlayerPrefs.Save();
    }
    
    public void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        
        if (audioMixer != null)
        {
            float volume = value > 0 ? Mathf.Log10(value / 100f) * 20f : -80f;
            audioMixer.SetFloat("MusicVolume", volume);
        }
        
        PlayerPrefs.Save();
    }
    
    // Public method to reset all settings to defaults
    public void ResetToDefaults()
    {
        PlayerPrefs.DeleteAll();
        LoadSettings();
        ApplySettingsToUI();
        
        // Apply default settings
        OnDisplayModeChanged(0);
        OnResolutionChanged(0);
        OnMaxFpsChanged(1);
        OnVSyncChanged(true);
        OnLanguageChanged(0);
        OnLockCursorChanged(false);
        OnCameraShakeChanged(50f);
        OnMasterVolumeChanged(100f);
        OnSoundVolumeChanged(100f);
        OnMusicVolumeChanged(100f);
    }

    public void SaveSettings()
    {
        // Lưu settings từ UI vào PlayerPrefs
        PlayerPrefs.SetInt(DISPLAY_MODE_KEY, displayModeDropdown.value);
        PlayerPrefs.SetInt(RESOLUTION_KEY, resolutionDropdown.value);
        PlayerPrefs.SetInt(MAX_FPS_KEY, maxFpsDropdown.value);
        PlayerPrefs.SetInt(VSYNC_KEY, vSyncToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt(LANGUAGE_KEY, languageDropdown.value);
        PlayerPrefs.SetInt(LOCK_CURSOR_KEY, lockCursorToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat(CAMERA_SHAKE_KEY, cameraShakeSlider.value);
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolumeSlider.value);
        PlayerPrefs.SetFloat(SOUND_VOLUME_KEY, soundVolumeSlider.value);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolumeSlider.value);
        PlayerPrefs.Save();

        // Áp dụng ngay lập tức các thay đổi
        OnDisplayModeChanged(displayModeDropdown.value);
        OnResolutionChanged(resolutionDropdown.value);
        OnMaxFpsChanged(maxFpsDropdown.value);
        OnVSyncChanged(vSyncToggle.isOn);
        OnLanguageChanged(languageDropdown.value);
        OnLockCursorChanged(lockCursorToggle.isOn);
        OnCameraShakeChanged(cameraShakeSlider.value);
        OnMasterVolumeChanged(masterVolumeSlider.value);
        OnSoundVolumeChanged(soundVolumeSlider.value);
        OnMusicVolumeChanged(musicVolumeSlider.value);
    }
}

// Simple CameraShake class for reference (you can replace with your existing system)
[System.Serializable]
public class CameraShake
{
    public void SetIntensity(float intensity)
    {
        // Implement your camera shake intensity setting here
        Debug.Log($"Camera shake intensity set to: {intensity}");
    }
} 