using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static BoundsCheck;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EnemyShield))]
public class Enemy_4 : Enemy
{
    [Header("Inscribed")]
    private EnemyShield[] allShields;
    private EnemyShield thisShield;
    public float duration = 4;
    private Vector3 p0, p1;
    private float timeStart;
    public override int ScorePerKill
    {
        get { return base.ScorePerKill * 3; } // Override the getter, for example
        set { base.ScorePerKill = value; }    // You can override the setter as well if needed
    }

    private void Start()
    {
        allShields = GetComponentsInChildren<EnemyShield>();
        thisShield = GetComponent<EnemyShield>();

        p0 = p1 = pos;
        InitMovement();
    }

    private void InitMovement()
    {
        p0 = p1;
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);


        if (p0.x * p1.x > 0 && p0.y * p1.y > 0)
        {
            if (Mathf.Abs(p0.x) > Mathf.Abs (p0.y))
            {
                p1.x *= -1;
            }
            else
            {
                p1.y *= -1;
            }
        }
        timeStart = Time.time;
    }

    public override void Move()
    {
        BoundsCheck bnd = GetComponent<BoundsCheck>();
        bnd.screenLocs = eScreenLocs.onScreen;
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = u - 0.15f * Mathf.Sin(u * 2 * Mathf.PI);
        pos = (1-u) * p0 + p1*u;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherGO = collision.gameObject;
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null)
        {
            Destroy(otherGO);

            if (bndCheck.isOnScreen)
            {
                GameObject hitGO = collision.contacts[0].thisCollider.gameObject;
                if (hitGO == otherGO)
                {
                    hitGO = collision.contacts[0].otherCollider.gameObject;
                }

                float dmg = Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;

                bool shieldFound = false;
                foreach (EnemyShield s in allShields)
                {
                    if (s != hitGO) {
                        s.TakeDamage(dmg);
                        shieldFound = true;
                    }
                }

                if (!shieldFound) thisShield.TakeDamage(dmg);

                if (thisShield.isActive) return;

                if (!calledShipDestroyed)
                {
                    Main.S.coins += this.ScorePerKill / 10;
                    Main.CheckUpgrades();
                    Main.SHIP_DESTROYED(this);
                    changeScore();
                    calledShipDestroyed = true;
                }

                Destroy(gameObject);

            }
        }
        else
        {
            Debug.Log ("Enemy_4 hit by non-ProjectileHero: " + otherGO.name);
        }
    }

}
