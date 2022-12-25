using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    Transform camTr;

    private void Start() {
        camTr = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {

        transform.eulerAngles = new Vector3(0, camTr.eulerAngles.y, 0);

    }
}
