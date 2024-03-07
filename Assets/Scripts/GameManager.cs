using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Search;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    //Declare variables
    //Queue related vars
    public List<GameObject> gameQueue; //The list of gameobjects currently in the queue.
    [SerializeField] private List<GameObject> referenceItems;          //A list of all the items that can appear in the queue. Goes [Orb1 -> Orb7, JunkSquare, JunkTriangle, PowerupEraser, PowerupPaint]
    [SerializeField] private List<Image> referenceQueueImages;         //A reference to the 7 queue image slots.
    [SerializeField] private List<TextMeshProUGUI> referenceQueueText; //A reference to the 7 queue text slots.

    //Player related vars
    [SerializeField] private int playerCount;          //The amount of players playing the game.
    [SerializeField] private List<string> playerNames; //A list of the player names.
    [SerializeField] private List<Color> playerColors; //A list of the player colours.

    [SerializeField] private List<KeyCode> playerControlsLeft;  //A list of the player left buttons
    [SerializeField] private List<KeyCode> playerControlsRight; //A list of the player right buttons.
    [SerializeField] private List<KeyCode> playerControlsDrop;  //A list of the player drop buttons.

    [SerializeField] private GameObject referenceMarker; //A reference to the player marker gameobject.

    // Start is called before the first frame update
    void Start()
    {
        SetupQueue(20, 10, 4, 1, 0, 0, 0, 1, 1, 1, 1);
        //Setup players from the list
        for(int i = 0; i < playerCount; i++)
        {
            //Create player marker
            GameObject referencePlayerMarker = Instantiate(referenceMarker, new Vector3(Mathf.Clamp((float)i / playerCount * 14 - 7,-6,6), 3.5f,0), Quaternion.identity) as GameObject;

            //Get the new player's script and set relevent variables.
            PlayerController referencePlayerControl = referencePlayerMarker.GetComponent<PlayerController>();
            referencePlayerControl.playerID = i+1;
            referencePlayerControl.playerColor = playerColors[i];
            referencePlayerControl.playerControlLeft = playerControlsLeft[i];
            referencePlayerControl.playerControlRight = playerControlsRight[i];
            referencePlayerControl.playerControlDrop = playerControlsDrop[i];
            referencePlayerControl.referenceGameManager = this.gameObject;
            referencePlayerControl.playerName = playerNames[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        //If there are only 8 values in the list left, add another bag to the end.
        if (gameQueue.Count <= 8)
        {
            SetupQueue(20, 10, 4, 1, 0, 0, 0, 1, 1, 1, 1);
        }

        //Update visuals for the shown queue.
        for (int i = 0; i < 7; i++)
        {
            //Set the image
            referenceQueueImages[i].sprite = gameQueue[i].GetComponent<SpriteRenderer>().sprite;
            referenceQueueImages[i].transform.localScale = gameQueue[i].transform.localScale;

            //If the queue item is a orb, then update the text to show its orbValue, otherwise hide the text
            if (gameQueue[i].gameObject.tag == "Orb")
            {
                referenceQueueText[i].enabled = true;
                ItemController referenceItemController = gameQueue[i].GetComponent<ItemController>();
                referenceQueueText[i].text = referenceItemController.orbValue.ToString();
            } else
            {
                referenceQueueText[i].enabled = false;
            }
        }

    }

    //Ammends a new, full queue of prefabs based on the list settings.
    void SetupQueue(int orb1, int orb2, int orb3, int orb4, int orb5, int orb6, int orb7, int junkSquare, int junkTriangle, int powerupEraser, int powerupPaint)
    {
        for (int i = 0; i < orb1; i++){
            gameQueue.Add(referenceItems[0]);
        }
        for (int i = 0; i < orb2; i++)
        {
            gameQueue.Add(referenceItems[1]);
        }
        for (int i = 0; i < orb3; i++)
        {
            gameQueue.Add(referenceItems[2]);
        }
        for (int i = 0; i < orb4; i++)
        {
            gameQueue.Add(referenceItems[3]);
        }
        for (int i = 0; i < orb5; i++)
        {
            gameQueue.Add(referenceItems[4]);
        }
        for (int i = 0; i < orb6; i++)
        {
            gameQueue.Add(referenceItems[5]);
        }
        for (int i = 0; i < orb7; i++)
        {
            gameQueue.Add(referenceItems[6]);
        }
        for (int i = 0; i < junkSquare; i++)
        {
            gameQueue.Add(referenceItems[7]);
        }
        for (int i = 0; i < junkTriangle; i++)
        {
            gameQueue.Add(referenceItems[8]);
        }
        for (int i = 0; i < powerupEraser; i++)
        {
            gameQueue.Add(referenceItems[9]);
        }
        for (int i = 0; i < powerupPaint; i++)
        {
            gameQueue.Add(referenceItems[10]);
        }

        //Randomize the list order
        ShuffleList(gameQueue);
    }

    //List Shuffle Function
    public List<GameObject> ShuffleList(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        return list;
    }

}
