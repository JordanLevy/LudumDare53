using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    public static int scoreValue = 0;
    public TextMeshProUGUI score;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        score.text = "Score: " + scoreValue;
    }
}
