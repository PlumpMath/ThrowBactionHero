using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

  public delegate void ResetAction(Projectile resetProjectile);
  public static event ResetAction OnReset;

  public int speed = 10;
  public Vector2 reset;

  private Rigidbody2D rb;
  private SpriteRenderer sr;
  private BoxCollider2D box;

  private Vector2 pausedVelocity;
  private Direction direction;
  private bool firingProjectile = false;
  private Vector3 startingPosition;

  void OnEnable() {
    GameController.OnPause += Pause;
    GameController.OnUnpause += Unpause;
    Enemy.OnFire += Fire;
  }

  void OnDisable() {
    GameController.OnPause -= Pause;
    GameController.OnUnpause -= Unpause;
    Enemy.OnFire -= Fire;
  }

	void Start() {
    rb = GetComponent<Rigidbody2D>();
    sr = GetComponent<SpriteRenderer>();
    box = GetComponent<BoxCollider2D>();

    sr.enabled = false;
    box.enabled = false;
  }

  void OnCollisionEnter2D( Collision2D collision ) {
    if( collision.gameObject.name == "Player" ) {
      ResetProjectile();
    } else {
      ResetProjectile();
    }
  }

  // Custom

  void Pause() {
    pausedVelocity = rb.velocity;
    rb.velocity = Vector2.zero;
  }

  void Unpause() {
    rb.velocity = pausedVelocity;
  }

  void Fire( Enemy enemy ) {
    if( !GameObject.ReferenceEquals(enemy.gameObject, transform.parent.gameObject) ) {
      return;
    }
    if( !firingProjectile ) {
      startingPosition = transform.position;
      direction = enemy.direction;
      StartCoroutine("FireCoroutine");
    }
  }

  void FireProjectile() {
    Vector2 newVelocity = new Vector2(0, 0);
    sr.enabled = true;
    box.enabled = true;
    switch(direction) {
      case Direction.Right:
        newVelocity = Vector2.right;
        break;
      case Direction.Left:
        newVelocity = Vector2.right * -1;
        break;
    }
    rb.velocity = newVelocity * speed;
    firingProjectile = true;
  }

  void ResetProjectile() {
    sr.enabled = false;
    box.enabled = false;
    rb.velocity = new Vector2(0f, 0f);
    transform.position = startingPosition;
    firingProjectile = false;
    if( OnReset != null ) {
      OnReset(this);
    }
  }

  // Coroutine

  IEnumerator FireCoroutine() {

    FireProjectile();
    yield return new WaitForSeconds(3f);
    while( firingProjectile ) {
      float distance = Vector2.Distance(startingPosition, transform.position);
      if( distance > 10f ) {
        break;
      }
      yield return new WaitForSeconds(0.1f);
    }
    if( firingProjectile ) {
      ResetProjectile();
    }
  }
}
