using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

  public delegate void StartAction(Player player);
  public static event StartAction OnStart;

  public delegate void DeathAction();
  public static event DeathAction OnDeath;

  public delegate void InjuryAction(int health);
  public static event InjuryAction OnInjury;

  public int health = 2;
  private int maxHealth = 2;
  private bool dead = false;

  void Start() {
    if( OnStart != null ) {
      OnStart(this);
    }
  }

	void Update() {
    if( transform.position.y < -6 && !dead ) {
      triggerDeath();
    }
	}

  void OnCollisionEnter2D( Collision2D collision ) {
    switch(collision.gameObject.name) {
      case "Enemy":
        DecreaseHealth(2);
        break;
      case "Projectile":
        DecreaseHealth(1);
        break;
    }
  }

  public void IncreaseHealth() {
    if( health < maxHealth ) {
      health += 1;
    }
  }

  public void DecreaseHealth(int amount) {
    if( health > 0 ) {
      health -= amount;
      triggerInjury();
    }

    if( health <= 0 ) {
      triggerDeath();
    }
  }

  // Triggers

  void triggerDeath() {
    dead = true;
    if( OnDeath != null ) {
      OnDeath();
    }
  }

  void triggerInjury() {
    if( OnInjury != null ) {
      OnInjury(health);
    }
  }

}
