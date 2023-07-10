using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ResetData : MonoBehaviour
{
    private SoundManager _soundManager;

    void Start()
    {
        _soundManager = AllServices.Instance.GetService<SoundManager>();
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SoundManager.PlaySfx(_soundManager.SoundClick);

        AllServices.Instance.GetService<GameManager>().SetupValues();
    }

    public void UnlockAll()
    {
        SaveInfoManager.SaveWorldsOpened(int.MaxValue);
        int worldCount = AllServices.Instance.GetService<MainMenu>().WorldLevel.Length;
        for (int i = 0; i < worldCount; i++)
        {
            SaveInfoManager.SaveLevelsOpened(i + 1, 1000);
        }
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SoundManager.PlaySfx(_soundManager.SoundClick);
    }
}
