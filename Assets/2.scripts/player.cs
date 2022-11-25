using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    private float hAxis;
    private float vAxis;

    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;
    public GameObject[] grenades;
    public int hasGrenades;
    public Camera followCamera;
    public GameObject grenadeObj;

    public int ammo;
    public int coin;
    public int health;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int MaxHasGrenades;

    bool wDown;
    bool jDown;
    bool fDown;
    bool gDown;
    bool rDown;
    bool iDown;
    bool isJump;
    bool isDodge;
    bool sDown1;
    bool sDown2;
    bool sDown3;
    bool isSwap;
    bool isFireReady = true;
    bool isReload = false;
    bool isBorder = false;
    bool isDamage;
    bool isShop;


    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    Animator anim;
    MeshRenderer[] meshs;

    GameObject nearObject;
    Weapon equipWeapon;

    int equipWeaponIndex = -1;
    float fireDelay;

    void Awake()
    {
        Application.targetFrameRate = 60;
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshs = GetComponentsInChildren<MeshRenderer>();

        PlayerPrefs.SetInt("MaxScore", 112500);

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Grenade();
        Attack();
        Reload();
        Dodge();
        Swap();
        Interation();

    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
        iDown = Input.GetButtonDown("Interation");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");
        fDown = Input.GetButton("Fire1");
        gDown = Input.GetButton("Fire2");

        rDown = Input.GetButtonDown("Reload");
    }

    void Move() {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
        {
            moveVec = dodgeVec;
        }
        if(isSwap || isReload || !isFireReady)
        {
            moveVec = Vector3.zero;
        }
        if(!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.7f : 1f) * Time.deltaTime;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        //#1. 키보드에 의한 회전
        transform.LookAt(transform.position + moveVec);
    
        //#2. 마우스에 의한 회전
        if(fDown)
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 1000))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);

            }
        }

    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap && !isReload)
        {
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");

            isJump = true;
        }
    }
    void Grenade()
    {
        if(hasGrenades == 0)
        {
            return;
        }
        if(gDown && !isReload && !isSwap)
        {

            Vector3 vector3 = Input.mousePosition;


            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100)){
                Vector3 nextVec = (rayHit.point - transform.position).normalized * 20;
                nextVec.y = 13;
                

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);
            }
        }
    }
    void Attack()
    {
        if(equipWeapon == null)
        {
            return;
        }
        fireDelay += Time.deltaTime; //공격딜레이에 시간을 더해주고 공격가능 여부를 확인
        isFireReady = equipWeapon.rate < fireDelay;

        if(fDown && isFireReady && !isDodge && !isSwap && !isReload && !isShop)
        {
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee? "doSwing": "doShot");
            fireDelay = 0;

        }
    }

    void Reload()
    {
        if(equipWeapon == null)
        {
            return;
        }

        if(equipWeapon.type == Weapon.Type.Melee)
        {
            return;
        }

        if (ammo == 0)
        {
            return;
        }

        if(equipWeapon.curAmmo == equipWeapon.maxAmmo)
        {
            return;
        }

        if (rDown && !isJump && !isDodge && !isSwap && isFireReady && !isReload && !isShop)
        {
            anim.SetTrigger("doReload");
            isReload = true;
            Invoke("Reloadout", 2f);
        }
    }

    void Reloadout()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo -= reAmmo;
        isReload = false;

    }


    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap && !isReload)
        {
            dodgeVec = moveVec;
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.3f);
        }
    }

    void DodgeOut()
    {
        speed *= 0.5f;
        isDodge = false;

    }
    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0))
            return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1))
            return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2))
            return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if((sDown1 || sDown2 || sDown3)&& !isJump && !isDodge)
        {
            if(equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }

    }

    void SwapOut()
    {
        isSwap = false;

    }

    void Interation()
    {
        if(iDown && nearObject != null && !isJump && !isDodge && !isSwap && !isReload)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }

            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
    }

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.black);
        isBorder = Physics.Raycast(transform.position, transform.forward , 5, LayerMask.GetMask("Wall"));
            }

    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();


    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);

            isJump = false;
        }
    }
    void OnTriggerStay(Collider other)
    {
       if(other.tag == "Weapon" || other.tag == "Shop")
            nearObject = other.gameObject;
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearObject = null;
        }

        else if (other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop= false;
            nearObject = null;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);
                    hasGrenades += item.value;
                    if (hasGrenades > MaxHasGrenades)
                        hasGrenades = MaxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if(other.tag == "EnemyBullet")
        {
            if(!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                bool isBossAtk = other.name == "Boss Melee Area";               
                StartCoroutine(OnDamage(isBossAtk));
            }
            if (other.GetComponent<Rigidbody>() != null)
            {
                Destroy(other.gameObject);
            }
        }





    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamage = true;

        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;

        }
        if(isBossAtk)
        {
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(1f);

        
        isDamage = false;
        if (isBossAtk)
        {
            rigid.velocity = Vector3.zero;
        }

        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;

        }
    }
}
