using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

  public delegate void LevelReadyAction();
  public static event LevelReadyAction OnReady;

  public static Level control = null;
  public string scene;
  public string nextScene;

  public static float time;
  public static float points;

  private IEnumerator shakeCoroutine;

  void OnEnable() {
    GameController.OnLoad += Run;
    UITitlePane.OnStart += Run;
    UIRestart.OnRestart += Run;
  }

  void OnDisable() {
    GameController.OnLoad -= Run;
    UITitlePane.OnStart -= Run;
    UIRestart.OnRestart -= Run;
  }

  void Awake() {
    GameController.SetLevel(this);

    if( control == null ) {
      control = this;
    } else if( control != this ) {
      Destroy(this);
    }
  }

	void Update() {
    time += Time.deltaTime;
	}

  // Custom

  void Run() {
    time = 0f;
    if( OnReady != null ) {
      OnReady();
    }
  }

  public static void AddPoints(int newPoints) {
    points += newPoints;
  }

}
