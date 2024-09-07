using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    private int lifetime = 500;
    // Update is called once per frame
    void Update()
    {
        lifetime--;
        if (lifetime < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
