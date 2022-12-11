using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plot : MonoBehaviour
{
    public bool isOccupied = false;
    public Plant occupiedPlant;

    public bool glow;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    public void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (glow)
        {
            float colorValue = (((Mathf.Asin(Mathf.Sin(Time.time * 5f)))) + (Mathf.PI / 2)) / Mathf.PI;
            colorValue = (colorValue + 2) / 3;
            sr.color = new Color(colorValue, colorValue, colorValue, 1);
        }
        else
        {
            sr.color = Color.white;
        }
    }
}
