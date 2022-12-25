using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator : MonoBehaviour
{
    public GameObject[] created = null;

    public Transform[] createPos = null;

    public float createTime = 2;
    public float spendTime = 0;

    public int goalCreateCount = 1;

    private void Start()
    {

    }

    private void Update()
    {
        spendTime += Time.deltaTime;

        if (spendTime < createTime)
            return;

        spendTime -= createTime;
        Create();

    }

    void Create() {
        for (int i = 0, j = 0; i < createPos.Length && j < goalCreateCount; i++)
        {

            if (goalCreateCount - j == createPos.Length - i || Random.Range(0, createPos.Length) < goalCreateCount)
            {
                Instantiate(created[Random.Range(0, created.Length)], createPos[i].position, Quaternion.identity);
                j++;

            }

        }
    }
}
