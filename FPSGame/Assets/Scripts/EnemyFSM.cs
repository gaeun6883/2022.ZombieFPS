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

    Animator anim;  // 좀비의 애니메이터 컴포넌트 변수
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
    public float moveDistance = 20f;    // 이동 가능 범위

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
        anim = GetComponentInChildren<Animator>();  // Enemy 오브젝트의 자식 Zombie1 오브젝트의 Animator 컴포넌트를 가져옴
        smith = GetComponent<NavMeshAgent>();       // 네비게이션 에이전트 컴포넌트 받아오기

        // 자신(좀비)의 초기 위치와 회전 값 저장하기
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
            print("상태 전환 : Idle -> Move");
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
            //print("상태 전환 : Move -> Return");
        }
        else if (Vector3.Distance(transform.position, player.position) > attackDistance)
        {   /*
            dir = (player.position - transform.position).normalized;
            cc.Move(dir * mooveSpeed * Time.deltaTime);
            transform.forward = dir;*/
            //내비게이션의 이동을 멈추고, 경로를 초기화한다.
            smith.isStopped = true;
            smith.ResetPath();
            //내비게이션으로 접근하는 최소 거리를 공격 가능거리로 설정
            smith.stoppingDistance = attackDistance;
            //내비게이션의 목적지를 플레이어의 위치로 설정
            smith.destination = player.position;
        }
        else
        {
            m_State = EnemyState.Attack;
            //print("상태전환: Move -> Attack");
            // 적이 플레이어와 부딪혔을 때 2초뒤에 공격을 실행함
            currentTime = attackDelay;

            anim.SetTrigger("MoveToAttackDelay");
        }
    }
    void Attack()
    {
        if (Vector3.Distance(transform.position, player.position) < attackDistance)
        {
            currentTime += Time.deltaTime;
            // 시간을 잰 뒤 2초 이상 되었다면 공격을 실행
            if (currentTime > attackDelay)
            {
                //print("공격");
                //player.GetComponent<PlayerMove>().DamageAction(attackPower);
                currentTime = 0;
                anim.SetTrigger("StartAttack");
            }
        }
        else
        {
            m_State = EnemyState.Move;
            //print("상태 전환: Attack->Move");
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
            //print("상태 전환 : Return -> Idle");
        }
        else
        {
            /*
            dir = (originPos - transform.position).normalized;
            transform.forward = dir;*/

            smith.stoppingDistance = 0;
            //내비게이션의 목적지를 플레이어의 위치로 설정
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
            //print("상태전환: Any State -> Damaged");
            anim.SetTrigger("Damaged");
            Damaged();
        }
        else {
            m_State = EnemyState.Die;
            //print("상태 전환: Any State -> Die");
            anim.SetTrigger("Die");
            Die();
        }
    }

    void Damaged() {
        StopAllCoroutines();
        StartCoroutine(DamageProcess());
    }

    IEnumerator DamageProcess() {
        // 0.5초동안 피격 애니메이션 실행 등 데미지 처리하는 시간만큼
        // 시간을 번 뒤 나머지 루틴 실행
        yield return new WaitForSeconds(1f);
        m_State = EnemyState.Move;
        //print("상태 전환 : Damaged -> Move");
    }
    void Die() {
        GameManager.gm.AddScore(score);
        zombieCount--;
        // 진행중인 피격 코루틴 함수를 중지한다.
        StopAllCoroutines();
        // 죽은 뒤 처리를 위한 코루틴 함수 실행
        StartCoroutine(DieProcess());
    }
    IEnumerator DieProcess() {
        // 캐릭터 컨트롤러 비활성화 (이동 x)
        //cc.enabled = false;
        // 2초간 대기한 뒤 자기자신을 제거
        yield return new WaitForSeconds(2f);
        //print("소멸");
        Destroy(gameObject);
    }

    public void KnockBack(Vector3 vec)
    {
        knockBack = vec;

    }
}
