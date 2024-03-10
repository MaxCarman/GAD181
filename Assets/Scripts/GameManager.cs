using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Search;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEditor.Animations;
using System.Security.Cryptography;

public class GameManager : MonoBehaviour
{
    //Declare variables
    //Game settings
    public int gameState;          //The current game state. 0 = Pre-Game, 1 = In-Game, 2 = Post-Game
    public int gamePlayersAlive;   //The current amount of players alive in the game.
    public string gameWinnerName;  //The playerName of the winner.
    public Color gameWinnerColor;  //The color of the winner.
    public int settingPlayerCount; //The amount of players playing the game.
    public int settingMap;         //The selected map.

    //Queue related vars
    public List<GameObject> gameQueue; //The list of gameobjects currently in the queue.
    [SerializeField] private List<GameObject> referenceItems;          //A list of all the items that can appear in the queue. Goes [Orb1 -> Orb7, JunkSquare, JunkTriangle, PowerupEraser, PowerupPaint]
    [SerializeField] private List<UnityEngine.UI.Image> referenceQueueImages;         //A reference to the 7 queue image slots.
    [SerializeField] private List<TextMeshProUGUI> referenceQueueText; //A reference to the 7 queue text slots.

    //Pregame/Postgame UI related vars
    [SerializeField] private Canvas referenceCanvasPregame;            //A reference to the pregame canvas.
    [SerializeField] private TextMeshProUGUI referencePlayerCountText; //A reference to the pregame player count text.
    [SerializeField] private TextMeshProUGUI referenceMapText;         //A reference to the pregame map text.

    [SerializeField] private GameObject referenceTutorialPopup; //A reference to the tutorial popup.
    [SerializeField] private GameObject referenceControlsPopup; //A reference to the controls popup.

    [SerializeField] private Canvas referenceCanvasPostgame; //A reference to the postgame canvas.
    [SerializeField] private TextMeshProUGUI referenceWinnerText; //A reference to the postgame winner text.

    //Map related vars
    [SerializeField] private List<GameObject> referenceMapObjects; //A list of parent objects for each map.
    [SerializeField] private List<string> referenceMapNames; //A list of map names.

    //Player related vars
    [SerializeField] private List<string> playerNames; //A list of the player names.
    [SerializeField] private List<Color> playerColors; //A list of the player colours.

    [SerializeField] private List<KeyCode> playerControlsLeft;  //A list of the player left buttons
    [SerializeField] private List<KeyCode> playerControlsRight; //A list of the player right buttons.
    [SerializeField] private List<KeyCode> playerControlsDrop;  //A list of the player drop buttons.

    [SerializeField] private GameObject referenceMarker; //A reference to the player marker gameobject.
    private List<GameObject> referencePlayers; //A list of all the current player control scripts. Used to determine game end conditions and winners.

    //Misc vars
    int launchCooldown = 0; //Used for title screen launching.

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Only update if game in pre-game.
        if(gameState == 0)
        {
            //Show specific canvases
            referenceCanvasPregame.enabled = true;
            referenceCanvasPostgame.enabled = false;

            //Update player count text
            referencePlayerCountText.text = "Player count: " + settingPlayerCount;

            //Update map text
            referenceMapText.text = "Map Type: " + referenceMapNames[settingMap];

            //Launch orbs around!
            launchCooldown += 1;
            if(launchCooldown >= 500)
            {
                launchCooldown = 0;
                foreach (GameObject i in GameObject.FindGameObjectsWithTag("Orb"))
                {
                    //If it has low motion, launch it.
                    //i.GetComponent<Rigidbody2D>().mass = 3;
                    if(i.GetComponent<Rigidbody2D>().velocity.x < 1 && i.GetComponent<Rigidbody2D>().velocity.y < 1)
                    {
                        i.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(500, -501) * i.GetComponent<Rigidbody2D>().mass, Random.Range(500, -501) * i.GetComponent<Rigidbody2D>().mass));
                    }
                }
            }
        }

        //Only update if the game is in progress.
        if(gameState == 1)
        {
            //Show specific canvases
            referenceCanvasPregame.enabled = false;
            referenceCanvasPostgame.enabled = false;

            //If there are only 8 values in the list left, add another bag to the end.
            if (gameQueue.Count <= 8)
            {
                SetupQueue(20, 10, 4, 1, 0, 0, 0, 1, 1, 1, 1);
            }

            //Update visuals for the shown queue.
            for (int i = 0; i < 7; i++)
            {
                //Set the queue image.
                referenceQueueImages[i].sprite = gameQueue[i].GetComponent<SpriteRenderer>().sprite;
                referenceQueueImages[i].transform.localScale = gameQueue[i].transform.localScale;

                //If the queue item is a orb, then update the text to show its orbValue, otherwise hide the text.
                if (gameQueue[i].gameObject.tag == "Orb")
                {
                    referenceQueueText[i].enabled = true;
                    ItemController referenceItemController = gameQueue[i].GetComponent<ItemController>();
                    referenceQueueText[i].text = referenceItemController.orbValue.ToString();
                }
                else
                {
                    referenceQueueText[i].enabled = false;
                }
            }

            //Loop through each script reference to see if they are still alive, and set the gamePlayersAlive value accordingly.
            gamePlayersAlive = 0;
            for (int i = 0; i < referencePlayers.Count; i++)
            {
                if(referencePlayers[i].GetComponent<PlayerController>().playerIsAlive == true)
                {
                    gamePlayersAlive += 1;
                }
            }

            //If there are 1 or no players left, end the game.
            if (gamePlayersAlive <= 1)
            {
                List<int> playerScores = new List<int>();
                //Find the player with the highest score by creating a list of scores, sorting them and taking the first index. Then compare this index back to the scripts, stopping when the first index matchs the script's score value.
                for (int i = 0; i < referencePlayers.Count; i++)
                {
                    playerScores.Add(referencePlayers[i].GetComponent<PlayerController>().playerScore);
                }
                playerScores.Sort();
                for (int i = 0; i < referencePlayers.Count; i++)
                {
                    if (referencePlayers[i].GetComponent<PlayerController>().playerScore == playerScores[referencePlayers.Count-1])
                    {
                        gameWinnerName = referencePlayers[i].GetComponent<PlayerController>().playerName;
                        gameWinnerColor = referencePlayers[i].GetComponent<PlayerController>().playerColor;
                        break;
                    }
                }

                //End the game and set winner text.
                gameState = 2;
                referenceWinnerText.text = gameWinnerName + " WON!";
                referenceWinnerText.color = gameWinnerColor;

                //Disable player markers and held items, but not player score texts.
                for (int i = 0; i < referencePlayers.Count; i++)
                {
                    referencePlayers[i].GetComponent<PlayerController>().referenceMarker.GetComponent<SpriteRenderer>().enabled = false;
                    referencePlayers[i].GetComponent<PlayerController>().referenceCooldownBar.GetComponent<SpriteRenderer>().enabled = false;
                    referencePlayers[i].GetComponent<PlayerController>().referenceDropLine.GetComponent<SpriteRenderer>().enabled = false;
                    referencePlayers[i].GetComponent<PlayerController>().referenceOutOfGameEffect.GetComponent<SpriteRenderer>().enabled = false;
                    referencePlayers[i].GetComponent<PlayerController>().referenceHeldItem.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }

        //Only update if the game is post-game.
        if(gameState == 2)
        {
            //Show specific canvases
            referenceCanvasPregame.enabled = false;
            referenceCanvasPostgame.enabled = true;
        }

    }

    //Function called when the game starts.
    public void SetupGame()
    {
        //Clear the game board/queue.
        ClearGame();
        referenceTutorialPopup.SetActive(false);
        referenceTutorialPopup.SetActive(false);

        //Create a new queue.
        SetupQueue(20, 10, 4, 1, 0, 0, 0, 1, 1, 1, 1);
        referencePlayers = new List<GameObject>();

        gamePlayersAlive = settingPlayerCount;

        //Calculate passing/spawn positions so players are split evenly.
        float spawnPos = 0;
        float divide = 14 / (float)settingPlayerCount;
        float padding = 14 / ((float)settingPlayerCount * 2);
        spawnPos += padding;

        //Setup players from the list using the selected player count.
        for (int i = 0; i < settingPlayerCount; i++)
        {
            //Create player marker
            GameObject referencePlayer = Instantiate(referenceMarker, new Vector3(spawnPos - 7, 3.5f, 0), Quaternion.identity) as GameObject;
            //Debug.Log("Divide: " + divide + ", Padding: " + padding + ", SpawnPos: " + spawnPos);
            spawnPos += divide;
            //Old Position Formula: Mathf.Clamp((float)i / settingPlayerCount * 14 - 7, -6, 6)
            //Get the new player's script and set relevent variables.
            PlayerController referencePlayerControl = referencePlayer.GetComponent<PlayerController>();
            referencePlayerControl.playerID = i + 1;
            referencePlayerControl.playerColor = playerColors[i];
            referencePlayerControl.playerControlLeft = playerControlsLeft[i];
            referencePlayerControl.playerControlRight = playerControlsRight[i];
            referencePlayerControl.playerControlDrop = playerControlsDrop[i];
            referencePlayerControl.referenceGameManager = this.gameObject;
            referencePlayerControl.playerName = playerNames[i];

            //Add the player control script to the list.
            referencePlayers.Add(referencePlayer);
        }

        //Declare that the game has started.
        gameState = 1;
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

    //Clears the game board/players/queue.
    public void ClearGame()
    {
        //Delete all currently existing player markers and items.
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Player"))
        {
            Destroy(i);
        }
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Orb"))
        {
            Destroy(i);
        }
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Junk"))
        {
            Destroy(i);
        }
        foreach (GameObject i in GameObject.FindGameObjectsWithTag("Powerup"))
        {
            Destroy(i);
        }

        //Clear the previous queue.
        gameQueue.Clear();
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

    //OnClick event for + playercount
    public void ClickPlayerCountPlus()
    {
        settingPlayerCount += 1;
        settingPlayerCount = Mathf.Clamp(settingPlayerCount,2,4);
    }

    //OnClick event for - playercount
    public void ClickPlayerCountMinus()
    {
        settingPlayerCount -= 1;
        settingPlayerCount = Mathf.Clamp(settingPlayerCount, 2, 4);
    }

    //OnClick event for + map
    public void ClickPlayerMapPlus()
    {
        settingMap += 1;
        settingMap = Mathf.Clamp(settingMap, 0, 4);
        UpdateMap();
    }

    //OnClick event for - map
    public void ClickPlayerMapMinus()
    {
        settingMap -= 1;
        settingMap = Mathf.Clamp(settingMap, 0, 4);
        UpdateMap();
    }

    //Update map based on selected map.
    public void UpdateMap()
    {
        //Hide all maps
        for (int i = 0; i < referenceMapObjects.Count; i++)
        {
            referenceMapObjects[i].SetActive(false);
        }

        //Show specific map
        referenceMapObjects[settingMap].SetActive(true);
    }

    //OnClick event for the exit button. If in-game, go back to the title screen, if in the pre-game, then send them back to the gems select screen.
    public void ClickExit()
    {
        if(gameState == 1 || gameState == 2)
        {
            gameState = 0;

            //Destroy player markers
            foreach (GameObject i in GameObject.FindGameObjectsWithTag("Player"))
            {
                Destroy(i);
            }
        }
        if (gameState == 0)
        {
            //SEND TO GAME SELECT
        }
    }

    //OnClick event for the tutorial button. Toggles based on visibility.
    public void ClickTutorial()
    {
        referenceControlsPopup.SetActive(false);
        if(referenceTutorialPopup.activeSelf == true)
        {
            referenceTutorialPopup.SetActive(false);
        } else
        {
            referenceTutorialPopup.SetActive(true);
        }
    }

    //OnClick event for the tutorial button. Toggles based on visibility.
    public void ClickControls()
    {
        referenceTutorialPopup.SetActive(false);
        if (referenceControlsPopup.activeSelf == true)
        {
            referenceControlsPopup.SetActive(false);
        }
        else
        {
            referenceControlsPopup.SetActive(true);
        }
    }
}
