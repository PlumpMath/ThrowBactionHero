using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

  public static GameController control;
  public static Level level;

  public delegate void LoadAction();
  public static event LoadAction OnLoad;

  public delegate void PauseAction();
  public static event PauseAction OnPause;

  public delegate void UnpauseAction();
  public static event UnpauseAction OnUnpause;

  public delegate void EndAction();
  public static event EndAction OnEnd;

  public string firstScene;

  private bool paused = false;
  private bool ended = false;

  void OnEnable() {
    Player.OnDeath += RestartLevel;
    UIPausePane.OnContinue += Continue;
    UICreditPane.OnEnd += RestartGame;
  }

  void OnDisable() {
    Player.OnDeath -= RestartLevel;
    UIPausePane.OnContinue -= Continue;
    UICreditPane.OnEnd -= RestartGame;
  }

  void Awake() {
    if( control == null ) {
      control = this;
    } else {
      Destroy(control);
    }
  }

	void Start() {
    Cursor.visible = false;

    if( firstScene != "" ) {
      SceneManager.LoadScene(firstScene, LoadSceneMode.Single);
    } else {
      SceneManager.LoadScene("Scene00", LoadSceneMode.Single);
    }
	}

	void Update() {
    HandlePause();
	}

  // CUSTOM

  public static void SetLevel( Level newLevel ) {
    level = newLevel;
  }

  public static void RestartLevel(){
    control.StartCoroutine(control.RestartCoroutine());
  }

  public static void ChangeLevel(){
    control.StartCoroutine(control.ChangeLevelCoroutine());
  }

  public static void End() {
    control.ended = true;
    if( OnEnd != null ) {
      OnEnd();
    }
  }

  public static bool hasEnded() {
    return control.ended;
  }

  void Load() {
    if( OnLoad != null ) {
      OnLoad();
    }
  }

  void HandlePause() {
    if( !Input.GetKeyDown(KeyCode.Escape) ) {
      return;
    }
    if( paused ) {
      paused = false;
      if( OnUnpause != null ) {
        OnUnpause();
      }
    } else {
      paused = true;
      if( OnPause != null ) {
        OnPause();
      }
    }
  }

  void Continue() {
    if( OnUnpause != null ) {
      OnUnpause();
    }
  }

  void RestartGame() {
    Start();
  }

  // Coroutines

  IEnumerator RestartCoroutine() {
    yield return new WaitForSeconds(0.5f);
    UIOverlay.control.Show(UIOverlayState.RestartPane);
    SceneManager.LoadScene(level.scene, LoadSceneMode.Single);
    yield return null;
  }

  IEnumerator ChangeLevelCoroutine() {
    Destroy(level.gameObject);
    AsyncOperation async = SceneManager.LoadSceneAsync(level.nextScene, LoadSceneMode.Single);

    while( async.progress < 1f ) {
      yield return null;
    }

    if( OnLoad != null ) {
      OnLoad();
    }
  }

}
