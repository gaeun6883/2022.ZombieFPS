using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public GameObject hitEffect; // hit 효과 이미지 변수

    Vector3 knockBack = Vector3.zero;

    public Slider hpslider;

    public float moveSpeed = 7f;    // 이동 속도 변수
    public float jumpPower = 10f;    // 점프력 변수
    public float maxYVelocity = 3f;

    public bool isJumping = false;

    Animator anim;

    CharacterController cc;
    float gravity = -10f;   // 중력 변수
    float yVelocity = 0;    // 수직 속력 변수

    public int hp = 20;
    int maxHp = 20;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        maxHp = hp;
    }

    // Update is called once per frame
    void Update() {

        if (GameManager.gm.gState == GameManager.GameState.GameOver)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        // 이동 방향을 설정
        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized;


        // 메인카메라를 기준으로 방향을 변환
        dir = Camera.main.transform.TransformDirection(dir);

        if (cc.collisionFlags == CollisionFlags.Below)
        {
            if (isJumping && knockBack.magnitude < 0.1f)
            {
                isJumping = false;
            }
            if (yVelocity < 0)
            {
                yVelocity = 0;
            }
        }

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            {
                yVelocity = jumpPower;
                isJumping = true;
            }

        }
        //캐릭터 수직속도에 중력 값을 적용함
        yVelocity += gravity * Time.deltaTime;
        /*
        if (Mathf.Abs(yVelocity) > maxYVelocity) {
            yVelocity = maxYVelocity * Mathf.Sign(yVelocity);
        }
        */
        if (yVelocity < -maxYVelocity)
        {
            yVelocity = -maxYVelocity;
        }
        
        if (knockBack.magnitude > 0.1f)
        {
            knockBack -= knockBack * Time.deltaTime * 2f;
            anim.SetFloat("MoveMotion", 0f);
        }
        else {
            knockBack = Vector3.zero;
            // Move 블렌드 트리를 호출하고 벡터의 크기값을 넘겨준다.
            if (isJumping)
                anim.SetFloat("MoveMotion", 0f);
            else
                anim.SetFloat("MoveMotion", dir.magnitude);
        }

        dir.y = yVelocity;

        // 이동 속도에 맞춰 이동
        cc.Move((dir + knockBack) * moveSpeed * Time.deltaTime);

        //transform.position += dir * moveSpeed * Time.deltaTime;

        hpslider.value = (float)hp / (float)maxHp;
    }

    public void DamageAction(int damage) {
        // 적의 공격력만큼 플레이어의 체력을 감소시킴
        hp -= damage;
        StartCoroutine(PlayHitEffect());
    }

    IEnumerator PlayHitEffect() {
        // 0.3초 뒤 효과 사라짐
        hitEffect.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        hitEffect.SetActive(false);
    }

    public void KnockBack(Vector3 dir, float knockPower) {
        knockBack = dir * knockPower;
        yVelocity = knockBack.y;
        knockBack.y = 0;
        isJumping = true;

    }

    public void Heal() {
        hp += 5;
        if (hp > maxHp)
            hp = maxHp;
    }
}
