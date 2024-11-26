using UnityEngine;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

public class BuildingButton : MonoBehaviour
{
    public GameObject button;
    public float startingZ = .15f;
    public float depressedZ = .05f;
    public bool isPressed = false;
    public Action function;
    void Start()
    {
        button = gameObject;
    }

    public void Press()
    {
        isPressed = true;
        PositionButton();
    }

    public void Depress()
    {
        isPressed = false;
        PositionButton();
        function?.Invoke();
    }

    private void Update()
    {
        PositionButton();
    }

    private void PositionButton()
    {
        Vector3 currentScale = button.transform.localScale;
        button.transform.localScale = new Vector3(currentScale.x, currentScale.y, isPressed ? depressedZ : startingZ);
    }
}
