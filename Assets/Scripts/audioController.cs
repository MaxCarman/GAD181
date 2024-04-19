using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Attached to each sound object. After a vreif period, the object is destroyed so that empty sound objects dont build up and cause issues as the game progresses.
public class audioController : MonoBehaviour
{
    int timer = 0; //The timer until the object despawns.
    public AudioSource audioSource; //A reference to the child audio source object.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += 1;
        if(timer >= 5 * 120)
        {
            Destroy(gameObject);
        }
    }
}
