using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class MainMenu : MonoBehaviour
{
    public AudioSource buttonclicksound;

    public GameObject RulesetFirstSelect, NormalFirstSelect, CloseFirstSelect;
    public void StartGame()
    {
        if (!buttonclicksound.isPlaying)
            buttonclicksound.Play();
        SceneManager.LoadSceneAsync("Production");
    }

    public void RulesetOpen(){
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(RulesetFirstSelect);
    }
    public void RulesetClose(){
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(CloseFirstSelect);
    }
    public void QuitGame()
    {
        if (!buttonclicksound.isPlaying)
            buttonclicksound.Play();
        Application.Quit();
    }
}
