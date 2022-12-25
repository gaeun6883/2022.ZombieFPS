using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityEffect : DestroyEffect
{
    protected override void Bomb()
    {
        StartCoroutine(Gravity());
    }

    IEnumerator Gravity() {

        while (gameObject)
        {
            Collider[] targets = Physics.OverlapSphere(transform.position, range);

            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].transform.gameObject.layer == LayerMask.NameToLayer("Enemy") && targets[i].transform.tag == "Enemy")
                {
                    //targets[i].transform.position = transform.position;
                    EnemyFSM efsm = targets[i].GetComponent<EnemyFSM>();
                    efsm.KnockBack((transform.position - targets[i].transform.position).normalized * boomPower);
                }
            }
            yield return null;
        }
    }
}
