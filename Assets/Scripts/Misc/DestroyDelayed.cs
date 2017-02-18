using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDelayed : MonoBehaviour {

    private float duration;
	void Start ()
    {
        // .. Destroy this object after the alive duration is passed from the score text
        duration = GetComponent<ScoreText>().aliveDuration;

        StartCoroutine(DestroyAfter(duration));	
	}
	
	IEnumerator DestroyAfter(float dur)
    {
        yield return new WaitForSeconds(dur);

        Destroy(gameObject);
    }
}
