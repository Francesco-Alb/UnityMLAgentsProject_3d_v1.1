using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlaneBalancerAgent : Agent
{
    public Transform platform;
    private Quaternion platformRotation;

    public EnvironmentController environmentController;
    private Transform ball;
    private Rigidbody ballRb;

    public float rotationSpeed = 100f;
    public float maxTiltAng = 60f;
    public float tiltPenaltyThreshold = 0.4f;
    public float movementThreshold = 0.5f;
    [SerializeField] public bool ballIsStill = false;

    void FixedUpdate()
    {
        platform.localRotation = Quaternion.RotateTowards(
            platform.localRotation,
            platformRotation,
            rotationSpeed * Time.fixedDeltaTime);

        // Check if ball has fallen off
        if (ball != null && ball.localPosition.y < -10f)
        {
            AddReward(-1f);
            EndEpisode();
            Debug.Log("Ball fell off the platform.");
        }

        // Check if ball is moving
        if (ballRb.velocity.magnitude > 0.4f)
        {
            ballIsStill = false;
            Debug.Log("Object still moving");

            // AddReward(-0.001f); // CURRICULUM 1
            AddReward(-0.005f); // CURRICULUM 2
        }
        else
        {
            ballIsStill = true;
        }
    }

    public override void OnEpisodeBegin()
    {
        environmentController.ResetEnvironment();

        // Fetch newly spawned ball
        ball = environmentController.GetBallTransform();
        ballRb = environmentController.GetBallRigidbody();
        platformRotation = platform.localRotation;

        // Change ball's color depending on whether calmSpawn is true
        Renderer ballRenderer = ball.GetComponent<Renderer>();

        if (environmentController.calmSpawn)
        {
            ballRenderer.material.color = Color.green;
        }
        else
        {
            ballRenderer.material.color = Color.red;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (ball == null || ballRb == null)
        {
            // Pad with zeros to avoid warnings, maintain 9 observations
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(Vector3.zero);
            sensor.AddObservation(Vector3.zero);
            return;
        }

        sensor.AddObservation(ball.localPosition);
        sensor.AddObservation(ballRb.velocity);
        sensor.AddObservation(platform.localRotation.eulerAngles / 180f); // Normalize
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Extract the actions
        float tiltX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f);
        float tiltZ = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f);

        ApplyTilt(tiltX, tiltZ);

        // Add a small reward for each step (survival reward)
        // AddReward(+0.01f); // CURRICULUM 1
        AddReward(+0.001f);   // CURRICULUM 2
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuous = actionsOut.ContinuousActions;
        continuous[0] = Input.GetAxisRaw("Vertical");
        continuous[1] = Input.GetAxisRaw("Horizontal");
    }

    // Function to tilt platform
    private void ApplyTilt(float x, float z)
    {
        // Convert action to tilt in degrees and clamp to max tilt
        float angleX = x * maxTiltAng;
        float angleZ = z * maxTiltAng;

        if (Mathf.Abs(x) > movementThreshold || Mathf.Abs(z) > movementThreshold)
        {
            // set new rotation
            platformRotation = Quaternion.Euler(angleX, 0, angleZ);
        }
        else
        {
            platformRotation = Quaternion.Euler(0, 0, 0);   
        }

        // Penalty for action (to discourage twitchiness)
        if (Mathf.Abs(x) > tiltPenaltyThreshold || Mathf.Abs(z) > tiltPenaltyThreshold)
        {
            Debug.Log("Action Penalty.");
            // AddReward(-0.001f); // CURRICULUM 1
            AddReward(-0.005f); // CURRICULUM 2
        }
    }
}

// | Event                                   | Curriculum 1 Reward | Curriculum 2 Reward | Notes |
// |-----------------------------------------|---------------------|---------------------|-------|
// | Ball still in target area (per sec)     | -                   | +0.5                | Given once per `stillRewardCooldown` (1s) if ball is motionless in target. |
// | Ball moving in target area (per sec)    | +0.1                | +0.001              | Given once per cooldown if ball is inside target but moving. |
// | Survival step (each `OnActionReceived`) | +0.01               | +0.001              | Small shaping reward for staying alive. |
// | Ball moving (per `FixedUpdate`)         | -0.001              | -0.005              | Discourages excessive motion of the ball outside target. |
// | Action penalty (per `ApplyTilt` above threshold) | -0.001              | -0.005              | Discourages twitchy or unnecessary tilting. |
// | Ball falls off platform (once)          | -1.0                | -1.0                | Strong penalty; ends episode. |
