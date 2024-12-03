using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject GameOverScreen;
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
        Debug.Log("called");
    }

    public void restart(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void menu(){
        SceneManager.LoadSceneAsync("Production");
    }

    public void view(){
        Debug.Log("TBI");
    }
}
