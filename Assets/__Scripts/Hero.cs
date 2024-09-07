using Assets.__Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S {  get; private set; }

    [Header("Inscribed")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public GameObject projectilePrefab;
    public Weapon[] weapons;

    [Header("Dynamic")]
    [Range(0, 4)]
    public float _shieldLevel = 1;

    [Tooltip("This field holds a reference to the last triggering GameObject")]
    private GameObject lastTriggerGo = null;

    public delegate void WeaponFireDelegate();
    public event WeaponFireDelegate fireEvent;    // Start is called before the first frame update
    void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }

        ClearWeapons();
        weapons[0].SetType(eWeaponType.blaster);
        weapons[1].SetType(eWeaponType.blaster);
        //fireEvent += TempFire;
    }

    // Update is called once per frame
    void Update()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(vAxis * pitchMult, hAxis * rollMult, 0);

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TempFire();
        //}

        if (Input.GetAxis("Jump") == 1 && fireEvent != null)
        {
            fireEvent();
        }

        if (Input.GetAxis("SpeedUpgrade") == 1 && Main.S.speedUpgradeButton.enabled == true)
        {
            Main.S.UpgradeSpeed();
        }
        if (Input.GetAxis("DelayUpgrade") == 1 && Main.S.shootingDelayButton.enabled == true)
        {
            Main.S.UpgradeShootingDelay();
        }
    }

    public void EnemyColided(Collider other) { 
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        Debug.Log("Shield trigger hit by: " + go.name);

        if (go == lastTriggerGo) return;
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        PowerUp pUp = go.GetComponent<PowerUp>();
        if (enemy != null)
        {
            shieldLevel--;
            Destroy(go);
        }
        else if (pUp!=null){
            AbsorbPowerUp(pUp);
        }
        else
        {
            Debug.Log("Shield trigger hit by non-Enemy" + go.name);
        }
    }

    private void AbsorbPowerUp(PowerUp pUp)
    {
        Debug.Log("Absorbed PowerUp: " +  pUp.type);
        switch (pUp.type) {
            case eWeaponType.shield:
                shieldLevel++;
                break;
            default:
                if (pUp.type == weapons[0].type)
                {
                    Weapon weap = GetEmptyWeaponSlot();
                    if (weap != null)
                    {
                        weap.SetType(pUp.type);
                    }
                }
                else
                {
                    ClearWeapons();
                    weapons[0].SetType(pUp.type);
                }
                    break; 
        }
        pUp.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel {  get { return _shieldLevel; }
        private set {
            _shieldLevel = MathF.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject);
                Main.HERO_DIED();
            }
        }
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == eWeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return null;
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(eWeaponType.none);
        }
    }
}
