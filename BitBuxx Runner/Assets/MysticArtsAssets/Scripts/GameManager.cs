using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Audio")] //Adding music in the game made everything so much better :). 
    [SerializeField] AudioSource titleMusic;
    [SerializeField] AudioSource mainMusic; 

    public Text deathscoreText, deathcoinText; 
    private const int COIN_SCORE_AMOUNT = 5;
    private int lastScore;
    private int totalCoinAmount; 
    public static GameManager Instance { set; get; }
    
    //use this to debug the game by reloading the scene
    public Scene currentScene;

    public Button settingsButton; 

    public bool IsDead { set; get; }
    private bool isGameStarted = false;
    private PlayerMotor motor;
    private const float MAX_DISTANCE = 10000000;

    // UI and UI Fields 
    public Animator gameCanvas, menuAnim, moneyAnim;
    public static float scoreToTeleport; 
    [SerializeField] Text scoreText, coinText, modiferText, highScoreText, coinScoreText;
    [SerializeField] float score, coinScore, modifierScore, modifierScoreCap, totalCoinScore;

    //Death Menu 
    public Animator deathMenuAnim;

    private void Start()
    {
        gameCanvas.SetTrigger("Hide");
        mainMusic.Stop(); 
        titleMusic.Play();
        isGameStarted = false;
}

    private void Awake()
    {
        highScoreText.text = PlayerPrefs.GetInt("Highscore").ToString();
        coinScoreText.text = PlayerPrefs.GetInt("CoinAmount", 0).ToString();
        totalCoinScore = PlayerPrefs.GetInt("CoinAmount"); 

        //current scene is for debugging, might not be used in the final product. -Mike
        currentScene = SceneManager.GetActiveScene();
        Debug.Log("Current scene is " + currentScene.name);

        Instance = this;
        modifierScore = 1.0f;
        modifierScoreCap = 5.0f; 
        modiferText.text = "x" + modifierScore.ToString("0.0");
        scoreText.text = scoreText.text = score.ToString("0");
        coinText.text = coinScore.ToString("0"); 
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>(); 
    }

    private void Update()
    {
        //Reset mechanic no longer needed so I deleted it. 

        //Debug.Log(modifierScore); 

        if(MobileInputs.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            titleMusic.Stop();
            mainMusic.Play(); 
            motor.StartGame();
            FindObjectOfType<CameraMotor>().IsMoving = true;
            gameCanvas.SetTrigger("Show");
            menuAnim.SetTrigger("Hide"); 
        }

        if (isGameStarted && !IsDead && !PlayerMotor.IsTeleporting)
        {
            //Increase Score 
            score += (Time.deltaTime * modifierScore);
            scoreToTeleport += (Time.deltaTime); 
            if(lastScore != (int)score)
            {
                lastScore = (int)score; 
                scoreText.text = score.ToString("0");
            }
        }
    }

    private void FixedUpdate()
    {
        //this is to fix the possible issue of floating points breaking after a long time
        if (motor.gameObject.transform.position.z >= MAX_DISTANCE)
        {
            //Debug.Log(motor.gameObject.transform.position.z);
            MovePlayerAndObjectsToOrigin();
        }
    }
    public void GetCoin()
    {
        moneyAnim.SetTrigger("Collect");
        coinScore ++;
        totalCoinScore ++; 
        coinText.text = coinScore.ToString("0");
        score += COIN_SCORE_AMOUNT; 
        scoreText.text = scoreText.text = score.ToString("0"); 
    }

    public void Settings()
    {
        Debug.Log("Button Pressed!"); 
    }

    public void UpdateModifier(float modifierAmount)
    {
        if(modifierScore < modifierScoreCap)
        {
            modifierScore = 1.0f + modifierAmount;
            modiferText.text = "x" + modifierScore.ToString("0.0");
        }
    }

    //Restarts Game. 
    public void OnPlayButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene"); 
    }

    //Death menu pops up. 
    public void OnDeath()
    {
        IsDead = true; 
        deathscoreText.text = score.ToString("0");
        deathcoinText.text = coinScore.ToString("0"); 
        deathMenuAnim.SetTrigger("Dead");
        gameCanvas.SetTrigger("Hide");
        PlayerPrefs.SetInt("CoinAmount", (int)totalCoinScore); 

        //Check for highscore 
        if(score > PlayerPrefs.GetInt("Highscore"))
        {
            float s = score;
            if (s % 1 == 0)
                s += 1; 
            PlayerPrefs.SetInt("Highscore", (int)s); 
        }
    }

    //gets the segments in the level manager, the player, and the camera
    //and moves them back max distance units
    private void MovePlayerAndObjectsToOrigin()
    {
        try
        {
            //find the level manager (MAKE SURE THE MANAGER IS TAGGED!!!)
            GameObject levelManager = GameObject.FindGameObjectWithTag("LevelManager");
            Debug.Log("Found a LevelManager :D");

            //move the child objects back a MAX_DISTANCE of units
            Segment[] segments = levelManager.GetComponentsInChildren<Segment>();
            foreach(Segment segment in segments)
            {
                MoveObjectBack(segment.gameObject.transform);
            }

            //do the same thing to the player
            MoveObjectBack(motor.gameObject.transform);
            //and do the same for the camera
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");    //we don't need to do the try/catch since the cam is tagged by default
            MoveObjectBack(cam.transform);
            LevelManager lm = levelManager.GetComponent<LevelManager>();
            lm.currentSpawnZ -= Mathf.FloorToInt(MAX_DISTANCE);
        }
        catch
        {
            //this should only happen if some forgets to tag the manager
            Debug.Log("No object with the tag \"LevelManager\" found!");
            return;
        }
    }

    //moves the object 'obj' back by the max distance of units
    private void MoveObjectBack(Transform obj)
    {
        Vector3 newPos = new Vector3(obj.position.x, obj.position.y, obj.position.z - MAX_DISTANCE);
        obj.position = newPos;
    }

}
