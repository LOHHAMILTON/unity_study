using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C};
    public Type enumType;
    public int masHealth;
    public int curHealth;
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bullet;
    public bool isChase;
    public bool isAttack;


    Rigidbody rigid;
    BoxCollider BoxCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;
    


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        BoxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        anim = GetComponentInChildren<Animator>();

        nav = GetComponent<NavMeshAgent>();

        Invoke("chaseStart", 2);
    }


    void chaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    private void Update()
    {
        if(nav.enabled)
        {
            nav.SetDestination(target.position);//목표만 잃어버리는거라 이동이 유지됨;
            nav.isStopped = !isChase;


        }
    }



    void FreezeVelocity()
    {
        if(isChase)
        {
            rigid.velocity = Vector3.zero;

            rigid.angularVelocity = Vector3.zero;
        }

    }
    
    void Targeting()
    {
        float targetRadius = 0f;
        float targetRange = 0f;

        switch (enumType)
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case Type.B:
                targetRadius = 1f;
                targetRange = 12f;
                break;
            case Type C:
                targetRadius = 0.5f;
                targetRange = 25f;
                break;

        }


        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,//그냥 레이를 쏘게 되면 레이가 실처럼 가늘기 때문에'
                                                                        //공격 할수 있는 횟수가 작음
                                             targetRadius,//구체의 반지름
                                             transform.forward,
                                             targetRange,
                                             LayerMask.GetMask("Player"));
        if(rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        switch (enumType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;
                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                rigid.AddForce(transform.forward * 40, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.2f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);

                break;


            case Type C:
                yield return new WaitForSeconds(1f);
                GameObject instantBullet = Instantiate(bullet, transform.position,transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;

        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

    }

    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;   
            StartCoroutine(OnDamage(reactVec, false));

        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec, false));
        }

    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        if(curHealth >0)
        {
            mat.color = Color.white;
        }
        else
        {
            mat.color = Color.gray;
            gameObject.layer = 14;
            isChase = false;
            nav.enabled = false;

            anim.SetTrigger("doDie");

            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up *3;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);

                Destroy(gameObject, 4);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rigid.AddForce(reactVec * 5, ForceMode.Impulse);

                Destroy(gameObject, 4);
            }



        }
    }
}