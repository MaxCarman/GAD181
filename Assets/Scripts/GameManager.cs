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
    public List<GameObject> gameQueue;
    [SerializeField] private GameObject referenceLv1Orb;
    [SerializeField] private GameObject referenceLv2Orb;
    [SerializeField] private GameObject referenceLv3Orb;
    [SerializeField] private GameObject referenceLv4Orb;
    [SerializeField] private GameObject referenceLv5Orb;
    [SerializeField] private GameObject referenceLv6Orb;
    [SerializeField] private GameObject referenceLv7Orb;
    [SerializeField] private GameObject referenceJunkSquare;
    [SerializeField] private GameObject referenceJunkTriangle;
    [SerializeField] private GameObject referencePowerupEraser;
    [SerializeField] private GameObject referencePowerupPaintBucket;

    [SerializeField] private Image referenceQueue1;
    [SerializeField] private Image referenceQueue2;
    [SerializeField] private Image referenceQueue3;
    [SerializeField] private Image referenceQueue4;
    [SerializeField] private Image referenceQueue5;
    [SerializeField] private Image referenceQueue6;
    [SerializeField] private Image referenceQueue7;

    [SerializeField] private TextMeshProUGUI referenceText1;
    [SerializeField] private TextMeshProUGUI referenceText2;
    [SerializeField] private TextMeshProUGUI referenceText3;
    [SerializeField] private TextMeshProUGUI referenceText4;
    [SerializeField] private TextMeshProUGUI referenceText5;
    [SerializeField] private TextMeshProUGUI referenceText6;
    [SerializeField] private TextMeshProUGUI referenceText7;

    //Player related vars
    [SerializeField] private int playerCount;
    [SerializeField] private GameObject referenceMarker;
    [SerializeField] private List<Color> playerColors;
    [SerializeField] private List<string> playerNames;
    [SerializeField] private List<KeyCode> playerControlsLeft;
    [SerializeField] private List<KeyCode> playerControlsRight;
    [SerializeField] private List<KeyCode> playerControlsDrop;

    // Start is called before the first frame update
    void Start()
    {
        SetupQueue(20, 10, 4, 1, 0, 0, 0, 1, 1, 1, 1);
        //Setup players from the list
        for(int i = 0; i < playerCount; i++)
        {
            //Create player marker
            GameObject referencePlayerMarker = Instantiate(referenceMarker, new Vector3(Mathf.Clamp((float)i / playerCount * 14 - 7,-6,6), 3.5f,0), Quaternion.identity) as GameObject;
            //Debug.Log("i = " + i + ", playerCount = " + playerCount);
            //Debug.Log(Mathf.Clamp((float)i/playerCount*14-7,-6,6));
            playerControl referencePlayerControl = referencePlayerMarker.GetComponent<playerControl>();
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
        //If there are only 8 values in the list left, add another bag.
        if (gameQueue.Count <= 8)
        {
            SetupQueue(20, 10, 4, 1, 0, 0, 0, 1, 1, 1, 1);
        }

        //Update visuals for the shown queue.
        referenceQueue1.sprite = gameQueue[0].GetComponent<SpriteRenderer>().sprite;
        referenceQueue2.sprite = gameQueue[1].GetComponent<SpriteRenderer>().sprite;
        referenceQueue3.sprite = gameQueue[2].GetComponent<SpriteRenderer>().sprite;
        referenceQueue4.sprite = gameQueue[3].GetComponent<SpriteRenderer>().sprite;
        referenceQueue5.sprite = gameQueue[4].GetComponent<SpriteRenderer>().sprite;
        referenceQueue6.sprite = gameQueue[5].GetComponent<SpriteRenderer>().sprite;
        referenceQueue7.sprite = gameQueue[6].GetComponent<SpriteRenderer>().sprite;

        referenceQueue1.transform.localScale = gameQueue[0].transform.localScale;
        referenceQueue2.transform.localScale = gameQueue[1].transform.localScale;
        referenceQueue3.transform.localScale = gameQueue[2].transform.localScale;
        referenceQueue4.transform.localScale = gameQueue[3].transform.localScale;
        referenceQueue5.transform.localScale = gameQueue[4].transform.localScale;
        referenceQueue6.transform.localScale = gameQueue[5].transform.localScale;
        referenceQueue7.transform.localScale = gameQueue[6].transform.localScale;

        //If it is a orb, also show the index's orb size, otherwise hide the text component.
        if (gameQueue[0].gameObject.tag == "Orb")
        {
            referenceText1.enabled = true;
            itemControl referenceHeldItemScript = gameQueue[0].GetComponent<itemControl>();
            referenceText1.text = referenceHeldItemScript.mergeValue.ToString();
        } else
        {
            referenceText1.enabled = false;
        }
        if (gameQueue[1].gameObject.tag == "Orb")
        {
            referenceText2.enabled = true;
            itemControl referenceHeldItemScript = gameQueue[1].GetComponent<itemControl>();
            referenceText2.text = referenceHeldItemScript.mergeValue.ToString();
        }
        else
        {
            referenceText2.enabled = false;
        }
        if (gameQueue[2].gameObject.tag == "Orb")
        {
            referenceText3.enabled = true;
            itemControl referenceHeldItemScript = gameQueue[2].GetComponent<itemControl>();
            referenceText3.text = referenceHeldItemScript.mergeValue.ToString();
        }
        else
        {
            referenceText3.enabled = false;
        }
        if (gameQueue[3].gameObject.tag == "Orb")
        {
            referenceText4.enabled = true;
            itemControl referenceHeldItemScript = gameQueue[3].GetComponent<itemControl>();
            referenceText4.text = referenceHeldItemScript.mergeValue.ToString();
        }
        else
        {
            referenceText4.enabled = false;
        }
        if (gameQueue[4].gameObject.tag == "Orb")
        {
            referenceText5.enabled = true;
            itemControl referenceHeldItemScript = gameQueue[4].GetComponent<itemControl>();
            referenceText5.text = referenceHeldItemScript.mergeValue.ToString();
        }
        else
        {
            referenceText5.enabled = false;
        }
        if (gameQueue[5].gameObject.tag == "Orb")
        {
            referenceText6.enabled = true;
            itemControl referenceHeldItemScript = gameQueue[5].GetComponent<itemControl>();
            referenceText6.text = referenceHeldItemScript.mergeValue.ToString();
        }
        else
        {
            referenceText6.enabled = false;
        }
        if (gameQueue[6].gameObject.tag == "Orb")
        {
            referenceText7.enabled = true;
            itemControl referenceHeldItemScript = gameQueue[6].GetComponent<itemControl>();
            referenceText7.text = referenceHeldItemScript.mergeValue.ToString();
        }
        else
        {
            referenceText7.enabled = false;
        }
    }

    //Creates a new, full queue of prefabs based on the list settings.
    void SetupQueue(int Lv1Orbs, int Lv2Orbs, int Lv3Orbs, int Lv4Orbs, int Lv5Orbs, int Lv6Orbs, int Lv7Orbs, int JunkSquares, int JunkTriangles, int PowerupErasers, int PowerupPaintBuckets)
    {
        for (int i = 0; i < Lv1Orbs; i++){
            gameQueue.Add(referenceLv1Orb);
        }
        for (int i = 0; i < Lv2Orbs; i++)
        {
            gameQueue.Add(referenceLv2Orb);
        }
        for (int i = 0; i < Lv3Orbs; i++)
        {
            gameQueue.Add(referenceLv3Orb);
        }
        for (int i = 0; i < Lv4Orbs; i++)
        {
            gameQueue.Add(referenceLv4Orb);
        }
        for (int i = 0; i < Lv5Orbs; i++)
        {
            gameQueue.Add(referenceLv5Orb);
        }
        for (int i = 0; i < Lv6Orbs; i++)
        {
            gameQueue.Add(referenceLv6Orb);
        }
        for (int i = 0; i < Lv7Orbs; i++)
        {
            gameQueue.Add(referenceLv7Orb);
        }
        for (int i = 0; i < JunkSquares; i++)
        {
            gameQueue.Add(referenceJunkSquare);
        }
        for (int i = 0; i < JunkTriangles; i++)
        {
            gameQueue.Add(referenceJunkTriangle);
        }
        for (int i = 0; i < PowerupErasers; i++)
        {
            gameQueue.Add(referencePowerupEraser);
        }
        for (int i = 0; i < PowerupPaintBuckets; i++)
        {
            gameQueue.Add(referencePowerupPaintBucket);
        }

        //Randomize the list order
        ShuffleList(gameQueue);

        //Debug out the new list order
        for (int i = 0; i < gameQueue.Count; i++)
        {
            //Debug.Log(gameQueue[i]);
        }
    }

    //List Shuffle Function
    public List<GameObject> ShuffleList(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            GameObject temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        return list;
    }

}
