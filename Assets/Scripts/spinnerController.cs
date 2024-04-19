using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerController : MonoBehaviour
{
    //Delcare variables
    [SerializeField] private float spinnerTurnSpeed; //Defines the turn speed/direction.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Spin based on the spinner turn speed.
        gameObject.transform.Rotate(0,0,spinnerTurnSpeed);
    }
}
