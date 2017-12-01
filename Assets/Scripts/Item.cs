using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

  public int throwForce;
  public float explosionRadius = 5f;
  public LayerMask explosionLayerMask;

  private SpriteRenderer sr;
  private Rigidbody2D rb;
  private Animator anim;
  private AudioSource audio;
  private BoxCollider2D boxCollider;
  private EdgeCollider2D edgeCollider;

  private bool hasBeenThrown;
  private bool exploding;

	void Start() {
    sr = GetComponent<SpriteRenderer>();
    rb = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    audio = GetComponent<AudioSource>();
    boxCollider = GetComponent<BoxCollider2D>();
    edgeCollider = GetComponent<EdgeCollider2D>();
	}

	void Update() {

	}

  void OnDrawGizmos() {
    Gizmos.DrawWireSphere(transform.position, explosionRadius);
  }

  void OnCollisionEnter2D( Collision2D collision ) {
    if( collision.gameObject.name == "Player" ) {
      edgeCollider.enabled = false;
      Vector2 liftPos = rb.position;
      liftPos.y += 1f;
      rb.MovePosition( liftPos );
      PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
      player.Throw();
      StartCoroutine("ThrowCoroutine");
    } else if( isExplodable(collision.gameObject.name) ) {
      Explosion();
    }
  }

  void OnCollisionExit2D( Collision2D collision ) {
    hasBeenThrown = true;
  }

  // Custom

  bool isExplodable(string name) {
    if( name == "Tilemap" || name == "Enemy" || name == "Platforms" || name == "Solid Platforms" || name == "Obstacle" || name == "Cube" ) {
      return true;
    } else {
      return false;
    }
  }

  void Explosion() {
    if( hasBeenThrown && !exploding )
      StartCoroutine("ExplosionCoroutine");
  }

  void TryLand() {
    if( hasBeenThrown )
      StartCoroutine("LandCoroutine");
  }

  void ExplosionForce() {
    Vector2 explosionPos = transform.position;
    Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, explosionRadius, explosionLayerMask);

    foreach (Collider2D hit in colliders) {
      if( hit.gameObject.name == "Player" || hit.gameObject == gameObject ) {
        continue;
      }
      Rigidbody2D otherRB = hit.attachedRigidbody;

      if( otherRB ) {
        otherRB.bodyType = RigidbodyType2D.Dynamic;
        Vector2 heading = otherRB.position - explosionPos;
        float distance = heading.magnitude;
        Vector2 direction = heading / distance;
        otherRB.AddForce(direction * 8f, ForceMode2D.Impulse);
      }
    }

  }

  void SelfDestruct() {
    Destroy(gameObject);
  }

  // Coroutines

  IEnumerator ThrowCoroutine() {
    yield return new WaitForSeconds(0.1f);
    rb.AddForce( new Vector2(-1.5f, 2f) * throwForce );
    yield return new WaitForSeconds(0.3f);
    boxCollider.enabled = true;
    boxCollider.isTrigger = false;
  }

  IEnumerator ExplosionCoroutine() {
    exploding = true;
    audio.Play();
    // Color color = sr.color;
    // sr.color = Color.red;
    anim.SetBool("isExploding", true);
    rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
    boxCollider.enabled = false;
    ExplosionForce();
    exploding = false;
    yield return null;
  }

}
