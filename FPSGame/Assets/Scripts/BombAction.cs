using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAction : MonoBehaviour
{
    public GameObject bombEffect;

    private void Start()
    {
    }
    private void OnCollisionEnter(Collision collision)
    {
        Bomb();
    }

    public void Bomb() {

        Instantiate(bombEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {

    }

}
