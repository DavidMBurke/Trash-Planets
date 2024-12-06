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
    public Color baseColor;
    public Color highlightColor = Color.yellow;
    public new Renderer renderer;
    void Start()
    {
        button = gameObject;
        renderer = GetComponent<Renderer>();
        baseColor = renderer.material.color;
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

    public void Highlight()
    {
        if (renderer != null)
        {
            renderer.material.color = highlightColor;
        }
    }

    public void RemoveHighlight()
    {
        if (renderer != null)
        {
            renderer.material.color = baseColor;
        }
    }
}
