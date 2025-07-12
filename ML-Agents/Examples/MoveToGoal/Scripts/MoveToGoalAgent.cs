using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent {

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    [SerializeField] private float speed = 1f;

    public override void OnEpisodeBegin()
    {
        // Generate random spawn positions for the agent and the target
        float xAgentSpawn = Random.Range(-4.5f, +4.5f);
        float zAgentSpawn = Random.Range(-4f, +4f);
        float xTargetSpawn = Random.Range(-4.5f, +4.5f);
        float zTargetSpawn = Random.Range(-4f, +4f);

        // Set the agent's position
        transform.localPosition = new Vector3(xAgentSpawn, 0, zAgentSpawn);

        // Set the target's position
        targetTransform.localPosition = new Vector3(xTargetSpawn, 0, zTargetSpawn);
    }

    public override void CollectObservations(VectorSensor sensor)
    {   
        // SPACE SIZE VECTOR OBS = 6: each of these pass 3 values, as positions are x,y,z 
        sensor.AddObservation(transform.localPosition); //
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        Debug.Log(actions.ContinuousActions[0]);

        transform.position += new Vector3(moveX, 0, moveZ) * Time.deltaTime * speed;

        // Add a small penalty for each step
        AddReward(-0.0001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Target"))
        {
            Debug.Log("Target Reached!");
            AddReward(5f);
            floorMeshRenderer.material = winMaterial;
            EndEpisode();
        }
        if (other.CompareTag("Wall"))
        {
            Debug.Log("Collided with a wall!");
            AddReward(-0.01f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }
}
