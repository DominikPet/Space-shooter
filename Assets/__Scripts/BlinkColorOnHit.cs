using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkColorOnHit : MonoBehaviour
{
    private static float blinkDuration = 0.1f;
    private static Color blinkColor = Color.red;

    [Header("Dynamic")]
    public bool showingColor = false;
    public float blinkCompleteTime;
    public bool ignoreOnCollisionEnter = false;

    private Material[] materials;
    private Color[] originalColors;
    private BoundsCheck bndCheck;
    private Animator animator;

    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        materials = Utils.GetAllMaterialsFromChildren(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (animator != null)
        {
            animator.SetTrigger("ShipHit"); 
        }
        if (ignoreOnCollisionEnter) {  return; }
        ProjectileHero p = collision.gameObject.GetComponent<ProjectileHero>();
        if (p!=null)
        {
            if (bndCheck != null && !bndCheck.isOnScreen) {
                return;
            }
            SetColors();
        }
    }

    public void SetColors()
    {
        foreach (Material m in materials)
        {
            m.color = blinkColor;
        }
        showingColor = true;
        blinkCompleteTime = Time.time + blinkDuration;
    }

    void RevertColors()
    {
        for (int i = 0; i < materials.Length;i++)
        {
            materials[i].color = originalColors[i];
        }
    }
    void Update()
    {
        if (showingColor && Time.time > blinkCompleteTime)
        {
            RevertColors();
        }
    }
}
