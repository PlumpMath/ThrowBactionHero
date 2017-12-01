using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour {

  public AudioClip fireClip;
  public AudioClip deadClip;

  private AudioSource audio;

  void OnEnable() {
    Enemy.OnDeath += PlayDeadClip;
    Enemy.OnFire += PlayFireClip;
  }

  void OnDisable() {
    Enemy.OnDeath -= PlayDeadClip;
    Enemy.OnFire -= PlayFireClip;
  }

	void Start() {
    audio = GetComponent<AudioSource>();
	}

	void Update() {

	}

  // Customize

  void PlayDeadClip() {
    audio.PlayOneShot(deadClip);
  }

  void PlayFireClip(Enemy enemy) {
    audio.PlayOneShot(fireClip);
  }
}
