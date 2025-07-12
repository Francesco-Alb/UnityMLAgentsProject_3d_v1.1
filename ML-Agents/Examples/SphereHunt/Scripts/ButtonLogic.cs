using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLogic : MonoBehaviour
{
    [SerializeField] private Material unpressedMaterial; // Material for the button when unpressed (yellow)
    [SerializeField] private Material pressedMaterial;   // Material for the button when pressed (grey)
    [SerializeField] private MeshRenderer buttonMeshRenderer; // Renderer for the button visuals (child object)
    [SerializeField] public Transform buttonBaseTransform;   // The base of the button (parent object)
    [SerializeField] public TargetLogic targetLogic; // Reference to the TargetLogic script

    private bool isPressed = false; // Track the button's state

    // Called when the agent presses the button
    public void PressButton()
    {
        if (!isPressed)
        {
            isPressed = true;
            buttonMeshRenderer.material = pressedMaterial;
            Debug.Log("Button pressed");
            targetLogic.SpawnTarget(); // Spawn a new target
            // targetLogic.ResetTarget();
        }
    }

    public void ResetButton()
    {
        isPressed = false;
        buttonMeshRenderer.material = unpressedMaterial; 

        // Resets the button and base position and state
        float randomX = Random.Range(-4.5f,+4.5f);
        float randomZ = Random.Range(-4f,+4f);
        buttonBaseTransform.localPosition = new Vector3(randomX,0,randomZ); 

        Debug.Log("Button Reset");
    }

    // Check if button is pressed
    public bool IsPressed()
    {
        return isPressed;
    }
}
