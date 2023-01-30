using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private AudioSource takeItem;
    [SerializeField] private AudioSource bugPlace;
    [SerializeField] private AudioSource bugRepellant;
    [SerializeField] private AudioSource fertilizerAudio;
    [SerializeField] private AudioSource Planting;
    [SerializeField] private AudioSource Watering;
    [SerializeField] private AudioSource Harvesting;

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
    //public Sprite[] playerStates;
    public itemType holdedItemType;
    public Animator playerAnimation;

    public LayerMask itemLayerMask;
    public LayerMask plotLayerMask;

    public Items[] items;
    private PlantPool pool;

    [Header("Sprite Change")]
    public int plantType = 0;

    public int score = 0;
    public static bool canMove = false;
    public Text scoreText;

    public enum states
    {
        none, holding
    };

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        scoreText.text = score.ToString();
        canMove = false;

        holdedItem = this.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        playerSprite = this.GetComponent<SpriteRenderer>();

        rb = this.GetComponent<Rigidbody2D>();

        pool = GameObject.FindGameObjectWithTag("Pool").GetComponent<PlantPool>();

        direction = 'd';

        changeState(states.none);
    }

    public void changeState(states newState, itemType _itemType = itemType.none, Sprite sprite = null, char direction = 'd')
    {
        state = newState;

        holdedItemType = _itemType;
        holdedItem.sprite = sprite;
    }

    Items highlightedItem, glowedItem;
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
        if(canMove) rb.MovePosition(rb.position + new Vector2(inputX, inputY) * speed);

        //convert direction to vector
        Vector2 directionVector = Vector2.zero;
        if (canMove)
        {
            switch (direction)
            {
                case 'u': directionVector = Vector2.up; break;
                case 'd': directionVector = Vector2.down; break;
                case 'l': directionVector = Vector2.left; break;
                case 'r': directionVector = Vector2.right; break;
            }
        }
        
        
        Vector2 checkVector = (Vector2)transform.position + Vector2.down * 0.75f;

        //check for item
        RaycastHit2D pickUpItem = Physics2D.Raycast(checkVector, directionVector, 2f, itemLayerMask);

        if (pickUpItem)
        {
           
            highlightedItem = pickUpItem.collider.gameObject.GetComponent<Items>();
            bool found = false;
            foreach (Items i in items)
            {
                if (i == highlightedItem)
                {
                    found = true;
                    highlightedItem.glow = true;
                    break;
                }
            }
            if (!found)
            {
                highlightedItem.glow = false;
                highlightedItem = null;
            }
        }
        else
        {
            foreach (Items i in items)
            {
                i.glow = false;
            }
        }

        //check for plot
        RaycastHit2D selectedPlot = Physics2D.Raycast(checkVector, directionVector, 2f, plotLayerMask);
        if (selectedPlot)
        {
            highlightedPlot = selectedPlot.collider.gameObject.GetComponent<Plot>();
        }

        //item and plot overlap
        if (Input.GetKey(actionKey) && canMove)
        {
            //pickup item
            if (pickUpItem && highlightedItem)
            {
                takeItem.Play();
                if (highlightedItem._itemType != itemType.keranjang)
                {
                    changeState(states.holding, highlightedItem._itemType, highlightedItem.gameObject.GetComponent<SpriteRenderer>().sprite, direction);
                }
                else if(highlightedItem._itemType == itemType.keranjang && 
                    holdedItemType == itemType.keranjang)
                {
                    highlightedItem.gameObject.GetComponent<SpriteRenderer>().sprite = highlightedItem.gameObject.GetComponent<Keranjang>().full;
                    highlightedItem.gameObject.GetComponentInChildren<ParticleSystem>().Play();
                    score++;
                    scoreText.text = score.ToString();
                    changeState(states.none, itemType.none, null, direction);
                }
                
            }
            //do action on plot
            if (selectedPlot && highlightedPlot)
            {
                //nanem
                if (holdedItemType == itemType.seed && !highlightedPlot.occupiedPlant)
                {
                    Planting.Play();
                    Plant newPlant = Instantiate(pool.plants[plantType], selectedPlot.transform);
                    newPlant.name = pool.plants[plantType].name;
                    newPlant.transform.localPosition = Vector3.up * 0.5f;
                    highlightedPlot.occupiedPlant = newPlant;
                    changeState(states.none, itemType.none, null, direction);
                }
                else if (highlightedPlot.occupiedPlant &&
                        highlightedPlot.occupiedPlant.name == pool.plants[plantType].name)
                {
                    if (holdedItemType == itemType.wateringcan)
                    {
                        Watering.Play();
                        highlightedPlot.occupiedPlant.KasihAir();
                        changeState(states.none, itemType.none, null, direction);
                    }
                    else if (holdedItemType == itemType.fertilizer)
                    {
                        fertilizerAudio.Play();
                        highlightedPlot.occupiedPlant.KasihFertilizer();
                        changeState(states.none, itemType.none, null, direction);
                    }
                    else if (holdedItemType == itemType.pesticide)
                    {
                        bugRepellant.Play();
                        highlightedPlot.occupiedPlant.KasihPesticide();
                        changeState(states.none, itemType.none, null, direction);
                    }
                    //panen
                    else if (highlightedPlot.occupiedPlant.currentPhase == Plant.Phases.fruit &&
                        holdedItemType != itemType.keranjang)
                    {
                        if (highlightedPlot.occupiedPlant.HarvestFruit())
                        {
                            Harvesting.Play();
                            changeState(states.holding, itemType.keranjang, highlightedPlot.occupiedPlant.fruit, direction);
                        }
                    }
                }
                else if (highlightedPlot.occupiedPlant &&
                        highlightedPlot.occupiedPlant.name != pool.plants[plantType].name)
                {
                    if (holdedItemType == itemType.pest)
                    {
                        bugPlace.Play();
                        highlightedPlot.occupiedPlant.KasihHama();
                        changeState(states.none, itemType.none, null, direction);
                    }
                }
            }
        }

        if(inputX == 0 && inputY == 0)
        {
            if (state == states.none)
            {
                switch (direction)
                {
                    case 'u': playerAnimation.Play("Player_idle_back"); break;
                    case 'd': playerAnimation.Play("Player_idle_front"); break;
                    case 'r': playerAnimation.Play("Player_idle_side"); break;
                    case 'l': playerAnimation.Play("Player_idle_side"); break;
                }

            }
            else if (state == states.holding)
            {
                switch (direction)
                {
                    case 'u': playerAnimation.Play("Player_idle_holding_back"); break;
                    case 'd': playerAnimation.Play("Player_idle_holding_front"); break;
                    case 'r': playerAnimation.Play("Player_idle_holding_side"); break;
                    case 'l': playerAnimation.Play("Player_idle_holding_side"); break;
                }
            }
        }
        else
        {
            if (state == states.none)
            {
                switch (direction)
                {
                    case 'u': playerAnimation.Play("Player_run_back"); break;
                    case 'd': playerAnimation.Play("Player_run_front"); break;
                    case 'r': playerAnimation.Play("Player_run_side"); break;
                    case 'l': playerAnimation.Play("Player_run_side"); break;
                }

            }
            else if (state == states.holding)
            {
                switch (direction)
                {
                    case 'u': playerAnimation.Play("Player_run_holding_back"); break;
                    case 'd': playerAnimation.Play("Player_run_holding_front"); break;
                    case 'r': playerAnimation.Play("Player_run_holding_side"); break;
                    case 'l': playerAnimation.Play("Player_run_holding_side"); break;
                }
            }
        }

        if (direction == 'l')
            playerSprite.flipX = true;
        else
            playerSprite.flipX = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Plot"))
        {
            collision.gameObject.GetComponent<Plot>().glow = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Plot"))
        {
            collision.gameObject.GetComponent<Plot>().glow = false;
            highlightedPlot = null;
        }
    }
}
