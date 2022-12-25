using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    NavMeshAgent smith;
    public Slider hpBar;

    public int score;

    Animator anim;  // ������ �ִϸ����� ������Ʈ ����
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }
    public EnemyState m_State;
    public float findDistance = 8f;
    Transform player;

    public static int zombieCount = 0;

    public float attackDistance = 2f;
    public float moveSpeed = 5f;
    CharacterController cc;

    float currentTime = 0;
    float attackDelay = 2f;

    public int attackPower = 3;

    Vector3 originPos;
    Quaternion originRot;
    public float moveDistance = 20f;    // �̵� ���� ����

    public int maxHp = 100;
    public int hp = 100;

    Vector3 knockBack = Vector3.zero;

    public bool check = false;

    // Start is called before the first frame update
    void Start()
    {
        if (zombieCount >= GameManager.gm.enemyLimit) {
            Destroy(gameObject);
            return;
        }

        zombieCount++;

        m_State = EnemyState.Idle;

        player = GameObject.FindWithTag("Player").transform;

        cc = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();  // Enemy ������Ʈ�� �ڽ� Zombie1 ������Ʈ�� Animator ������Ʈ�� ������
        smith = GetComponent<NavMeshAgent>();       // �׺���̼� ������Ʈ ������Ʈ �޾ƿ���

        // �ڽ�(����)�� �ʱ� ��ġ�� ȸ�� �� �����ϱ�
        originPos = transform.position;
        originRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gm.gState == GameManager.GameState.Ready)
            return;

        switch (m_State)
        {
            case EnemyState.Idle: Idle(); break;
            case EnemyState.Move: Move(); break;
            case EnemyState.Attack: Attack();  break;
            case EnemyState.Return: Return();  break;
        }

        cc.Move((smith.velocity + knockBack) * Time.deltaTime);

        if (check) {
            print(smith.velocity);
            print(knockBack);
        }

        if (knockBack.magnitude > 0.1f)
        {
            knockBack -= knockBack * Time.deltaTime * 2f;
        }
        else
        {
            knockBack = Vector3.zero;
        }

        if (cc.collisionFlags == CollisionFlags.Below)
        {
            if (knockBack.y < 0)
            {
                knockBack.y = 0;
            }
        }
        knockBack.y += -10 * Time.deltaTime;
    }
    void Idle()
    {
        if (Vector3.Distance(transform.position, player.position) < findDistance)
        {
            m_State = EnemyState.Move;
            print("���� ��ȯ : Idle -> Move");
            anim.SetTrigger("IdleToMove");
        }
        else
        {
        }
    }

    void Move()
    {
        if (Vector3.Distance(transform.position, originPos) > findDistance)
        {
            m_State = EnemyState.Return;
            //print("���� ��ȯ : Move -> Return");
        }
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {   /*
            dir = (player.position - transform.position).normalized;
            cc.Move(dir * mooveSpeed * Time.deltaTime);
            transform.forward = dir;*/
            //������̼��� �̵��� ���߰�, ��θ� �ʱ�ȭ�Ѵ�.
            smith.isStopped = true;
            smith.ResetPath();
            //������̼����� �����ϴ� �ּ� �Ÿ��� ���� ���ɰŸ��� ����
            smith.stoppingDistance = attackDistance;
            //������̼��� �������� �÷��̾��� ��ġ�� ����
            smith.destination = player.position;
        }
        else
        {
            m_State = EnemyState.Attack;
            //print("������ȯ: Move -> Attack");
            // ���� �÷��̾�� �ε����� �� 2�ʵڿ� ������ ������
            currentTime = attackDelay;

            anim.SetTrigger("MoveToAttackDelay");
        }
    }
    void Attack()
    {
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            currentTime += Time.deltaTime;
            // �ð��� �� �� 2�� �̻� �Ǿ��ٸ� ������ ����
            if (currentTime > attackDelay)
            {
                //print("����");
                //player.GetComponent<PlayerMove>().DamageAction(attackPower);
                currentTime = 0;
                anim.SetTrigger("StartAttack");
            }
        }
        else
        {
            m_State = EnemyState.Move;
            //print("���� ��ȯ: Attack->Move");
            currentTime = 0;
            anim.SetTrigger("AttackToMove");
        }
    }

    public void AttackAction() {
        player.GetComponent<PlayerMove>().DamageAction(attackPower);
    }

    void Return()
    {
        if (Vector3.Distance(transform.position, originPos) < 5f) {

            smith.isStopped = true;
            smith.ResetPath();

            m_State = EnemyState.Idle;

            hp = maxHp;

            anim.SetTrigger("MoveToIdle");
            //print("���� ��ȯ : Return -> Idle");
        }
        else
        {
            /*
            dir = (originPos - transform.position).normalized;
            transform.forward = dir;*/

            smith.stoppingDistance = 0;
            //������̼��� �������� �÷��̾��� ��ġ�� ����
            smith.destination = originPos;
        }

    }
    public void HitEnemy(int hitPower) {
        smith.isStopped = true;
        smith.ResetPath();

        findDistance = Mathf.Infinity;

        if (m_State == EnemyState.Die) {
            return;
        }
        hp -= hitPower;
        hpBar.value = (float)hp / (float)maxHp;
        if (hp > 0) {
            m_State = EnemyState.Damaged;
            //print("������ȯ: Any State -> Damaged");
            anim.SetTrigger("Damaged");
            Damaged();
        }
        else {
            m_State = EnemyState.Die;
            //print("���� ��ȯ: Any State -> Die");
            anim.SetTrigger("Die");
            Die();
        }
    }

    void Damaged() {
        StopAllCoroutines();
        StartCoroutine(DamageProcess());
    }

    IEnumerator DamageProcess() {
        // 0.5�ʵ��� �ǰ� �ִϸ��̼� ���� �� ������ ó���ϴ� �ð���ŭ
        // �ð��� �� �� ������ ��ƾ ����
        yield return new WaitForSeconds(1f);
        m_State = EnemyState.Move;
        //print("���� ��ȯ : Damaged -> Move");
    }
    void Die() {
        GameManager.gm.AddScore(score);
        zombieCount--;
        // �������� �ǰ� �ڷ�ƾ �Լ��� �����Ѵ�.
        StopAllCoroutines();
        // ���� �� ó���� ���� �ڷ�ƾ �Լ� ����
        StartCoroutine(DieProcess());
    }
    IEnumerator DieProcess() {
        // ĳ���� ��Ʈ�ѷ� ��Ȱ��ȭ (�̵� x)
        //cc.enabled = false;
        // 2�ʰ� ����� �� �ڱ��ڽ��� ����
        yield return new WaitForSeconds(2f);
        //print("�Ҹ�");
        Destroy(gameObject);
    }

    public void KnockBack(Vector3 vec)
    {
        knockBack = vec;

    }
}
