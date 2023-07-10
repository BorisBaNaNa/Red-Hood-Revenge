using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu_Level : MonoBehaviour
{
    public int worldNumber = 0;
    public int levelNumber = 0;

    public string loadscene = "Level Name";

    public TextMeshProUGUI TextLevel;
    public GameObject Locked;

    // Use this for initialization
    void Start()
    {
        int worldReached = SaveInfoManager.LoadWorldsOpened();
        int levelReached = SaveInfoManager.LoadLevelsOpened(worldNumber);
        if (levelNumber <= levelReached && worldNumber <= worldReached)
        {
            TextLevel.gameObject.SetActive(true);
            TextLevel.text = levelNumber.ToString();
            Locked.SetActive(false);
        }
        else
        {
            TextLevel.gameObject.SetActive(false);
            Locked.SetActive(true);
            GetComponent<Button>().interactable = false;
        }
    }

    public void LoadScene()
    {
        LoadSceneState.LoadScene(loadscene);
    }
}
