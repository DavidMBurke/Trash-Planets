using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorebarLogic : MonoBehaviour
{
    private int P1Score;
    private int P2Score;
    private float P1ratio;
    private float P2ratio;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        P1Score = GameObject.Find("Player").GetComponent<Player>().playerScore + 1;
        P2Score = GameObject.Find("Player2").GetComponent<Player>().playerScore + 1;
        P1ratio = (float) P1Score/(P1Score + P2Score);
        P2ratio = (float) P2Score/(P1Score + P2Score);
        Debug.Log(P1Score);
        Debug.Log(P2Score);
        Debug.Log(P2ratio);
        
        GameObject.Find("P2 Score").GetComponent<RectTransform>().localScale = new Vector3(P2ratio, 1, 1);
    }
}
