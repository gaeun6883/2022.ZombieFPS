using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public string itemName;
    public float beingTime;

    private void Start()
    {
        Destroy(gameObject, beingTime);
    }

    private void Update()
    {
        transform.eulerAngles += Vector3.up * Time.deltaTime * 30;
    }
}
