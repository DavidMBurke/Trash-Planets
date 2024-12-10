using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject GameOverScreen;

    public GameObject PauseScreen;

    public GameObject RuleSet;
    public ScorebarLogic scorebar;

    public GameObject GameOverSelect, PauseSelect, RulesetFirstSelect,RuleSetCloseSelect;

    public bool isPaused = false;

    public bool gameend = false;
    [SerializeField] TextMeshProUGUI winnerText;
    void Start()
    {
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !gameend && !RuleSet.activeSelf){
            Debug.Log("Escape");
            if(isPaused)
                resume();
            else{
                pause();
            }

        }
    }

    public void gameOver(){
        Cursor.visible = true;
        GameOverScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameOverSelect);
        GetWinner();
    }

    public void restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void menu(){
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void pause(){
        Time.timeScale =  0f;
        PauseScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(PauseSelect);
        isPaused = true;
    }

    public void resume(){
        PauseScreen.SetActive(false);
        Time.timeScale =  1f;
        EventSystem.current.SetSelectedGameObject(null);
        isPaused = false;
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
public void RulesetOpen(){
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(RulesetFirstSelect);
    }
public void RulesetClose(){
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(RuleSetCloseSelect);
    }
}
