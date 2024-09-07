using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backgroundScroller : MonoBehaviour
{
    [SerializeField] float speed = 0.5f;
    float offset;
    Material mat;


    void Start()
    {
        mat = GetComponent<Renderer>().material;   
    }

    // Update is called once per frame
    void Update()
    {
        offset += Time.deltaTime * speed / 10f;
        mat.SetTextureOffset("_MainTex", new Vector2 (offset,0));
    }
}
