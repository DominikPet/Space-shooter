using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class pickUpField : MonoBehaviour
{
    private GameObject go;
    Vector3 startPos;
    PowerUp pUp;
    float t = 0;

    public SphereCollider sphere;
    // Start is called before the first frame update
    void Start()
    {
        sphere = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    public void IncreasePickUpRadius(int increase)
    {
        sphere.radius += increase;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Pick up field collision for " + collision.gameObject.name);
        go = collision.gameObject;
        startPos = go.transform.position;
        pUp = go.GetComponent<PowerUp>();
    }

    private void PullPickUp() {
        if (t <= 1)
        {
            go.transform.position = Vector3.Lerp(startPos, Hero.S.transform.position, t);
        }
        else
        {
            t = 0;
            pUp = null;
        }
    }

    private void FixedUpdate()
    {
        if (pUp != null)
        {
            t += 0.1f;
            PullPickUp();
        }
    }
}
