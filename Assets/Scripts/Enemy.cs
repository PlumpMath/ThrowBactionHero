using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction {
  Left,
  Right
}

public class Enemy : MonoBehaviour {

  public delegate void DeathAction();
  public static event DeathAction OnDeath;

  public delegate void FireAction(Enemy enemy);
  public static event FireAction OnFire;

  public Direction direction = Direction.Right;

  public Projectile projectile;

  private SpriteRenderer sr;
  private Rigidbody2D rb;
  private Animator anim;
  private Animation animation;

  private bool ready = false;
  private bool alive = true;
  private bool attacking = false;
  private bool paused = false;
  private bool pausedAttacking = false;
  private bool coroutineRunning = false;
  private Vector2 rayDirection = new Vector2(0, 0);

  void OnEnable() {
    Level.OnReady += Ready;
    GameController.OnPause += Pause;
    GameController.OnUnpause += Ready;
    Projectile.OnReset += Reset;
  }

  void OnDisable() {
    Level.OnReady -= Ready;
    GameController.OnPause -= Pause;
    GameController.OnUnpause -= Ready;
    Projectile.OnReset -= Reset;
  }

	void Start() {
    sr = GetComponent<SpriteRenderer>();
    rb = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    animation = GetComponent<Animation>();

    int yRot = 0;
    switch(direction) {
      case Direction.Right:
        yRot = 180;
        rayDirection = Vector2.right;
        break;
      case Direction.Left:
        yRot = 0;
        rayDirection = Vector2.right * -1;
        break;
    }

    Vector3 newAngles = transform.eulerAngles;
    newAngles.y = yRot;
    transform.eulerAngles = newAngles;
	}

  void Update() {
    if( !ready || !alive ) {
      return;
    }

    if( alive && rb.bodyType == RigidbodyType2D.Dynamic ) {
      StartDeath();
    }

    CheckForPlayer();
  }

  void OnCollisionEnter2D( Collision2D collision ) {
    if( alive && collision.gameObject.name == "Cube" ) {
      StartDeath();
    }
  }

  // Custom

  void Ready() {
    attacking = false;
    ready = true;
    paused = false;
  }

  void Pause() {
    paused = true;
    ready = false;
  }

  void Reset(Projectile resetProjectile) {
    if( projectile != resetProjectile ) {
      return;
    }
  }

  void StartDeath() {
    alive = false;
    if( OnDeath != null ) {
      OnDeath();
    }
    anim.SetBool("isDead", true);
    StartCoroutine("Die");
  }

  void CheckForPlayer() {
    RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, rayDirection, 8f);
    // Debug.DrawRay(transform.position, rayDirection, Color.green);
    bool includesPlayer = false;
    for( int i = 0; i < hits.Length; i++ ) {
      if( hits[i].collider.gameObject.name == "Player" ) {
        attacking = true;
        includesPlayer = true;
        StartCoroutine("Attack");
        break;
      }
    }
    if( !includesPlayer ) {
      // attacking = false;
    }
  }

  void ResetFire() {
    anim.SetBool("isFiring", false);
  }

  // Coroutines

  IEnumerator Die() {
    Color color = sr.color;
    Color noColor = new Color(color.r, color.g, color.b, 0);
    yield return new WaitForSeconds(0.25f);
    sr.color = noColor;
    yield return new WaitForSeconds(0.25f);
    sr.color = color;
    yield return new WaitForSeconds(0.25f);
    sr.color = noColor;
    yield return new WaitForSeconds(0.25f);
    sr.color = color;
    yield return new WaitForSeconds(0.25f);
    sr.color = noColor;
    yield return new WaitForSeconds(0.25f);
    sr.color = color;
    Destroy(gameObject);
  }

  IEnumerator Attack() {
    if( !coroutineRunning ) {
      coroutineRunning = true;
      yield return new WaitForSeconds(0.5f);
      while( paused ) {
        yield return new WaitForSeconds(0.1f);
      }
      if( alive && attacking ) {
        anim.SetBool("isFiring", true);
        attacking = false;
        if( OnFire != null ) {
          OnFire(this);
        }
      }
      yield return new WaitForSeconds(2f);
      while( paused ) {
        yield return new WaitForSeconds(0.1f);
      }
      coroutineRunning = false;
    }
  }
}
