using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class PlayerController : MonoBehaviour
{
    //Declare Variables
    //Player related vars
    public int playerID;      //The player's numerical ID.
    public string playerName; //The player's inputted name.
    public Color playerColor; //The player's inputted color.

    public bool playerIsAlive;        //If the player is alive and still in the game.
    public int playerScore;           //The player's score.
    public GameObject playerHeldItem; //The player's currently held item.
    public GameObject playerStartingItem; //The item the player starts with.
    private float playerUIPos;         //The player's UI x position.

    private int dropCooldown = 1 * 600; //The cooldown before another item can be dropped.
    private int dropTimer = 15 * 600;   //The time left until the item will automatically drop.
    
    public KeyCode playerControlLeft;  //The player's keybind to move left.
    public KeyCode playerControlRight; //The player's keybind to move right.
    public KeyCode playerControlDrop;  //The player's keybind to drop.

    //Self-Reference Vars
    public GameObject referenceHeldItem; //A reference to the player's held item above them.

    public GameObject referenceMarker;          //A reference to the player's triangluar marker piece.
    public GameObject referenceCooldownBar;     //A reference to the player's visual cooldown piece.
    public GameObject referenceDropLine;        //A reference to the player's drop line.
    public GameObject referenceOutOfGameEffect; //A reference to the player's out of game effect.
    public GameObject referenceGameManager;      //A reference to the GameManager object.

    [SerializeField] private TextMeshProUGUI referenceScoreText; //A reference to the player's score UI.
    [SerializeField] private TextMeshProUGUI referenceNameText;  //A reference to the player's name UI.

    // Start is called before the first frame update
    void Start()
    {
        //Update child parts and UI to the correct colours/text.
        referenceMarker.GetComponent<Renderer>().material.color = playerColor;
        referenceCooldownBar.GetComponent<Renderer>().material.color = playerColor;
        referenceDropLine.GetComponent<Renderer>().material.color = playerColor;
        referenceOutOfGameEffect.GetComponent<Renderer>().material.color = playerColor;

        referenceScoreText.color = playerColor;
        referenceNameText.color = playerColor;

        referenceNameText.text = playerName;

        //Disable the out of game effect so it is not visible.
        referenceOutOfGameEffect.SetActive(false);

        //Save the starting x position for player UI elements.
        playerUIPos = transform.position.x;

        //Get the first item
        CreateItem(false);

    }

    // Update is called once per frame
    void Update()
    {
        //Update the score text.
        referenceScoreText.text = playerScore.ToString() + ".P";

        //Update the cooldown bar visuals.
        referenceCooldownBar.transform.localScale = new Vector3(dropTimer * 0.00015f, referenceCooldownBar.transform.localScale.y, referenceCooldownBar.transform.localScale.z);

        //Update the held item's position.
        referenceHeldItem.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        //Prevent movement of player's UI components.
        referenceNameText.transform.position = new Vector3(playerUIPos, referenceNameText.transform.position.y, referenceNameText.transform.position.z);
        referenceScoreText.transform.position = new Vector3(playerUIPos, referenceScoreText.transform.position.y, referenceScoreText.transform.position.z);

        //Only let players control or drop items if they are still alive and the game has not ended.
        if (playerIsAlive == true && referenceGameManager.GetComponent<GameManager>().gameState == 1)
        {
            //Player movement controls, allow movement if held and not too close to the edges.
            if (Input.GetKey(playerControlLeft) & transform.position.x > -6.5)
            {
                transform.position = new Vector3((float)(transform.position.x - 0.005), transform.position.y, transform.position.z);
            }
            if (Input.GetKey(playerControlRight) & transform.position.x < 6.5)
            {
                transform.position = new Vector3((float)(transform.position.x + 0.005), transform.position.y, transform.position.z);
            }

            //Player dropping controls: Drop the item if pressed and it is off cooldown. OR if the drop timer reaches 0, forcing a drop.
            if ((Input.GetKeyDown(playerControlDrop) && dropCooldown == 0) || (dropTimer == 0))
            {
                DropItem();
                CreateItem(true);
            }

            //Reduce the cooldown if it is above 1.
            if (dropCooldown >= 1)
            {
                dropCooldown -= 1;
            }

            //Reduce the drop timer.
            if (dropTimer >= 1)
            {
                dropTimer -= 1;
            }
        }

        //If te player is not alive, then disable child objects.
        if (playerIsAlive == false)
        {
            referenceOutOfGameEffect.SetActive(true);
            referenceHeldItem.GetComponent<SpriteRenderer>().enabled = false;
            referenceDropLine.GetComponent<SpriteRenderer>().enabled = false;
            referenceCooldownBar.GetComponent<SpriteRenderer>().enabled = false;
            referenceScoreText.fontStyle = FontStyles.Italic;
            referenceNameText.fontStyle = FontStyles.Italic;
        }
    }

    //Function for dropping the held item.
    void DropItem()
    {
        //Set the held item to active so it drops, set it to active
        ItemController referenceItemController = referenceHeldItem.GetComponent<ItemController>();
        referenceItemController.itemActive = true;
        referenceHeldItem = null;
    }

    //Function for creating new items. If true, use the next queue item, if false, use the item set in the editor.

    void CreateItem(bool queueItem)
    {
        //Receive the most recent queue item and add it as the next held item.
        GameManager referenceGameManagerScript = referenceGameManager.GetComponent<GameManager>();
        if(queueItem == true)
        {
            playerHeldItem = referenceGameManagerScript.gameQueue[0];
            referenceGameManagerScript.gameQueue.RemoveAt(0);
        }
        if(queueItem == false)
        {
            playerHeldItem = playerStartingItem;
        }

        //Instantiate the new held item, and assign its owner/references.
        referenceHeldItem = Instantiate(playerHeldItem, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity) as GameObject;
        ItemController referenceItemController = referenceHeldItem.GetComponent<ItemController>();
        referenceItemController.itemOwner = playerID;
        referenceItemController.referenceOwnerScript = this.GetComponent<PlayerController>();
        referenceItemController.referenceGameManager = referenceGameManager;

        //Change colour if it is a orb/powerup, but not junk.
        if (referenceHeldItem.CompareTag("Orb") || referenceHeldItem.CompareTag("Powerup"))
        {
            referenceHeldItem.GetComponent<Renderer>().material.SetColor("_Color", playerColor);
        }

        //If the held item is a junk item, randomise its scale and rotation.
        if (referenceHeldItem.CompareTag("Junk"))
        {
            referenceHeldItem.transform.localScale = new Vector3(Random.Range(0.5f, 1.5f), Random.Range(0.5f, 1.5f), 1);
            referenceHeldItem.transform.eulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        }

        //Reset the drop cooldown and timer.
        dropCooldown = 1 * 600;
        dropTimer = 15 * 600;
    }


    //If hidden on game end, also hide the held item.
    private void OnDisable()
    {
        Destroy(referenceHeldItem);
    }
}
