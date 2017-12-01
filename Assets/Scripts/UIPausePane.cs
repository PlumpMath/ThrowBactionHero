using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPausePane : MonoBehaviour {

  public delegate void ContinueAction();
  public static event ContinueAction OnContinue;

  public Color normalColor;
  public Color selectedColor;

  public Image[] options;

  private int optionIndex;

	void Start() {
    optionIndex = 0;
	}

	void Update() {

    if( GameController.hasEnded() ) {
      optionIndex = 1;
      options[0].enabled = false;
    } else {
      options[0].enabled = true;
    }

    CheckInput();

    for( int i = 0; i < options.Length; i++ ) {
      if( i == optionIndex ) {
        options[i].color = selectedColor;
      } else {
        options[i].color = normalColor;
      }
    }
	}

  void CheckInput() {
    float vertical = Input.GetAxis("Vertical");
    if( vertical < 0 && optionIndex < (options.Length-1) ) {
      Audio.PlayOptionSelect();
      optionIndex++;
    } else if( vertical > 0 && optionIndex != 0 ) {
      Audio.PlayOptionSelect();
      optionIndex--;
    }

    if( Input.GetKeyDown(KeyCode.Return) ) {
      Audio.PlayOptionConfirm();

      switch(optionIndex) {
        case 0:
          if( OnContinue != null ) {
            OnContinue();
          }
          break;
        case 1:
          Application.Quit();
          break;
      }
    }
  }
}
