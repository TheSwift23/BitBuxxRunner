using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private const int COIN_SCORE_AMOUNT = 5;
    private int lastScore; 
    public static GameManager Instance { set; get; }
    
    //use this to debug the game by reloading the scene
    public Scene currentScene;

    public bool IsDead { set; get; }
    private bool isGameStarted = false;
    private PlayerMotor motor;
    private const float MAX_DISTANCE = 10;

    // UI and UI Fields 
    [SerializeField] Text scoreText, coinText, modiferText;
    private float score, coinScore, modifierScore; 

    private void Awake()
    {
        //current scene is for debugging, might not be used in the final product. -Mike
        currentScene = SceneManager.GetActiveScene();
        Debug.Log("Current scene is " + currentScene.name);

        Instance = this;
        modifierScore = 1.0f;
        modiferText.text = "x" + modifierScore.ToString("0.0");
        scoreText.text = scoreText.text = score.ToString("0");
        coinText.text = coinScore.ToString("0"); 
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>(); 
    }

    private void Update()
    {
        //Debug: reset the game scene when we press a button
        //breaks execution once we do.
        //will need to be removed once we have a better reload mechanic set up -Mike
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(currentScene.buildIndex);
            return;
        }

        if(MobileInputs.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            motor.StartGame(); 
        }

        if (isGameStarted && !IsDead)
        {
            //Increase Score 
            score += (Time.deltaTime * modifierScore);
            if(lastScore != (int)score)
            {
                lastScore = (int)score; 
                scoreText.text = score.ToString("0");
                //Debug.Log(lastScore);     //is it really necessary to log EVERY TIME the score increases? -Mike
            }
        }
    }

    private void FixedUpdate()
    {
        //this is to fix the possible issue of floating points breaking after a long time
        if (motor.gameObject.transform.position.z >= MAX_DISTANCE)
        {
            Debug.Log(motor.gameObject.transform.position.z);
            MovePlayerAndObjectsToOrigin();
        }
    }
    public void GetCoin()
    {
        coinScore ++;
        coinText.text = coinScore.ToString("0");
        score += COIN_SCORE_AMOUNT; 
        scoreText.text = scoreText.text = score.ToString("0"); 
    }

    public void UpdateModifier(float modifierAmount)
    {
        modifierScore = 1.0f + modifierAmount;
        modiferText.text = "x" + modifierScore.ToString("0.0");
    }

    private void MovePlayerAndObjectsToOrigin()
    {
        try
        {
            GameObject levelManager = GameObject.FindGameObjectWithTag("LevelManager");
            Debug.Log("Found a LevelManager :D");

            //move the child objects back a MAX_DISTANCE of units
            Segment[] segments = levelManager.GetComponentsInChildren<Segment>();
            foreach(Segment segment in segments)
            {
                MoveObjectBack(segment.gameObject.transform);
            }

            MoveObjectBack(motor.gameObject.transform);
            GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
            MoveObjectBack(cam.transform);
        }
        catch
        {
            Debug.Log("No object with the tag \"LevelManager\" found!");
            return;
        }
    }

    private void MoveObjectBack(Transform obj)
    {
        Vector3 newPos = new Vector3(obj.position.x, obj.position.y, obj.position.z - MAX_DISTANCE);
        obj.position = newPos;
    }

}
