using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICreditPane : MonoBehaviour {

  public delegate void EndAction();
  public static event EndAction OnEnd;

  public Image theEnd;
  public Transform credits;
  public float creditsMaxHeight;
  public Animator anim;

  private float time = 0f;
  private Color theEndColor;
  private Color transparent;
  private float creditsYPos;

	void Start() {
    anim = GetComponent<Animator>();
    theEndColor = theEnd.color;
    transparent = new Color(0, 0, 0, 0f);
    theEnd.color = transparent;
    creditsYPos = credits.position.y;
	}

	void Update() {
    time += Time.deltaTime;

    if( time > 9f && !anim.GetBool("isScrolling") ) {
      anim.SetBool("isScrolling", true);
    } else if( time > 6f ) {
      theEnd.color = Color.Lerp(theEndColor, transparent, (time - 6f)/2f);
    } else if( time > 2f ) {
      theEnd.color = Color.Lerp(transparent, theEndColor, (time - 2f)/2f);
    }

	}

  void CreditsFinished() {
    time = 0f;
    anim.SetBool("isScrolling", false);
    credits.position = new Vector3(credits.position.x, creditsYPos, credits.position.z);
    if( OnEnd != null ) {
      OnEnd();
    }
  }
}
