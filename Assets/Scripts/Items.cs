using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum itemType
{
    none,
    keranjang,
    fertilizer,
    pest,
    wateringcan,
    seed,
    pesticide
};

public class Items : MonoBehaviour
{
    public int player;
    public itemType _itemType;

    public bool glow = false;
    private SpriteRenderer sr;

    public void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        if (glow)
        {
            float colorValue = (((Mathf.Asin(Mathf.Sin(Time.time * 5f)))) + (Mathf.PI/2)) / Mathf.PI;
            colorValue = (colorValue + 2) / 3;
            sr.color = new Color(colorValue, colorValue, colorValue, 1);
        }
        else
        {
            sr.color = Color.white;
        }
    }
}
