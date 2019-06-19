using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructAfterTime : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }

    private IEnumerator SelfDestruct()
    {
    	yield return new WaitForSeconds(1f);
    	Destroy(gameObject);
    }
}
