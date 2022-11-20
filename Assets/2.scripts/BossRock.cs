using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRock : Bullet
{
    Rigidbody rigid;
    float angularPower = 2f;
    float scaleValue = 0.01f;
    bool isShoot;
    
    

    void Awake()
    {
        Application.targetFrameRate = 60;
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
        transform.localScale = Vector3.one * 0.1f;

    }

    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(2.2f);
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while(!isShoot)
        {
            angularPower += 0.1f;
            transform.localScale += Vector3.one * scaleValue *1.2f;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null;

        }
    }

}
