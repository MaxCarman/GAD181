using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class ItemController : MonoBehaviour
{
    //Declare Variables
    //Item related vars
    public int itemOwner;           //The id of this object's player owner.
    public bool itemActive = false; //if this item is active. Is used for physics, collisions, and OutOfBounds timers.

    private int timerOutOfBounds = 0; //How many ticks this item has been out of bounds for.
    public int timerExisted = 0;      //How many ticks this item has existed for.

    //Orb specific vars
    public int orbValue;                                        //The size and point value of this orb. Should be 1-7 for orbs, or 0 for powerups and junk.
    [SerializeField] private GameObject referenceNextEvolution; //The object this orb evolves into after merging.

    //Powerup specific vars
    public string powerupType;               //The type of powerup this item is, is either Eraser or Paint.
    private int powerupDecay = 0;            //How many thics this item has existed for after touching an orb.
    private bool powerupHasTouched = false;  //If this powerup has touched an orb in its life.
    public Sprite paintStyle; //The stored style of the player, which is used for the paint powerup.

    //Self-Reference vars
    public PlayerController referenceOwnerScript;              //A reference to this script.
    [SerializeField] private TextMeshProUGUI referenceOrbText; //A reference to this object's value text. Only exists for orbs.
    public GameObject referenceGameManager;                   //A reference to the GameManager object.

    [SerializeField] private Rigidbody2D referenceRigidBody; //A reference to this object's rigid body.
    [SerializeField] private Collider2D referenceCollider;   //A reference to this object's collider.

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //If the item is active, wake up physics/colliders and increase its timer. Otherwise, disable physics and dont check timers.
        if (itemActive == true)
        {
            referenceRigidBody.WakeUp();
            referenceCollider.enabled = true;

            //Increase its time alive. If it is above the hazard line, also increase its OutOfBounds timer.
            timerExisted += 1;
            if (transform.position.y >= 3)
            {
                timerOutOfBounds += 1;
            }

            //If the item is below the map, delete it.
            if(transform.position.y <= -10)
            {
                Destroy(gameObject);
            }
        } else
        {
            referenceRigidBody.Sleep();
            referenceCollider.enabled = false;
        }

        //If out of bounds for more than x seconds, set the player to be out. Only check for orbs.
        if(timerOutOfBounds > 1.2 * 120 && gameObject.tag == "Orb" && referenceOwnerScript.playerIsAlive == true && referenceGameManager.GetComponent<GameManager>().gameState == 1)
        {
            var referenceOutAudio = Instantiate(referenceGameManager.GetComponent<GameManager>().referenceOutAudio, new Vector3(transform.position.x, transform.position.y, transform.position.z - 1), Quaternion.identity) as GameObject;
            referenceOwnerScript.playerIsAlive = false;
        }

        //if this object is a powerup and has touched a orb, increase its decay timer.
        if(powerupHasTouched == true)
        {
            powerupDecay += 1;
            //If decayed for x seconds, destroy this powerup.
            if (powerupDecay >= 0.5 * 120)
            {
                Destroy(gameObject);
            }
        }

        //If this object is a orb with a text child object, lock that childs rotation to always be 0 by inversing this object's rotation.
        if(gameObject.tag == "Orb" && referenceOrbText != null)
        {
            referenceOrbText.transform.localRotation = Quaternion.Inverse(this.gameObject.transform.rotation);
        }

        //If this object is invisble, also disable the orb text.
        if(GetComponent<SpriteRenderer>().enabled == false && gameObject.tag == "Orb" && referenceOrbText != null)
        {
            referenceOrbText.enabled = false;
        }   
    }

    //Merging/Powerup Logic
    void OnCollisionEnter2D(Collision2D collision)
    {
        //Get script reference
        ItemController referenceCollisionScript = collision.gameObject.GetComponent<ItemController>();

        //Orb merging logic
        if (gameObject.tag == "Orb" && referenceGameManager.GetComponent<GameManager>().gameState == 1)
        {
            //Check if the same tag, check if both have the same owner and check if older than the other (to make it only happen once). Check if its been alive for a bit to avoid instant merging.
            if (collision.gameObject.tag == "Orb" && timerExisted > referenceCollisionScript.timerExisted && itemOwner == referenceCollisionScript.itemOwner && orbValue == referenceCollisionScript.orbValue && orbValue < 7)
            {
                //Create the new object and set its colour/style
                var referenceNewItem = Instantiate(referenceNextEvolution, new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z), Quaternion.identity) as GameObject;
                referenceNewItem.GetComponent<Renderer>().material.SetColor("_Color", GetComponent<Renderer>().material.color);
                referenceNewItem.GetComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;


                //Add points to the player based on the merge value.
                referenceOwnerScript.playerScore += orbValue * 2;

                //Carry over this orbs's values onto the next object.
                ItemController referenceItemControlScript = referenceNewItem.GetComponent<ItemController>();
                referenceItemControlScript.itemOwner = this.itemOwner;
                referenceItemControlScript.referenceOwnerScript = this.referenceOwnerScript;
                referenceItemControlScript.itemActive = true;
                referenceItemControlScript.referenceGameManager = this.referenceGameManager;
                referenceItemControlScript.timerExisted = timerExisted;

                //Create a MergeParticle and transfer scale/color. Increase itensity by multiplying by orb value. Main values need a main variable due to unity limitations.
                var referenceMergeParticle = Instantiate(referenceGameManager.GetComponent<GameManager>().referenceMergeParticle, new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z - 1), Quaternion.identity) as ParticleSystem;
                referenceMergeParticle.emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 8 + ((float)orbValue)*2)});

                var referenceMergeParticleMain = referenceMergeParticle.main;
                referenceMergeParticleMain.startColor = GetComponent<Renderer>().material.color;
                referenceMergeParticleMain.startSize = 0.5f + ((float)orbValue/4) * Random.Range(0.5f,1f);

                //Create a MergeAudio and set its pitch according to the orbValue.
                var referenceMergeAudio = Instantiate(referenceGameManager.GetComponent<GameManager>().referenceMergeAudio, new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z - 1), Quaternion.identity) as GameObject;
                referenceMergeAudio.GetComponent<audioController>().audioSource.pitch = 0.7f + (float)orbValue * 0.1f;

                //Destroy both now that the new one is made.
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
            }

            //If they both somehow have the same time alive, add random values so they will become unsynced and can combine. This SHOULD fix merging problems.
            if (collision.gameObject.tag == "Orb" && timerExisted == referenceCollisionScript.timerExisted && itemOwner == referenceCollisionScript.itemOwner && orbValue == referenceCollisionScript.orbValue && orbValue < 7)
            {
                timerExisted = Random.Range(1, 10);
                Debug.Log("Merge conflict detected! Fixing...");
            }
        }

        //Powerup functionality
        if(gameObject.tag == "Powerup" && referenceGameManager.GetComponent<GameManager>().gameState == 1)
        {
            //Erase Powerup: Check if the powerup tag and touching, and that item is not being held by another player.
            if (powerupType == "Eraser" && gameObject.tag == "Powerup" && collision.gameObject.tag == "Orb" && itemActive == true && referenceCollisionScript.itemActive == true)
            {
                powerupHasTouched = true;

                //Create a EraseParticle. Increase itensity by multiplying by orb value. Main values need a main variable due to unity limitations.
                var referenceEraseParticle = Instantiate(referenceGameManager.GetComponent<GameManager>().referenceEraseParticle, new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z - 1), Quaternion.identity) as ParticleSystem;
                referenceEraseParticle.emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 8 + orbValue/2) });

                var referenceEraseParticleMain = referenceEraseParticle.main;
                referenceEraseParticleMain.startColor = GetComponent<Renderer>().material.color;
                referenceEraseParticleMain.startSize = 0.8f + ((float)referenceCollisionScript.orbValue * 0.1f) * Random.Range(1.2f, 1.8f);

                //Create a EraseAudio and set its pitch according to the orbValue.
                var referenceEraseAudio = Instantiate(referenceGameManager.GetComponent<GameManager>().referenceEraseAudio, new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z - 1), Quaternion.identity) as GameObject;
                referenceEraseAudio.GetComponent<audioController>().audioSource.pitch = 0.5f + (float)referenceCollisionScript.orbValue * 0.2f;

                //Destroy the other object.
                Destroy(collision.gameObject);
            }

            //Paint Powerup: //Check if the powerup tag and touching, and that item is not being held by another player.
            if (powerupType == "Paint" && gameObject.tag == "Powerup" && collision.gameObject.tag == "Orb" && itemActive == true && referenceCollisionScript.itemActive == true)
            {
                powerupHasTouched = true;

                //If it is not the same colour, Create a PaintParticle and PaintAudio. Increase itensity by multiplying by orb value. Main values need a main variable due to unity limitations.
                if(referenceCollisionScript.itemOwner != this.itemOwner)
                {
                    //Particles
                    var referencePaintParticle = Instantiate(referenceGameManager.GetComponent<GameManager>().referencePaintParticle, new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z - 1), Quaternion.identity) as ParticleSystem;
                    referencePaintParticle.emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 8 + orbValue/2)});

                    var referencePaintParticleMain = referencePaintParticle.main;
                    referencePaintParticleMain.startColor = GetComponent<Renderer>().material.color;
                    referencePaintParticleMain.startSize = 0.8f + ((float)referenceCollisionScript.orbValue * 0.1f) * Random.Range(1f, 1.5f);

                    //Audio
                    var referencePaintAudio = Instantiate(referenceGameManager.GetComponent<GameManager>().referencePaintAudio, new Vector3(collision.transform.position.x, collision.transform.position.y, collision.transform.position.z - 1), Quaternion.identity) as GameObject;
                    referencePaintAudio.GetComponent<audioController>().audioSource.pitch = 0.5f +(float)referenceCollisionScript.orbValue * 0.2f;
                }

                //Set the orb's new colour/style and owner ID.
                referenceCollisionScript.itemOwner = this.itemOwner;
                collision.gameObject.GetComponent<SpriteRenderer>().sprite = this.paintStyle;
                collision.gameObject.GetComponent<Renderer>().material.SetColor("_Color", this.GetComponent<Renderer>().material.color);
            }
        }
    }
}
