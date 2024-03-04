using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class itemControl : MonoBehaviour
{
    //Declare Variables
    public int itemOwner;
    public playerControl referenceOwnerScript;
    public bool itemActive = false;
    private int outOfBoundsTimer = 0;
    public int timeAlive = 0;
    
    private Rigidbody2D referenceRigidBody;
    private Collider2D referenceCollider;

    [SerializeField] private GameObject referenceNextEvolution;
    [SerializeField] private TextMeshProUGUI referenceOrbText;
    public int mergeValue;
    public string powerupType;
    private int powerupDecay = 0;
    private bool powerupHasTouched = false;

    // Start is called before the first frame update
    void Start()
    {
        //Get this rigidbody
        referenceRigidBody = GetComponent<Rigidbody2D>();
        referenceCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //If dropped is true, enable gravity/collision and begin out of bounds timers, otherwise disable.
        if (itemActive == true)
        {
            referenceRigidBody.WakeUp();
            referenceCollider.enabled = true;
            timeAlive += 1;
            if (transform.position.y >= 3)
            {
                outOfBoundsTimer += 1;
            }
        } else
        {
            referenceRigidBody.Sleep();
            referenceCollider.enabled = false;
        }

        //If out of bounds for more than x seconds, disable player. Only check for orbs, junk is fine.
        if(outOfBoundsTimer > 900 && gameObject.tag == "Orb")
        {
            //Debug.Log("Player is out!");
            referenceOwnerScript.playerIsAlive = false;
        }

        //Powerup despawn/decay timer, if over the timer, destroy this item.
        if(powerupHasTouched == true)
        {
            powerupDecay += 1;
            //Set opacity based on decay later on!
            if (powerupDecay >= 350)
            {
                Destroy(gameObject);
            }
        }

        //If it is an orb, keep orbtext at rotation 0.
        if(gameObject.tag == "Orb")
        {
            if(referenceOrbText != null)
            {
                referenceOrbText.transform.localRotation = Quaternion.Inverse(this.gameObject.transform.rotation);
            }
        }
    }

    //Combining Logic
    void OnCollisionEnter2D(Collision2D collision)
    {
        //Only do this if it is a orb, otherwise dont check at all
        if (gameObject.tag == "Orb")
        {
            itemControl referenceCollisionScript = collision.gameObject.GetComponent<itemControl>(); //Get script reference
            if (gameObject.tag == "Orb" && collision.gameObject.tag == "Orb" && timeAlive > referenceCollisionScript.timeAlive && itemOwner == referenceCollisionScript.itemOwner && mergeValue == referenceCollisionScript.mergeValue && mergeValue < 7) //Check if the same tag, check if both have the same owner and check if older than the other (to make it only happen once). Check if its been alive for a bit to avoid instant merging.
            {
                //Debug.Log("Merging!");

                //Create the new object and set its colour.
                var referenceNewItem = Instantiate(referenceNextEvolution, new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z), Quaternion.identity) as GameObject;
                var renderer = referenceNewItem.GetComponent<Renderer>();
                renderer.material.SetColor("_Color", GetComponent<Renderer>().material.color);


                //Add points
                referenceOwnerScript.playerScore += mergeValue * 2;

                //Set variables for the new object.
                itemControl referenceItemControlScript = referenceNewItem.GetComponent<itemControl>();
                referenceItemControlScript.itemOwner = this.itemOwner;
                referenceItemControlScript.referenceOwnerScript = this.referenceOwnerScript;
                referenceItemControlScript.itemActive = true;

                //Destroy both now that the new one is made.
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
            }
            if (gameObject.tag == "Orb" && collision.gameObject.tag == "Orb" && timeAlive == referenceCollisionScript.timeAlive && itemOwner == referenceCollisionScript.itemOwner && mergeValue == referenceCollisionScript.mergeValue && mergeValue < 7) //If both somehow have the same time alive, add random values so they will become unsynced and can combine.
            {
                timeAlive = Random.Range(1, 2);
                Debug.Log("Merge conflict detected! Fixing...");
            }
        }

        //Only check this for powerups
        if(gameObject.tag == "Powerup")
        {
            itemControl referenceCollisionScript = collision.gameObject.GetComponent<itemControl>(); //Get script reference
            //Eraser Powerup
            if (powerupType == "Eraser" && gameObject.tag == "Powerup" && collision.gameObject.tag == "Orb" && itemActive == true && referenceCollisionScript.itemActive == true) //Check if the powerup tag and touching, and that item is not being held by another player.
            {
                powerupHasTouched = true;
                Destroy(collision.gameObject);
            }

            //PaintBucket Powerup
            if (powerupType == "PaintBucket" && gameObject.tag == "Powerup" && collision.gameObject.tag == "Orb" && itemActive == true && referenceCollisionScript.itemActive == true) //Check if the powerup tag and touching, and that item is not being held by another player.
            {
                powerupHasTouched = true;

                //set the collision's new owner and colour
                referenceCollisionScript.itemOwner = this.itemOwner;
                collision.gameObject.GetComponent<Renderer>().material.SetColor("_Color", this.GetComponent<Renderer>().material.color); //<--- wtf is this goop
            }
        }
    }
}
