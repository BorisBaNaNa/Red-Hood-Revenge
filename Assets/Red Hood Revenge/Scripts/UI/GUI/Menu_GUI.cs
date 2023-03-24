using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_GUI : MonoBehaviour {

	public Text scoreText;
	public Text liveText;
	public Text bulletText;
	public Text coinText;
	public Text timerText;
	public GameObject UIPlayerController;

	private LevelManager _levelManager;

    private void Awake()
	{
		#if UNITY_STANDALONE
			UIPlayerController.SetActive(false);
		#elif UNITY_ANDROID
			UIPlayerController.SetActive(true);
		#endif

        _levelManager = AllServices.Instance.GetService<LevelManager>();
    }

    // Update is called once per frame
    void Update () {

        scoreText.text = _levelManager.Point.ToString ("0000000");
		coinText.text = _levelManager.Coin.ToString ("00");
		timerText.text = _levelManager.CurrentTime.ToString ("000");
		bulletText.text = _levelManager.BulletCount.ToString ();
		liveText.text = "x" + AllServices.Instance.GetService<GameManager>().LivesCount.ToString ();
	}
}
