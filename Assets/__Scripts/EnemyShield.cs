using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlinkColorOnHit))]
[DisallowMultipleComponent]
public class EnemyShield : MonoBehaviour
{
    [Header("Inscribed")]
    public float health = 10;

    private List<EnemyShield> protectors = new List<EnemyShield>();
    private BlinkColorOnHit blinker;
    // Start is called before the first frame update
    void Start()
    {
        blinker = GetComponent<BlinkColorOnHit>();
        blinker.ignoreOnCollisionEnter = true;

        if (transform.parent == null)
        {
            return;
        }

        EnemyShield shieldParent = transform.parent.GetComponent<EnemyShield>();
        if (shieldParent != null)
        {
            shieldParent.AddProtector(this);
        }
        
    }

    private void AddProtector(EnemyShield shieldChild)
    {
        protectors.Add(shieldChild);
    }

    public bool isActive
    {
        get { return gameObject.activeInHierarchy; }
        set
        {
            gameObject.SetActive(value);
        }
    }

    public float TakeDamage(float damage)
    {
        foreach(EnemyShield shield in protectors)
        {
            if (shield.isActive)
            {
                damage = shield.TakeDamage(damage);
            }
            if (damage == 0)
            {
                return 0;
            }
        }

        blinker.SetColors();

        health -= damage;
        if (health <= 0)
        {
            isActive = false;
            return -health;
        }

        return 0;
    }
    // Update is called once per frame
}
