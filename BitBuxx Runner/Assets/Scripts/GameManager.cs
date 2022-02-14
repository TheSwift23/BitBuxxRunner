using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    private const int COIN_SCORE_AMOUNT = 5;
    private int lastScore; 
    public static GameManager Instance { set; get; }

    public bool IsDead { set; get; }
    private bool isGameStarted = false;
    private PlayerMotor motor; 

    // UI and UI Fields 
    [SerializeField] Text scoreText, coinText, modiferText;
    private float score, coinScore, modifierScore; 

    private void Awake()
    {
        Instance = this;
        modifierScore = 1.0f;
        modiferText.text = "x" + modifierScore.ToString("0.0");
        scoreText.text = scoreText.text = score.ToString("0");
        coinText.text = coinScore.ToString("0"); 
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>(); 
    }

    private void Update()
    {
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
                Debug.Log(lastScore); 
            }
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


}
