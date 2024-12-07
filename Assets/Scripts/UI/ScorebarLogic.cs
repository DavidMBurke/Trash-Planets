using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorebarLogic : MonoBehaviour
{
    private int P1Trash;
    private int P2Trash;
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
        P1Trash = GameObject.Find("Planet1").GetComponent<Planet>().trashOnPlanet;
        P2Trash = GameObject.Find("Planet2").GetComponent<Planet>().trashOnPlanet;
        int smallerCount = Mathf.Min(P1Trash, P2Trash);
        if (smallerCount == P1Trash)
        {
            P1Score = 500;
            P2Score = 500 + P2Trash - P1Trash;
        } else
        {
            P2Score = 500;
            P1Score = 500 + P1Trash - P2Trash;
        }
        P1ratio = (float) P1Score/(P1Score + P2Score);
        P2ratio = (float) P2Score/(P1Score + P2Score);

        GameObject.Find("P2 Score").GetComponent<RectTransform>().localScale = new Vector3(P2ratio, 1, 1);
    }

    public (int,int) getScores(){
        return (P1Score , P2Score);
    }
}
