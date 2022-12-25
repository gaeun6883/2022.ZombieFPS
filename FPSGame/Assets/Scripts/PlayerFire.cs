using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFire : MonoBehaviour
{

    public CamRotate cam;
    public GameObject ui;
    public Camera mapCam;

    public Transform firePosition;  // 발사 위치
    public GameObject bombFactory;  // 수류탄 오브젝트
    public GameObject gravityBomb;  // 중력자탄
    public GameObject bulletEffect; // 피격 이펙트 오브젝트

    public Weapon[] weapons;

    ParticleSystem ps;  // 피격 이펙트 파티클 시스템

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
        Normal,     // 소총 모드
        Sniper      // 스나이퍼 모드
    }

    //WeaponMode wMode;

    public GameObject[] eff_Flash;  // 총 발사 효과 오브젝트 배열

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
        // 피격 이펙트 오브제그의 파티클 시스템 컴포넌트 가져오기
        // 피격 효과 재생을 위해...
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
        // 노멀 모드 : 수류탄 투척
        // 스나이퍼 : 화면 확대

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

                    // 레이를 생성한 후 발사될 위치와 진행 방향을 설정
                    Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

                    // 레이가 부딪힌 대상의 정볼르 저장할 변수를 생성
                    RaycastHit hitInfo = new RaycastHit();

                    cam.Rebond(reboundPower);

                    // 레이를 발사한 후 만일 부딪힌 물체가 있으면
                    // 피격 이펙트를 표시한다
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
                            // 피격 이벤트의 위치를 레이가 부딪힌 지점으로 이동시킴
                            bulletEffect.transform.position = hitInfo.point;
                            bulletEffect.transform.forward = hitInfo.normal;
                            // 피격 이펙트 효과 재생
                            ps.Play();
                        }
                    }
                    StartCoroutine(ShootEffectOn(0.05f));
                }
            }
        }

        // 숫자키 1번을 누르면 소총모드로 바뀌고, 카메라를 원래 상태로
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            //wMode = WeaponMode.Normal;
            cam.ZoomOut();
            //wModeText.text = "Normal Mode";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            // 숫자키 2번을 누르면 스나이퍼 모드로...
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
