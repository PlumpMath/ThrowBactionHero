using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UITitlePane : MonoBehaviour {

  public delegate void StartAction();
  public static event StartAction OnStart;

  public float smallScale = 15f;
  public float mediumScale = 20f;
  public float largeScale = 25f;

  public AudioClip mainTheme;
  public AudioClip mainThemeLoop;
  public Animator[] options;

  private AudioSource audio;

  private int optionIndex;
  private float timePassed;

  void Awake() {
    timePassed = 0f;
  }

	void Start() {
    optionIndex = 0;
  }

	void Update() {

    float zScale = transform.localScale.z;
    if( Screen.width > 1559 ) {
      transform.localScale = new Vector3(largeScale, largeScale, zScale);
    } else if( Screen.width > 1279 ) {
      transform.localScale = new Vector3(mediumScale, mediumScale, zScale);
    } else {
      transform.localScale = new Vector3(smallScale, smallScale, zScale);
    }

    CheckInput();

    for( int i = 0; i < options.Length; i++ ) {
      if( i == optionIndex ) {
        options[i].SetBool("isSelected", true);
      } else {
        options[i].SetBool("isSelected", false);
      }
    }
	}

  void CheckInput() {
    float horizontal = Input.GetAxis("Horizontal");
    if( horizontal > 0 && optionIndex < (options.Length-1) ) {
      Audio.PlayOptionSelect();
      optionIndex++;
    } else if( horizontal < 0 && optionIndex != 0 ) {
      Audio.PlayOptionSelect();
      optionIndex--;
    }

    if( Input.GetKeyDown(KeyCode.Return) ) {
      Audio.PlayOptionConfirm();

      switch(optionIndex) {
        case 0:
          UIOverlay.control.Hide();
          if( OnStart != null ) {
            OnStart();
          }
          break;
        case 1:
          Application.Quit();
          break;
      }
    }
  }
}
