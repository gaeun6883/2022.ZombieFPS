using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{

    public float destroyTime = 1.5f;
    public float range;
    public int boomPower;
    public float knockPower = 12f;

    //float currentTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        Bomb();
        Destroy(gameObject, destroyTime);
    }


    protected virtual void Bomb()
    {

        Collider[] targets = Physics.OverlapSphere(transform.position, range);

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                PlayerMove player = targets[i].GetComponent<PlayerMove>();
                Vector3 dir = (targets[i].transform.position - transform.position).normalized;
                player.KnockBack(dir, knockPower);
                player.DamageAction(boomPower);

            }
            else if (targets[i].transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                EnemyFSM efsm = targets[i].GetComponent<EnemyFSM>();

                if (efsm)
                {
                    efsm.HitEnemy(boomPower);
                    efsm.KnockBack((targets[i].transform.position - transform.position).normalized * knockPower);
                }
            }
        }
    }
}
