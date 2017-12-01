using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour {

  public AudioClip throwing;
  public AudioClip doorKick;
  public AudioClip injured;
  public AudioClip death;
  public AudioClip[] footsteps;

  private AudioSource audio;

  void OnEnable() {
    Player.OnDeath += Death;
    Player.OnInjury += Injury;
  }

  void OnDisable() {
    Player.OnDeath -= Death;
    Player.OnInjury -= Injury;
  }

	void Start() {
    audio = GetComponent<AudioSource>();
	}

  // Custom

  public void Injury(int health) {
    audio.PlayOneShot(injured);
  }

  public void Death() {
    audio.PlayOneShot(death);
  }

  void DoorKick() {
    audio.PlayOneShot(doorKick);
  }

  void Throw() {
    audio.PlayOneShot(throwing);
  }

  // This method is triggered by PlayerRun animation events
  void Footstep() {
    System.Random random = new System.Random();
    audio.PlayOneShot(footsteps[random.Next(0, 3)]);
  }
}
