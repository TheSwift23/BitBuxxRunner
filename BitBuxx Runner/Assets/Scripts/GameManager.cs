using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    private bool isGameStarted = false;
    private PlayerMotor motor; 

    // UI and UI Fields 
    [SerializeField] Text scoreText, coinText, modiferText;
    private float score, coinScore, modifierScore; 

    private void Awake()
    {
        Instance = this;
        UpdateScores();
        motor = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMotor>(); 
    }

    public void UpdateScores()
    {
        scoreText.text = score.ToString();
        coinText.text = coinScore.ToString();
        modiferText.text = modifierScore.ToString(); 
    }

    private void Update()
    {
        if(MobileInputs.Instance.Tap && !isGameStarted)
        {
            isGameStarted = true;
            motor.StartGame(); 
        }
    }
}
