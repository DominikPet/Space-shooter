using Assets.__Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoundsCheck))]

public class ProjectileHero : MonoBehaviour
{
    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Dynamic")]
    public Rigidbody rigid;
    [SerializeField]
    private eWeaponType _type;

    public eWeaponType type
    {
        get { return _type; }
        set { SetType(value);}
    }

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (bndCheck.LocIs(BoundsCheck.eScreenLocs.offUp))
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
            if (Hero.S.weapons[0].type == eWeaponType.missile)
            {
                Main.S.impactExplosionPrefab.transform.position = this.gameObject.transform.position;
                Instantiate<GameObject>(Main.S.impactExplosionPrefab);
            }
            else
            {
                Main.S.impactBasicPrefab.transform.position = this.gameObject.transform.position;
                Instantiate<GameObject>(Main.S.impactBasicPrefab);
        }
    }

    public void SetType(eWeaponType eType)
    {
        _type = eType;
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(_type);
        rend.material.color = def.projectileColor;
    }

    public Vector3 vel
    {
        get
        {
            return rigid.velocity;
        }
        set
        {
            rigid.velocity = value;
        }
    }
}
