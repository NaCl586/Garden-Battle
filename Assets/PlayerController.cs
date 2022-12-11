﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Keybind and Movement")]
    public KeyCode moveForward;
    public KeyCode moveBackward;
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode actionKey;

    public float speed = 0.1f;
    private states state;
    
    [HideInInspector] public char direction;

    [Header("Sprite Change")]
    private SpriteRenderer holdedItem;
    private SpriteRenderer playerSprite;
    public Sprite[] playerStates;
    public itemType holdedItemType;

    public LayerMask itemLayerMask;
    public LayerMask plotLayerMask;

    public Items[] items;
    private PlantPool pool;

    [Header("Sprite Change")]
    public int plantType = 0;

    public enum states
    {
        none, holding
    };

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        holdedItem = this.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        playerSprite = this.GetComponent<SpriteRenderer>();

        rb = this.GetComponent<Rigidbody2D>();

        pool = GameObject.FindGameObjectWithTag("Pool").GetComponent<PlantPool>();

        direction = 'd';

        changeState(states.none);
    }

    public void changeState(states newState, itemType _itemType = itemType.none, Sprite sprite = null)
    {
        state = newState;
        if(state == states.none)
            playerSprite.sprite = playerStates[0];
        else if (state == states.holding)
            playerSprite.sprite = playerStates[1];

        holdedItemType = _itemType;
        holdedItem.sprite = sprite;
    }

    Items highlightedItem;
    Plot highlightedPlot;

    // Update is called once per frame
    void Update()
    {
        int inputX = 0, inputY = 0;
        if (Input.GetKey(moveForward)) 
        {
            inputY = 1;
            direction = 'u';
        }
        if (Input.GetKey(moveBackward))
        {
            inputY = -1;
            direction = 'd';
        }
        if (Input.GetKey(moveLeft))
        {
            inputX = -1;
            direction = 'l';
        }
        if (Input.GetKey(moveRight))
        {
            inputX = 1;
            direction = 'r';
        }
        rb.MovePosition(rb.position + new Vector2(inputX, inputY) * speed);

        //convert direction to vector
        Vector2 directionVector = Vector2.zero;
        switch (direction)
        {
            case 'u': directionVector = Vector2.up; break;
            case 'd': directionVector = Vector2.down; break;
            case 'l': directionVector = Vector2.left; break;
            case 'r': directionVector = Vector2.right; break;
        }
        
        Vector2 checkVector = (Vector2)transform.position + Vector2.down * 0.75f;

        //check for item
        RaycastHit2D pickUpItem = Physics2D.Raycast(checkVector, directionVector, 2f, itemLayerMask);

        if (pickUpItem)
        {
            highlightedItem = pickUpItem.transform.gameObject.GetComponent<Items>();
            highlightedItem.glow = true;
        }
        else
        {
            foreach (Items i in items)
                i.glow = false;
        }

        //check for plot
        RaycastHit2D selectedPlot = Physics2D.Raycast(checkVector, directionVector, 2f, plotLayerMask);

        if (selectedPlot)
        {
            highlightedPlot = selectedPlot.transform.gameObject.GetComponent<Plot>();
            highlightedPlot.glow = true;
        }
        else if(highlightedPlot)
        {
            highlightedPlot.glow = false;
        }

        //item and plot overlap
        if (Input.GetKey(actionKey))
        {
            if (pickUpItem)
            {
                changeState(states.holding, highlightedItem._itemType, highlightedItem.gameObject.GetComponent<SpriteRenderer>().sprite);
            }
            if (selectedPlot && highlightedPlot)
            {
                //nanem
                if (holdedItemType == itemType.seed)
                {
                    if (!highlightedPlot.occupiedPlant)
                    {
                        Plant newPlant = Instantiate(pool.plants[plantType], selectedPlot.transform);
                        newPlant.transform.localPosition = Vector3.up * 0.5f;
                        highlightedPlot.occupiedPlant = newPlant;
                    }
                    changeState(states.none);
                }
                else if (holdedItemType == itemType.wateringcan)
                {
                    highlightedPlot.occupiedPlant.KasihAir();
                    changeState(states.none);
                }
                else if (holdedItemType == itemType.fertilizer)
                {
                    highlightedPlot.occupiedPlant.KasihFertilizer();
                    changeState(states.none);
                }
                else if (holdedItemType == itemType.pesticide)
                {
                    highlightedPlot.occupiedPlant.KasihPesticide();
                    changeState(states.none);
                }
                else if (holdedItemType == itemType.pest)
                {
                    highlightedPlot.occupiedPlant.KasihHama();
                    changeState(states.none);
                }
                //panen
                else 
                {
                    if (highlightedPlot.occupiedPlant.HarvestFruit())
                    {
                        changeState(states.holding, itemType.keranjang, highlightedPlot.occupiedPlant.fruit);
                    }
                }
            }
        }
        
    }
}