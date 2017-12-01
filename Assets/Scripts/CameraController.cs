using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

  public static CameraController control;

  public int xOffset;
  public int yOffset;
  private bool xShake;
  private bool yShake;

  private Player player;

  // public delegate void ReceiverDelegate( bool x, bool y, float seconds );
  // public event ReceiverDelegate

  void Awake() {
    if( control == null ) {
      control = this;
    } else {
      Destroy(control.gameObject);
    }
  }

  void Start() {
  }

	void OnEnable() {
    Player.OnStart += WatchPlayer;
    PlayerMovement.OnPlayerCausesShake += ScreenShake;
	}

  void OnDisable() {
    Player.OnStart -= WatchPlayer;
    PlayerMovement.OnPlayerCausesShake -= ScreenShake;
  }

	void Update() {
    if( player ) {
      float[] shakeOffsets = Shake();
      float xPos = player.transform.position.x + shakeOffsets[0];
      float yPos = player.transform.position.y + shakeOffsets[1];

      // transform.position = new Vector3(xPos + xOffset, yPos + yOffset, transform.position.z);
      if( transform.position != new Vector3(player.transform.position.x + xOffset, player.transform.position.y + yOffset, player.transform.position.z) ) {
        float newY = Mathf.Lerp(transform.position.y, yPos + yOffset, 10f * Time.deltaTime);
        transform.position = new Vector3(xPos + xOffset, newY, transform.position.z);
      }
    }
	}

  // Custom

  void WatchPlayer(Player playerInstance) {
    player = playerInstance;
    transform.position = new Vector3(player.transform.position.x + xOffset, player.transform.position.y + yOffset, transform.position.z);
  }

  float[] Shake() {
    float x = 0f;
    float y = 0f;

    if( xShake ) {
      x = Random.Range(0f, 0.2f);
    }

    if( yShake ) {
      y = Random.Range(0f, 0.2f);
    }

    float[] offsets = new float[2];
    offsets[0] = x;
    offsets[1] = y;
    return offsets;
  }

  void ScreenShake( bool x, bool y ) {
    xShake = x;
    yShake = y;
  }

  void ScreenShake( bool x, bool y, float seconds ) {
    xShake = x;
    yShake = y;
    StartCoroutine("ResetShakeCoroutine", seconds);
  }

  // Coroutine

  IEnumerator ResetShakeCoroutine( float seconds ) {
    yield return new WaitForSeconds(seconds);
    xShake = false;
    yShake = false;
  }
}





