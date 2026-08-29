using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("BGM")]
    [SerializeField]
    private AudioClip[] bgm;

    [SerializeField]
    private AudioSource bgmSource;

    private int currentBGM = -1;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        LoadCurrentVolume();
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    private void PlayBGMForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Title":
                PlayBGM(0);
                break;

            case "Scene01":
                PlayBGM(1);
                break;

            case "Loading":
                break;
        }
    }

    public void PlayBGM(int index)
    {
        if (bgm == null || bgm.Length == 0)
        {
            return;
        }

        if (index < 0 || index >= bgm.Length)
        {
            return;
        }

        if (currentBGM == index && bgmSource.isPlaying)
        {
            return;
        }

        currentBGM = index;
        bgmSource.clip = bgm[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void AdjustMasterVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        bgmSource.volume = volume;

        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    private void LoadCurrentVolume()
    {
        float volume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        bgmSource.volume = volume;
    }
}