using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDelayedGeneric : MonoBehaviour {

    [SerializeField]
    private float duration = 1f;
    void Start()
    {
        StartCoroutine(DestroyAfter(duration));
    }

    IEnumerator DestroyAfter(float dur)
    {
        yield return new WaitForSeconds(dur);

        Destroy(gameObject);
    }
}
