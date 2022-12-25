using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{

    public CamRotate cam;
    public GameObject ui;
    public Camera mapCam;

    public Transform firePosition;  // �߻� ��ġ
    public GameObject bombFactory;  // ����ź ������Ʈ
    public GameObject gravityBomb;  // �߷���ź
    public GameObject bulletEffect; // �ǰ� ����Ʈ ������Ʈ

    public Weapon[] weapons;

    ParticleSystem ps;  // �ǰ� ����Ʈ ��ƼŬ �ý���

    public float reloadTime = 0.5f;

    public int weaponPower = 5;
    public int reboundPower = 10;

    public float currentTime = 0.1f;
    float spendTime;

    public int maxBullet;
    public int nowBullet;

    public Text bulletCount;

    PlayerMove pm;
    enum WeaponMode { 
        Normal,     // ���� ���
        Sniper      // �������� ���
    }

    //WeaponMode wMode;

    public GameObject[] eff_Flash;  // �� �߻� ȿ�� ������Ʈ �迭

    public Text wModeText;

    Animator anim;

    bool reloading;

    public float throwPower = 15f;
    private void OnTriggerEnter(Collider other) {
        if (other.transform.CompareTag("Item")) {
            string contain = other.GetComponent<ItemInfo>().itemName;

            if (contain == "Heal") {
                pm.Heal();
            }
            else {
                for (int i = 0; i < weapons.Length; i++) {
                    if (weapons[i].itemName == contain) {
                        weapons[i].AddBullet();
                        break;
                    }
                }
            }
            Destroy(other.gameObject);
        }
    }

    private void Start()
    {
        // �ǰ� ����Ʈ ���������� ��ƼŬ �ý��� ������Ʈ ��������
        // �ǰ� ȿ�� ����� ����...
        pm = GetComponent<PlayerMove>();
        ps = bulletEffect.GetComponent<ParticleSystem>();
        spendTime = currentTime;
        nowBullet = maxBullet;
        anim = GetComponentInChildren<Animator>();

        bulletCount.text = nowBullet.ToString() + " / " + maxBullet.ToString();
        //nowBullet = maxBullet;
    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.gm.gState != GameManager.GameState.Run)
            return;

        /*
        if (Input.GetKey(KeyCode.Tab)) {
            mapCam.gameObject.SetActive(true);
            ui.SetActive(false);
        }
        else {
            mapCam.gameObject.SetActive(false);
            ui.SetActive(true);
        }
        */
        // ��� ��� : ����ź ��ô
        // �������� : ȭ�� Ȯ��

        if (reloading)
            return;

        if (Input.GetMouseButton(1)) {
            cam.Zoom();
            mapCam.gameObject.SetActive(false);
        }
        else {
            cam.ZoomOut();
            mapCam.gameObject.SetActive(true);
            for (int i = 0; i < weapons.Length; i++) {
                if (Input.GetKeyDown(weapons[i].key)) {
                    weapons[i].Shoot();
                    break;
                }
            }
        }

        if (Input.GetMouseButton(0)) {

            if (nowBullet <= 0) {
                Reload();
            }
            else {
                if (anim.GetFloat("MoveMotion") == 0) {
                    anim.SetTrigger("Attack");
                }
                if (spendTime >= currentTime) {
                    spendTime = 0;
                    nowBullet--;
                    bulletCount.text = nowBullet.ToString() + " / " + maxBullet.ToString();

                    // ���̸� ������ �� �߻�� ��ġ�� ���� ������ ����
                    Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

                    // ���̰� �ε��� ����� ������ ������ ������ ����
                    RaycastHit hitInfo = new RaycastHit();

                    cam.Rebond(reboundPower);

                    // ���̸� �߻��� �� ���� �ε��� ��ü�� ������
                    // �ǰ� ����Ʈ�� ǥ���Ѵ�
                    if (Physics.Raycast(ray, out hitInfo)) {

                        if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
                            int head = 1;
                            EnemyFSM eFSM;
                            if (hitInfo.collider.CompareTag("Head")) {
                                eFSM = hitInfo.transform.GetComponentInParent<EnemyFSM>();
                                GameManager.gm.AddScore(50);
                                head = 5;
                            }
                            else {
                                eFSM = hitInfo.collider.GetComponent<EnemyFSM>();
                            }
                            eFSM.HitEnemy(weaponPower * head);
                        }
                        else if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Bomb")) {
                            hitInfo.transform.GetComponent<BombAction>().Bomb();
                        }
                        else {
                            // �ǰ� �̺�Ʈ�� ��ġ�� ���̰� �ε��� �������� �̵���Ŵ
                            bulletEffect.transform.position = hitInfo.point;
                            bulletEffect.transform.forward = hitInfo.normal;
                            // �ǰ� ����Ʈ ȿ�� ���
                            ps.Play();
                        }
                    }
                    StartCoroutine(ShootEffectOn(0.05f));
                }
            }
        }

        // ����Ű 1���� ������ ���Ѹ��� �ٲ��, ī�޶� ���� ���·�
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            //wMode = WeaponMode.Normal;
            cam.ZoomOut();
            //wModeText.text = "Normal Mode";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            // ����Ű 2���� ������ �������� ����...
           // wMode = WeaponMode.Sniper;
           // wModeText.text = "Sniper Mode";
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            Reload();
        }

        if (spendTime < currentTime)
        {
            spendTime += Time.deltaTime;
        }
    }

    IEnumerator ShootEffectOn(float duration) {
        int num = Random.Range(0, eff_Flash.Length);
        eff_Flash[num].SetActive(true);
        yield return new WaitForSeconds(duration);
        eff_Flash[num].SetActive(false);
    }

    void Reload()
    {
        mapCam.gameObject.SetActive(true);
        if (nowBullet >= maxBullet)
            return;
        cam.ZoomOut();
        StartCoroutine(ReloadAction());
    }

    IEnumerator ReloadAction() {
        reloading = true;
        anim.SetTrigger("Reload");
        yield return new WaitForSeconds(reloadTime);
        nowBullet = maxBullet;
        bulletCount.text = nowBullet.ToString() + " / " + maxBullet.ToString();
        reloading = false;
    }

}
