using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject GameOverScreen;

    public ScorebarLogic scorebar;

    [SerializeField] TextMeshProUGUI winnerText;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void gameOver(){
        Cursor.visible = true;
        GameOverScreen.SetActive(true);
        GetWinner();
    }

    public void restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void menu(){
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void view(){
        Debug.Log("TBI");
    }


public void GetWinner()
{
    var scores = scorebar.getScores();
    if (scores.Item1 < scores.Item2)
    {
        winnerText.text = "Player 1 Wins!";
    }
    else if (scores.Item1 > scores.Item2)
    {
        winnerText.text = "Player 2 Wins!";
    }
    else
    {
        winnerText.text = "Tied!";
    }
}
}
