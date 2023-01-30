using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    private float length;
    public float parallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(-Mathf.Repeat(Time.time * parallaxEffect, length), transform.position.y, transform.position.z);
    }
}
