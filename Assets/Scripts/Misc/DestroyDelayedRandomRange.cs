using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDelayedRandomRange : MonoBehaviour {

    [SerializeField]
    private float Minimum = 2f;

    [SerializeField]
    private float Maximum = 11f;

    void Start()
    {
        StartCoroutine(DestroyAfter(Random.Range(Minimum, Maximum)));
    }

    IEnumerator DestroyAfter(float dur)
    {
        yield return new WaitForSeconds(dur);

        Destroy(gameObject);
    }
}
