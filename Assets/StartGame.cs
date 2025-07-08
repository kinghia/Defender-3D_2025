using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    [SerializeField] GameObject Settings;

    void Start()
    {
        Settings.SetActive(false);
    }

    void Update()
    {
        
    }

    public void OnButtonSettings()
    {
        Settings.SetActive(true);
    }

    public void OnSaveSettings()
    {
        Settings.SetActive(false);
    }

    public void OnButtonPlay()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnButtonQuit()
    {
        Debug.Log("Da quit");
        Application.Quit();
    }
}
