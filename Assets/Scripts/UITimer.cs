using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour {

  private Text timerText;

	void Start() {
    timerText = GetComponent<Text>();
	}

	void Update() {
    string currentTime = Level.time.ToString("F2");

    if( timerText.text != currentTime ) {
      timerText.text = currentTime;
    }
	}
}
