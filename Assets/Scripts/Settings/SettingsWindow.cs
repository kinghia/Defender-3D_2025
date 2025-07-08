using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] public GameObject settingsPanel;
    [SerializeField] public Button openSettingsButton;
    [SerializeField] public Button closeSettingsButton;
    [SerializeField] public Button resetToDefaultsButton;
    
    [Header("Settings")]
    [SerializeField] private SettingsManager settingsManager;
    
    void Start()
    {
        if (settingsManager == null)
            settingsManager = FindFirstObjectByType<SettingsManager>();
            
        // Set up button listeners
        if (openSettingsButton != null)
            openSettingsButton.onClick.AddListener(OpenSettings);
            
        if (closeSettingsButton != null)
            closeSettingsButton.onClick.AddListener(CloseSettings);
            
        if (resetToDefaultsButton != null)
            resetToDefaultsButton.onClick.AddListener(ResetToDefaults);
            
        // Initially hide settings panel
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
    
    void Update()
    {
        
    }
    
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }
    
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsManager.SaveSettings();
            settingsPanel.SetActive(false);
        }
    }
    
    public void ResetToDefaults()
    {
        if (settingsManager != null)
        {
            settingsManager.ResetToDefaults();
        }
    }
    
    // Method to toggle settings window
    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            if (settingsPanel.activeSelf)
                CloseSettings();
            else
                OpenSettings();
        }
    }
} 