using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    public static int scoreValue = 0;
    public static int strikeValue = 0;
    private static TextMeshProUGUI score;
    private static float gameOverTime;
    private ScoreController instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        score = gameObject.GetComponent<TextMeshProUGUI>();
        scoreValue = 0;
        strikeValue = 0;
        RefreshScore();
    }

    void Update(){
        if(strikeValue >= 3 && Time.time - gameOverTime > 1f){ // game over
            PauseManager.PauseGame(5);
        }
    }

    // Update is called once per frame
    public static void RefreshScore()
    {
        score.text = "Score: " + scoreValue + "\nStrikes: " + strikeValue;
    }

    public static void IncrementScore(){
        scoreValue++;
        RefreshScore();
    }

    public static void IncrementStrikes(){
        strikeValue++;
        RefreshScore();
        if(strikeValue >= 3)
        {
            gameOverTime = Time.time;
        }
    }
}
