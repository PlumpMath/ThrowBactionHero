using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

  public bool lastDoor;

  private Animator anim;

	void Start() {
    anim = GetComponent<Animator>();
	}

  public void OpenDoor() {
    anim.SetBool("isOpen", true);
  }
}
