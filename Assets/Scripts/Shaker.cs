using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour {

  public static Shaker control = null;

  private static IEnumerator shakeCoroutine;

  void Awake() {
    if( control == null ) {
      control = this;
    } else if( control != this ) {
      Destroy(this);
    }
  }

	void Start() {

	}

	void Update() {

	}

  // Custom

  public void ShakeTransform(Transform target, bool shakeX, bool shakeY) {
    shakeCoroutine = Shake(target, true, true);
    StartCoroutine(shakeCoroutine);
  }

  // Coroutines

  IEnumerator Shake(Transform target, bool shakeX, bool shakeY) {
    Vector3 originalPos = target.position;

    for( float t = 0f; t < 0.3f; t += Time.deltaTime ) {
      float randX = RandomRangeWithPadding(-2f, 2f, 1.9f);
      float randY = RandomRangeWithPadding(-2f, 2f, 1.9f);
      Vector3 shakePos = new Vector3(target.position.x + randX, target.position.y + randY, target.position.z);
      target.position = shakePos;

      yield return new WaitForSeconds(0.005f);
    }

    target.position = originalPos;

  }

  float RandomRangeWithPadding(float min, float max, float padding) {


    float middle = (max - (max - min)/2);
    float randomMin = Random.Range(middle - padding, min);
    float randomMax = Random.Range(middle + padding, max);
    int rand = Random.Range(0, 100);
    if( rand % 2 == 0 ) {
      return randomMin;
    } else {
      return randomMax;
    }
  }
}
