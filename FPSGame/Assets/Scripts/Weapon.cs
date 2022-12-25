using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public GameObject bomb;

    public string itemName;

    public Text bulletCountText;

    public Transform firePos;

    public KeyCode key;

    public int maxBulletCount;
    public int bulletCount;

    public float throwPower;
    public float currentTime = 0.1f;
    float spendTime = 0;
    

    public void Start() {
        spendTime = currentTime;
        bulletCountText.text = bulletCount.ToString() + " / " + maxBulletCount.ToString();
    }

    public void Update() {
        if (spendTime < currentTime) {
            spendTime += Time.deltaTime;
        }
    }

    public void Shoot()
    {
        if (spendTime >= currentTime && bulletCount > 0)
        {
            spendTime -= currentTime;
            GameObject created = MonoBehaviour.Instantiate(bomb);
            created.transform.position = firePos.position;
            Rigidbody rb = created.GetComponent<Rigidbody>();
            rb.velocity = Camera.main.transform.forward * throwPower;
            bulletCount--;

            bulletCountText.text = bulletCount.ToString() + " / " + maxBulletCount.ToString();
        }
    }

    public void AddBullet()
    {
        bulletCount++;

        if (bulletCount > maxBulletCount)
            bulletCount = maxBulletCount;

        bulletCountText.text = bulletCount.ToString() + " / " + maxBulletCount.ToString();

    }

}
