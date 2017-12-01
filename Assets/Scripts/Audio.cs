using UnityEngine;
using UnityEngine.Audio;

public class Audio : MonoBehaviour {

  public static Audio control;

  public AudioSource audio1;
  public AudioSource audio2;
  public AudioSource fxAudio;

  public AudioClip optionSelectClip;
  public AudioClip optionConfirmClip;

  public AudioMixerSnapshot introSnapshot;
  public AudioMixerSnapshot loopSnapshot;

  private float timePassed;


  void Awake() {

    timePassed = 0f;

    if( control == null ) {
      control = this;
    } else if( control != this ) {
      Destroy(this);
    }
  }
	// Use this for initialization
	void Start () {
    introSnapshot.TransitionTo(0f);

	}

	// Update is called once per frame
	void Update () {

    timePassed += Time.deltaTime;

    if( timePassed > audio1.clip.length - 2f ) {
      loopSnapshot.TransitionTo(0f);
      audio1.Stop();
    }

	}

  public static void PlayOptionSelect() {
    control.fxAudio.PlayOneShot(control.optionSelectClip);
  }

  public static void PlayOptionConfirm() {
    control.fxAudio.PlayOneShot(control.optionConfirmClip);
  }
}
