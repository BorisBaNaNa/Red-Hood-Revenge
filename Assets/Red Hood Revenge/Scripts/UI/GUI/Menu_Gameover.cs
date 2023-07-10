using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_Gameover : MonoBehaviour
{
    public Text liveText;
    public GameObject Next;
    public GameObject StartOver;
    public GameObject Buttons;
    int lives;

    void Awake()
    {
        var levelReached = SaveInfoManager.LoadLevelsOpened(GameManager.WorldPlaying);
        Next.SetActive(GameManager.LevelPlaying < levelReached && !LevelManager.IsLastLevel);
    }

    void OnEnable()
    {

        Buttons.SetActive(false);

        if (!AllServices.Instance.GetService<GameManager>().IsHaveNotLives)
            lives = AllServices.Instance.GetService<GameManager>().LivesCount;
        else
            lives = 0;


        liveText.text = (lives + 1).ToString("00");
        StartCoroutine(SubtractLiveCo(1));
    }

    IEnumerator SubtractLiveCo(float time)
    {
        yield return new WaitForSecondsRealtime(time);

        liveText.text = lives.ToString("00");
        liveText.gameObject.GetComponent<Animator>()?.SetTrigger("live");

        StartOver.SetActive(lives <= 0);
        Buttons.SetActive(!(lives <= 0));
    }
}
