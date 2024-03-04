using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class playerControl : MonoBehaviour
{
    //Declare Variables
    public int playerID;
    public int playerScore;
    public bool playerIsAlive;
    private int playerCooldown;
    public string playerName;
    public GameObject playerHeldItem;
    public Color playerColor;
    private int dropCooldown = 1 * 600;
    [SerializeField] private int dropTimer = 15 * 600;
    

    [SerializeField] public KeyCode playerControlLeft;
    [SerializeField] public KeyCode playerControlRight;
    [SerializeField] public KeyCode playerControlDrop;

    private GameObject referenceHeldItem;

    [SerializeField] private GameObject referenceMarker;
    [SerializeField] private GameObject referenceCooldown;
    [SerializeField] private GameObject referenceDropLine;
    [SerializeField] private GameObject referenceOutOfGameLine;
    [SerializeField] public GameObject referenceGameManager;

    [SerializeField] private TextMeshProUGUI referenceScoreText;
    [SerializeField] private TextMeshProUGUI referenceIDText;

    private float startingXPos;

    // Start is called before the first frame update
    void Start()
    {
        //Update child parts to the correct color. Colours wont change during gameplay, so this can be in start
        referenceMarker.GetComponent<Renderer>().material.color = playerColor;
        referenceCooldown.GetComponent<Renderer>().material.color = playerColor;

        //Load in the first orb above the cursor and set its colour.
        referenceHeldItem = Instantiate(playerHeldItem, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity) as GameObject;
        referenceHeldItem.GetComponent<Renderer>().material.SetColor("_Color", playerColor);

        //Set text colours and values
        referenceScoreText.color = playerColor;
        referenceIDText.color = playerColor;
        referenceDropLine.GetComponent<Renderer>().material.SetColor("_Color", playerColor);
        referenceOutOfGameLine.GetComponent<Renderer>().material.SetColor("_Color", playerColor);
        referenceOutOfGameLine.SetActive(false);
        referenceIDText.text = playerName;

        //Get starting pos
        startingXPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        //Update held item visual
        referenceHeldItem.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        //Update Score
        referenceScoreText.text = playerScore.ToString() + ".P";

        //Prevent movement of child ui components
        referenceIDText.transform.position = new Vector3(startingXPos, referenceIDText.transform.position.y,referenceIDText.transform.position.z);
        referenceScoreText.transform.position = new Vector3(startingXPos, referenceScoreText.transform.position.y, referenceScoreText.transform.position.z);


        //Only let player do stuff if they are alive!
        if (playerIsAlive == true)
        {
            //Player left/right movement. Only allow movement inside the box area, with 0.5 off both sides.
            if (Input.GetKey(playerControlLeft) & transform.position.x > -6.5)
            {
                transform.position = new Vector3((float)(transform.position.x - 0.005), transform.position.y, transform.position.z);
            }
            if (Input.GetKey(playerControlRight) & transform.position.x < 6.5)
            {
                transform.position = new Vector3((float)(transform.position.x + 0.005), transform.position.y, transform.position.z);
            }

            //Orb Dropping
            //Reduce Cooldown
            if (dropCooldown >= 1)
            {
                dropCooldown -= 1;
            }

            //Player dropping
            if (Input.GetKeyDown(playerControlDrop) && dropCooldown == 0) //Check Cooldowns
            {
                //Debug.Log("dropping due to button press");
                DropItem();
            }

            //Reduce Timer
            if (dropTimer >= 1)
            {
                dropTimer -= 1;
                //Debug.Log(dropTimer);
            }

            //Force drop if timer expires
            if (dropTimer == 0)
            {
                //Debug.Log("dropping due to timer expire");
                DropItem();
            }

            //Update the cooldownblock visuals
            if (dropTimer > 0)
            {
                referenceCooldown.transform.localScale = new Vector3(dropTimer * 0.00015f, referenceCooldown.transform.localScale.y, referenceCooldown.transform.localScale.z);
            }
        } else
        {
            referenceOutOfGameLine.SetActive(true);
            referenceHeldItem.GetComponent<SpriteRenderer>().enabled = false;
            referenceDropLine.GetComponent<SpriteRenderer>().enabled = false;
            referenceCooldown.GetComponent<SpriteRenderer>().enabled = false;
            //referenceScoreText.fontStyle = FontStyle.BoldAndItalic;
            //referenceIDText.fontStyle = FontStyle.BoldAndItalic;
        }
    }

    void DropItem()
    {
        //Remove the reference and set itemActive to true so it falls.
        itemControl referenceHeldItemScript = referenceHeldItem.GetComponent<itemControl>();
        referenceHeldItemScript.itemActive = true;
        referenceHeldItemScript.itemOwner = playerID;
        referenceHeldItemScript.referenceOwnerScript = this.GetComponent<playerControl>();
        referenceHeldItem = null;

        //Receive the most recent queue item and add it as the next held item.
        GameManager referenceGameManagerScript = referenceGameManager.GetComponent<GameManager>();
        playerHeldItem = referenceGameManagerScript.gameQueue[0];
        referenceGameManagerScript.gameQueue.RemoveAt(0);

        referenceHeldItem = Instantiate(playerHeldItem, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity) as GameObject;

        //Change colour if it is a orb/powerup, but not junk!
        if (referenceHeldItem.CompareTag("Orb") || referenceHeldItem.CompareTag("Powerup"))
        {
            referenceHeldItem.GetComponent<Renderer>().material.SetColor("_Color", playerColor);
        }

        //If the held item is a junk item, randomise its scale and rotation
        if (referenceHeldItem.CompareTag("Junk"))
        {
            referenceHeldItem.transform.localScale = new Vector3(Random.Range(0.5f,1.5f),Random.Range(0.5f, 1.5f),1);
            referenceHeldItem.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        }

        //Reset cooldown and timer
        dropCooldown = 1 * 600;
        dropTimer = 15 * 600;
    }
}
