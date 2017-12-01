using UnityEngine;
using UnityEngine.UI;

public class UILife : MonoBehaviour {

  public Image[] lifePoints;
  private Text lifeText;

  void OnEnable() {
    Player.OnInjury += UpdateLife;
  }

  void OnDisable() {
    Player.OnInjury -= UpdateLife;
  }

	void Start() {
    lifeText = GetComponent<Text>();
	}

  void UpdateLife(int newLife) {
    for( int i = 0; i < lifePoints.Length; i++ ) {
      lifePoints[i].enabled = false;
    }
    for( int i = 0; i < newLife; i++ ) {
      if( i < lifePoints.Length ) {
        lifePoints[i].enabled = true;
      }
    }
  }
}
