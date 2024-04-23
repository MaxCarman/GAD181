using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextController : MonoBehaviour
{
    //Declare variables
    public float floatSpeed; //How fast the text rises upwards/downwards.
    public float fadeSpeed;  //How fast the text fades.
    public TextMeshProUGUI referenceText; //The reference to this object's text.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move upwards
        transform.position = new Vector3(transform.position.x,transform.position.y + floatSpeed,transform.position.z);

        //Decrease opacity
        float newOpacity = referenceText.color.a;
        newOpacity -= fadeSpeed;

        referenceText.color = new Color(referenceText.color.r,referenceText.color.g,referenceText.color.b,newOpacity);

        //If opacity is 0, delete the object.
        if(referenceText.color.a <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
