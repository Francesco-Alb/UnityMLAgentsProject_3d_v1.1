using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class SphereHuntAgentX : Agent
{
    [SerializeField] private Transform buttonTransform; // Reference to the button's Transform
    [SerializeField] private ButtonLogicX buttonLogic;   // Script handling button behavior
    [SerializeField] private TargetLogicX targetLogic;   // Script handling target behavior
    [SerializeField] private Material winMaterial;      // Material for success (target reached)
    [SerializeField] private Material loseMaterial;     // Material for failure (wall collision)
    [SerializeField] private MeshRenderer floorMeshRenderer; // Renderer for floor feedback
    [SerializeField] private float speed = 1f;          // Movement speed of the agent

    private bool buttonPressed = false;                 // Track if the button was pressed

    // Called at the start of each episode
public override void OnEpisodeBegin()
{
    buttonPressed = false;                          
    buttonLogic.ResetButton();                      // Resets both the button and its base
    targetLogic.ResetTarget();                      // Randomize and reset the target

    // Randomize agent's position
    float xAgentSpawn = Random.Range(-4.5f, +4.5f);
    float zAgentSpawn = Random.Range(-4f, +4f);

    // Ensure the agent does not spawn too close to the button's base
    while (Vector3.Distance(new Vector3(xAgentSpawn, 0, zAgentSpawn), buttonLogic.buttonBaseTransform.localPosition) < 1f)
    {
        xAgentSpawn = Random.Range(-4.5f, +4.5f);
        zAgentSpawn = Random.Range(-4f, +4f);
    }

    // Set the agent's position
    transform.localPosition = new Vector3(xAgentSpawn, 0, zAgentSpawn);
}

    // Collects observations for the ML agent
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition); // Agent's position
        sensor.AddObservation(buttonTransform.localPosition); // Button's position
        sensor.AddObservation(buttonPressed ? 1f : 0f); // Whether the button has been pressed

        if (buttonPressed)
        {
            sensor.AddObservation(targetLogic.GetTargetPosition()); // Target's position after the button is pressed
        }
    }

    // Receives actions from the ML algorithm
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed;

        // Handle button press action
        if (actions.DiscreteActions[0] == 1 && !buttonPressed && Vector3.Distance(transform.position, buttonTransform.position) < 1f)
        {
            buttonLogic.PressButton();
            buttonPressed = true;
            AddReward(0.5f); // Reward for pressing the button
        }
    }

    // Heuristic function for manual testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal"); // Map horizontal movement
        continuousActions[1] = Input.GetAxisRaw("Vertical");   // Map vertical movement

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetKey(KeyCode.Space) ? 1 : 0; // Press button with spacebar
    }

    // Handle collisions with objects
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target") && buttonPressed)
        {
            AddReward(1f); // Reward for reaching the target
            floorMeshRenderer.material = winMaterial;
            EndEpisode();  // End the episode successfully
        }

        if (other.CompareTag("Wall"))
        {
            AddReward(-1f); // Penalty for hitting a wall
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();   // End the episode with failure
        }
    }
}
