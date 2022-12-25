using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public GameObject hitEffect; // hit ȿ�� �̹��� ����

    Vector3 knockBack = Vector3.zero;

    public Slider hpslider;

    public float moveSpeed = 7f;    // �̵� �ӵ� ����
    public float jumpPower = 10f;    // ������ ����
    public float maxYVelocity = 3f;

    public bool isJumping = false;

    Animator anim;

    CharacterController cc;
    float gravity = -10f;   // �߷� ����
    float yVelocity = 0;    // ���� �ӷ� ����

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


        // �̵� ������ ����
        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized;


        // ����ī�޶� �������� ������ ��ȯ
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
        //ĳ���� �����ӵ��� �߷� ���� ������
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
            // Move ���� Ʈ���� ȣ���ϰ� ������ ũ�Ⱚ�� �Ѱ��ش�.
            if (isJumping)
                anim.SetFloat("MoveMotion", 0f);
            else
                anim.SetFloat("MoveMotion", dir.magnitude);
        }

        dir.y = yVelocity;

        // �̵� �ӵ��� ���� �̵�
        cc.Move((dir + knockBack) * moveSpeed * Time.deltaTime);

        //transform.position += dir * moveSpeed * Time.deltaTime;

        hpslider.value = (float)hp / (float)maxHp;
    }

    public void DamageAction(int damage) {
        // ���� ���ݷ¸�ŭ �÷��̾��� ü���� ���ҽ�Ŵ
        hp -= damage;
        StartCoroutine(PlayHitEffect());
    }

    IEnumerator PlayHitEffect() {
        // 0.3�� �� ȿ�� �����
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
