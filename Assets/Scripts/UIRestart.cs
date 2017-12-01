using UnityEngine;
using UnityEngine.UI;

public class UIRestart : MonoBehaviour {

  public delegate void RestartAction();
  public static event RestartAction OnRestart;

  public Color color;
  public Color targetColor;
  private Color noColor;

  private Image image;
  private AudioSource audio;

  private bool open = false;
  private float inputTime = 0f;

  void Start() {
    image = GetComponent<Image>();
    audio = GetComponent<AudioSource>();

    noColor = new Color(0f, 0f, 0f, 0f);
  }

  void Update() {
    if( Input.GetKeyDown(KeyCode.Space) ) {
      inputTime = 0f;
      image.color = color;
      audio.Play();
    } else if( Input.GetKeyUp(KeyCode.Space) ) {
      inputTime = 0f;
      image.color = color;
    } else if( inputTime > 0.5f && Input.GetKey(KeyCode.Space) ) {
      inputTime = 0f;
      if( OnRestart != null ) {
        OnRestart();
      }
      UIOverlay.control.Hide();
    } else if( Input.GetKey(KeyCode.Space) ) {
      inputTime += Time.deltaTime;
      image.color = Color.Lerp(color, targetColor, inputTime*2);
    } else {
      image.color = color;
    }

  }
}
