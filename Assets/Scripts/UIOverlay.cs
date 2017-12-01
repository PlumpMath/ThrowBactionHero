using UnityEngine;
using UnityEngine.UI;

public enum UIOverlayState {
  Blank,
  RestartPane,
  TitlePane,
  PausePane,
  CreditsPane
}

public class UIOverlay : MonoBehaviour {

  public static UIOverlay control;

  public int currentPaneIndex = 0;
  public GameObject[] panes;

  private Image image;

  public UIOverlayState state;

  void OnEnable() {
    GameController.OnPause += Pause;
    GameController.OnUnpause += Unpause;
    GameController.OnEnd += End;
    UICreditPane.OnEnd += RestartGame;
  }

  void OnDisable() {
    GameController.OnPause -= Pause;
    GameController.OnUnpause -= Unpause;
    GameController.OnEnd -= End;
    UICreditPane.OnEnd -= RestartGame;
  }

  void Awake() {
    if( control == null ) {
      control = this;
    } else if( control != this ) {
      Destroy(this);
    }
  }

	void Start() {
    image = GetComponent<Image>();
    image.enabled = false;

    Show(state);
	}

	void Update() {
    if( (int)state != currentPaneIndex ) {
      panes[currentPaneIndex].SetActive(false);
      panes[(int)state].SetActive(true);
      currentPaneIndex = (int)state;
    }

    if( state == UIOverlayState.Blank ) {
      image.enabled = false;
    }
	}

  // Custom

  void Pause() {
    Show(UIOverlayState.PausePane);
  }

  void Unpause() {
    Show(UIOverlayState.Blank);
  }

  void End() {
    Show(UIOverlayState.CreditsPane);
  }

  void RestartGame() {
    Show(UIOverlayState.TitlePane);
  }

  public bool isShowing() {
    return image.enabled = true;
  }

  public void Show( UIOverlayState newState ) {
    state = newState;
    image.enabled = true;
  }

  public void Hide() {
    state = UIOverlayState.Blank;
    image.enabled = false;;
  }
}
