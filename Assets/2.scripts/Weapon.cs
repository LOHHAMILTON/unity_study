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


    public BoxCollider meleeArea;//������ �������� �ڽ�
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
    IEnumerator Swing() // ����Ƽ���� ���� �߿��� ������ �ڷ�ƾ
    {
        //yield ����� �����ϴ� Ű����
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
    IEnumerator Shot() // ����Ƽ���� ���� �߿��� ������ �ڷ�ƾ
    {
        //#1. �Ѿ� �߻�
        GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;

        yield return null;
        //#2. ź�ǹ���
        GameObject instantCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody CaseRigid = instantBullet.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up*Random.Range(2, 3);
        CaseRigid.AddForce(caseVec, ForceMode.Impulse);
        CaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);

    }
}

//Use() ���� ��ƾ -> Swing() �����ƾ -> Use() ���η�ƾ
//Use() ���� ��ƾ + Swing() �ڷ�ƾ(Co-Op) -> Use() ���η�ƾ

