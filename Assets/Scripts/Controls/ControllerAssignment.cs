using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem.Controls;
using TMPro;

public class ControllerAssignment : MonoBehaviour
{
    public TextMeshProUGUI connectedCount;

    private System.Collections.Generic.List<InputDevice> usingControllers;
    private bool startAssignment = false;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        StartCoroutine(CheckControllers());
    }

    void FixedUpdate()
    {
        if(startAssignment)
        {
            assignControllers();
        }
    }

    private IEnumerator CheckControllers()
    {
        connectedCount.text = "Connected Controllers: 0";

        while (true)
        {
            // Get controllers (joysticks + gamepads)
            var allDevices = InputSystem.devices;
            var connectedControllers = new System.Collections.Generic.List<InputDevice>();

            foreach (var device in allDevices)
            {
                if (device is Gamepad || device is Joystick)
                {
                    connectedControllers.Add(device);
                }
            }

            connectedCount.text = "Connected Controllers: " + connectedControllers.Count.ToString();

            if (connectedControllers.Count >= 2)
            {
                connectedCount.text = "Controllers connected! Starting...";
                yield return new WaitForSeconds(1f);
                SceneManager.LoadScene("Production");
                usingControllers = connectedControllers;
                startAssignment = true;
                break;
            }
            else
            {
                yield return null;
            }
        }
    }

    private void assignControllers()
    {
        GameObject playerPortal = GameObject.Find("PlayerPortal");

        if (playerPortal == null)
        {
            return;
        }

        setControllerByType(usingControllers[0], playerPortal.GetComponent<PlayerPortal>().player1);
        setControllerByType(usingControllers[1], playerPortal.GetComponent<PlayerPortal>().player2);

        Destroy(playerPortal);
        Destroy(this.gameObject);

    }

    private void setControllerByType(InputDevice device, PlayerInput playerInput)
    {
        if (device is Gamepad)
        {
            playerInput.SwitchCurrentControlScheme("Gamepad", device);
        }

        if (device is Joystick)
        {
            playerInput.SwitchCurrentControlScheme("Joystick", device);
        }
    }

}
