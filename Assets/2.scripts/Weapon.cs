using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;

    public int damage;
    public float rate;
    public int maxAmmo;
    public int curAmmo;


    public BoxCollider meleeArea;//범위를 지정해줄 박스
    public TrailRenderer trailEffect;
    public Transform bulletPos;
    public GameObject bullet;
    public Transform bulletCasePos;
    public GameObject bulletCase;


    // Update is called once per frame

    public void Use()
    {
        if(type == Type.Melee)
        {
            StopCoroutine("Swing");

            StartCoroutine("Swing");
        }

        else if(type == Type.Range && curAmmo >0 )
        {
            curAmmo--;
            StartCoroutine("Shot");

        }
    }
    IEnumerator Swing() // 유니티에서 가장 중요한 개념인 코루틴
    {
        //yield 결과를 전달하는 키워드
        //1
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        //2
        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;
        //3
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }
    IEnumerator Shot() // 유니티에서 가장 중요한 개념인 코루틴
    {
        //#1. 총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;
        //#2. 탄피배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody CaseRigid = instantBullet.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up*Random.Range(2, 3);
        CaseRigid.AddForce(caseVec, ForceMode.Impulse);
        CaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);

    }
}

//Use() 메인 루틴 -> Swing() 서브루틴 -> Use() 메인루틴
//Use() 메인 루틴 + Swing() 코루틴(Co-Op) -> Use() 메인루틴

