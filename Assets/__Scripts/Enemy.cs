using Assets.__Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.SocialPlatforms.Impl;

[RequireComponent(typeof(BoundsCheck))]

public class Enemy : MonoBehaviour
{
    [Header("Inscribed")]
    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10;
    private int scorePerKill = 20;
    public TextMeshPro textScorePerKill;

    // Public property to access the field
    public virtual int ScorePerKill
    {
        get { return scorePerKill; }
        set { scorePerKill = value; }
    }

    public float powerUpDropChance = 1f;

    protected bool calledShipDestroyed = false;

    protected BoundsCheck bndCheck;

    public Vector3 pos
    {
        get { return this.transform.position; }
        set { this.transform.position = value; }
    }

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }


    // Update is called once per frame
    void Update()
    {
        Move();

        if (!bndCheck.isOnScreen)
        {
            if (pos.y < bndCheck.camHeight - bndCheck.radius)
            {
                Debug.Log(gameObject.name + " was destroyed, it was off-screen.");
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherGO = collision.gameObject;
        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();

        if (p != null) {
            if (bndCheck.isOnScreen)
            {
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if (health <= 0)
                {
                    Main.S.coins+= this.ScorePerKill /10;
                    Main.CheckUpgrades();
                    if (!calledShipDestroyed)
                    {
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED(this);
                    }
                    changeScore();
                    Destroy(this.gameObject);
                }
            }
        Destroy(otherGO);

    }
        else
        {
            Debug.Log("Enemy hit by non-projectile " + otherGO.name);
        }
    }

    public void changeScore()
    {
        Main.S.score += this.ScorePerKill;
        Main.enemySpawnPerSecond = (float)Math.Max((double)Main.S.score / Main.enemySpawnPerSecondEasing, (double)Main.enemySpawnPerSecond);
        textScorePerKill.text = "+" + ScorePerKill;
        textScorePerKill.transform.position = this.gameObject.transform.position;
        Instantiate<TextMeshPro>(textScorePerKill);
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }
}
