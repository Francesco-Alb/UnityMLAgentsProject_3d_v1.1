using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
// using JetBrains.Annotations;


public class SphereHuntAgent : Agent
{
    [SerializeField] private Transform buttonTransform; // Reference to the button's Transform
    [SerializeField] private ButtonLogic buttonLogic;   // Script handling button behavior
    [SerializeField] private TargetLogic targetLogic;   // Script handling target behavior
    [SerializeField] private Material winMaterial;      // Material for success (target reached)
    [SerializeField] private Material loseMaterial;     // Material for failure (wall collision)
    [SerializeField] private MeshRenderer floorMeshRenderer;    // Renderer for floor feedback
    [SerializeField] public float speed = 2f;          // Movement speed of the agent

    // Called at the start of each episode
    public override void OnEpisodeBegin()
    {
        buttonLogic.ResetButton();  // Resets both the button and its base
        targetLogic.ResetTarget();// Tell TargetLogic to handle target creation/reset

        // Randomize agent's position
        float randomX = Random.Range(-4.5f,+4.5f);
        float randomZ = Random.Range(-4f,+4f);

        // Ensure the agent does not spawn too close to the button
        while (Vector3.Distance(new Vector3(randomX,0,randomZ), buttonLogic.buttonBaseTransform.localPosition) <1f) 
        {
            randomX = Random.Range(-4.5f,+4.5f);
            randomZ = Random.Range(-4f,+4f);   
        }

        // Set the agent's position
        transform.localPosition = new Vector3(randomX,0.5f,randomZ);
    }

    //Collect obs for the agent
    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 agentPosition = transform.localPosition;
        Vector3 buttonPosition = buttonTransform.localPosition;
        bool isButtonPressed = buttonLogic.IsPressed(); // Store the result
        float buttonPressed = isButtonPressed ? 1f : 0f;
        Vector3 targetPosition = isButtonPressed ? targetLogic.GetTargetPosition() : Vector3.zero;

        // Debug.Log($"Agent Position: {agentPosition}");
        // Debug.Log($"Button Position: {buttonPosition}");
        // Debug.Log($"Button Pressed: {buttonPressed}");
        // Debug.Log($"Target Position: {targetPosition}");

        sensor.AddObservation(agentPosition); // Agent's position (3 obs)
        sensor.AddObservation(buttonPosition); // Button's position (3 obs)
        sensor.AddObservation(buttonPressed); // Whether the button has been pressed (1 ob)
        sensor.AddObservation(targetPosition); // Target position or padding (3 obs)
    }

    // Receives actions from the ML algorithm
    public override void OnActionReceived(ActionBuffers actions)
    {
        // Movement: continuous actions
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed;

        //Discrete: button press
        if (actions.DiscreteActions[0] == 1 && !buttonLogic.IsPressed() && Vector3.Distance(transform.localPosition, buttonTransform.localPosition) <1f)
        {
            buttonLogic.PressButton();
            AddReward(2f); // Reward for pressing the button
        }

        // Add a small penalty for each step
        AddReward(-0.0001f);
    }

    // Heuristic function for manual testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal"); // Horizontal movement
        continuousActions[1] = Input.GetAxisRaw("Vertical"); // Vertical movement

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0; // press button with Spacebar
    }

    // Handle collisions with objects
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target") && buttonLogic.IsPressed())
        {
            AddReward(10f); // Reward for reaching the target
            floorMeshRenderer.material = winMaterial;
            Debug.Log("Target Collected!");
            Destroy(other.gameObject);
            EndEpisode(); 
        }

        if (other.CompareTag("Wall"))
        {
            AddReward(-0.01f); // Punishment for hitting a wall
            floorMeshRenderer.material = loseMaterial;
            Debug.Log("Collided with Wall!");
            EndEpisode();
        }
    }

}