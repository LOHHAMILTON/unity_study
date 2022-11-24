using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    Vector3 lookVec;
    Vector3 tauntVec;
    public bool isLook = true; // 타운트할때는 플레이어 쳐다보지 않도록


    void Awake()
    {

        rigid = GetComponent<Rigidbody>();
        BoxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        anim = GetComponentInChildren<Animator>();
        nav = GetComponent<NavMeshAgent>();
        nav.isStopped = true;



        if (enemyType != Type.D)
            Invoke("chaseStart", 2);


        StartCoroutine(Think());
    }

    void Update()
    {


        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5;
            transform.LookAt(target.position + lookVec);
        }

        else if(curHealth>0)
            nav.SetDestination(tauntVec);

        if(isDead)
        {
            StopAllCoroutines();
            return;
        }
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);
        int ranAction = Random.Range(0, 5);
        switch (ranAction) // break를 생략하여 조건을 늘릴 수 있습니다.
        {
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                
                break;
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(Taunt());
                break;

        }

    }

    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(Think());
    }    
    IEnumerator RockShot()
    {
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec; // 점프공격할 위치

        isLook = false;
        nav.isStopped = false;
        BoxCollider.enabled = false;
        anim.SetTrigger("doTaunt");
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;


        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        BoxCollider.enabled = true;


        StartCoroutine(Think());
    }
}
