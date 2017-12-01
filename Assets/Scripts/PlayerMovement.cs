using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {

  public static PlayerMovement control;

  public delegate void ScreenShakeDelegate( bool x, bool y, float seconds );
  public static event ScreenShakeDelegate OnPlayerCausesShake;

  public float horizontalForce = 0f;
  public float verticalForce = 0f;
  public float maxVelocity;

  private Rigidbody2D rb;
  private CapsuleCollider2D collider;
  private SpriteRenderer sr;
  private Animator anim;

  private const RigidbodyConstraints2D FREEZE_POS_X = RigidbodyConstraints2D.FreezePositionX;
  private const RigidbodyConstraints2D FREEZE_POS_Y = RigidbodyConstraints2D.FreezePositionY;
  private const RigidbodyConstraints2D FREEZE_ROT   = RigidbodyConstraints2D.FreezeRotation;

  private bool canMove = false;
  private Vector2 frameVector = new Vector2(0, 0);

  public float horizontal = 0;
  public bool jump        = false;
  public bool needsToJump = false;
  public bool grounded    = false;
  public bool kicking     = false;
  public bool exiting     = false;

  void OnEnable() {
    GameController.OnPause += Pause;
    GameController.OnUnpause += Ready;
    Player.OnDeath += Death;
    Level.OnReady += Ready;
    UITitlePane.OnStart += Ready;
    UIRestart.OnRestart += Ready;
  }

  void OnDisable() {
    GameController.OnPause -= Pause;
    GameController.OnUnpause -= Ready;
    Player.OnDeath -= Death;
    Level.OnReady -= Ready;
    UITitlePane.OnStart -= Ready;
    UIRestart.OnRestart -= Ready;
  }

  void Awake() {
    if( control == null ) {
      control = this;
    } else if( control != this ) {
      Destroy(control);
    }

    rb       = GetComponent<Rigidbody2D>();
    sr       = GetComponent<SpriteRenderer>();
    anim     = GetComponent<Animator>();
    collider = GetComponent<CapsuleCollider2D>();
  }

  void Update() {
    UpdateInputs();

    frameVector = new Vector2(0, 0);
    if( canMove ) {
      CheckHorizontalMovement();
      CheckJump();
    }
  }

  void FixedUpdate() {
    if( needsToJump ) {
      needsToJump = false;
      anim.SetBool("isJumping", true);
      frameVector = new Vector2(frameVector.x, (transform.up * 5 * verticalForce).y);
    }

    rb.AddForce(frameVector, ForceMode2D.Impulse);
    CapVelocity();
  }

  // COLLISION

  void OnCollisionEnter2D( Collision2D collision ) {
    if( isGround(collision.gameObject.name) ) {
      grounded = true;
      jump = false;
      anim.SetBool("isJumping", false);
    }
  }

  void OnCollisionStay2D( Collision2D collision ) {
    if( isGround(collision.gameObject.name) ) {
      grounded = true;
      jump = false;
    }
  }

  void OnCollisionExit2D( Collision2D collision ) {
    if( isGround(collision.gameObject.name) ) {
      grounded = false;
      jump = true;
    }
  }

  void OnTriggerStay2D( Collider2D collider ) {
    if( !kicking && collider.gameObject.name == "RedDoor" && Round(transform.position.y) == Round(collider.transform.position.y) ) {
      Door doorScript = collider.gameObject.GetComponent<Door>();
      if( doorScript ) {
        StartCoroutine("KickCoroutine", doorScript);
      }
    }
  }

  // CUSTOM

  void Ready() {
    canMove = true;
  }

  void Pause() {
    canMove = false;
  }

  public void Throw() {
    anim.SetBool("isThrowing", true);
  }

  void ResetThrow() {
    anim.SetBool("isThrowing", false);
  }

  void Death() {
    canMove = false;
    anim.SetBool("isDead", true);
  }

  float Round( float val ) {
    return (float)System.Math.Round(val, 1);
  }

  bool isGround( string name ) {
    if( name == "Tilemap" || name == "Platforms" || name == "Obstacles" || name == "SolidPlatforms" ) {
      return true;
    } else {
      return false;
    }
  }

  void UpdateInputs() {
    if( canMove ) {
      horizontal = Input.GetAxis("Horizontal");
      jump = Input.GetButtonDown("Jump");
    } else {
      rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
      horizontal = 0f;
      jump = false;
    }
  }

  void CheckHorizontalMovement() {
    if( horizontal == 0 ) {
      anim.SetBool("isRunning", false);
      rb.constraints = FREEZE_POS_X | FREEZE_ROT;
    } else {
      if( horizontal > 0 ) {
        sr.flipX = false;
      } else if( horizontal < 0 ) {
        sr.flipX = true;
      }

      anim.SetBool("isRunning", grounded);

      rb.constraints = FREEZE_ROT;
      frameVector.x = (transform.right * horizontal * horizontalForce).x;
    }
  }

  void CapVelocity() {
    if( Mathf.Abs(rb.velocity.x) > maxVelocity ) {
      Vector2 newVelocity;
      if( rb.velocity.x > 0 ) {
        newVelocity = new Vector2(maxVelocity, rb.velocity.y);
      } else if( rb.velocity.x < 0 ) {
        newVelocity = new Vector2(-maxVelocity, rb.velocity.y);
      } else {
        newVelocity = new Vector2(0f, rb.velocity.y);
      }
      rb.velocity = newVelocity;
    }
  }

  void CheckJump() {
    RaycastHit2D[] hits = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y), Vector2.up * -1, 0.6f);
    // Debug.DrawRay(transform.position, Vector2.up * -0.6f, Color.green);

    if( jump && hits.Length > 0 ) {
      foreach( RaycastHit2D hit in hits ) {
        if( isGround(hit.collider.gameObject.name) ) {
          rb.constraints = FREEZE_ROT;
          frameVector.y = (transform.up * 5 * verticalForce).y;
          needsToJump = true;
          break;
        }
      }
    }
  }

  // Coroutines

  IEnumerator KickCoroutine( Door doorScript) {
    kicking = true;
    anim.SetBool("isKicking", true);
    doorScript.OpenDoor();
    yield return new WaitForSeconds(0.2f);
    canMove = false;
    if( OnPlayerCausesShake != null ) {
      OnPlayerCausesShake(true, false, 0.07f);
    }
    anim.SetBool("isKicking", false);
    yield return new WaitForSeconds(0.5f);
    exiting = true;
    collider.enabled = false;
    for( int i = 0; i < 100; i++ ) {
      rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
      if( i < 9 ) {
        rb.AddForce(transform.right * 1f * horizontalForce, ForceMode2D.Impulse);
        horizontal = 1;
        anim.SetBool("isRunning", false);
        anim.SetBool("isExiting", true);
      } else {
        rb.AddForce(transform.right * 1f * horizontalForce, ForceMode2D.Impulse);
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        horizontal = 0;
      }
      yield return null;
    }
    kicking = false;
    if( doorScript.lastDoor ) {
      GameController.End();
    } else {
      GameController.ChangeLevel();
    }
  }

}
