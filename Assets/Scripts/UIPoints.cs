using UnityEngine;
using UnityEngine.UI;

public class UIPoints : MonoBehaviour {

  private Text pointsText;
  private Animator anim;

	void Start() {
    anim = GetComponent<Animator>();
    pointsText = GetComponent<Text>();
	}

	void Update() {
    anim.SetBool("isAddingPoints", false);
    string currentPoints = Level.points.ToString();

    if( pointsText.text != currentPoints ) {

      anim.SetBool("isAddingPoints", true);
      Shaker.control.ShakeTransform(transform, true, true);
      pointsText.text = currentPoints;
    }
	}
}
