using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject adjustPanel;

    [SerializeField]
    private Slider bgmSlider;

    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bgmSlider.value = volume;
    }

    public void StartNewGame()
    {
        Time.timeScale = 1f;
        Settings.fromSave = false;
        SceneManager.LoadScene("Loading");
    }

    public void LoadSavedGame()
    {
        if (PlayerPrefs.GetInt("HasSaveGame", 0) == 1)
        {
            Time.timeScale = 1f;
            Settings.fromSave = true;
            SceneManager.LoadScene("Loading");
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowHideAdjustPanel(bool show)
    {
        adjustPanel.SetActive(show);
    }

    public void SetVolume(float volume)
    {
        AudioManager.instance.AdjustMasterVolume(volume);
    }
}