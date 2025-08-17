using System.Collections;
using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEditor.AnimatedValues;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform platform;

    private GameObject currentBall;
    private Rigidbody ballRb;

    public float spawnRange = 1f;
    public bool calmSpawn = false;

    public void Initialize()
    {
        ballRb = ballPrefab.GetComponent<Rigidbody>();
    }

    public void ResetEnvironment()
    {
        // Destroy existing target, if any
        if (currentBall != null)
        {
            Destroy(currentBall);
        }

        // Instantiate new ball
        Vector3 spawnOffset = new Vector3(
            Random.Range(-spawnRange, spawnRange),
            1.0f,
            Random.Range(-spawnRange, spawnRange)
        );

        Vector3 spawnPos = platform.position + platform.TransformDirection(spawnOffset);
        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
        ballRb = currentBall.GetComponent<Rigidbody>();

        // Apply random force at spawn
        Vector3 forceDir = new Vector3(
            Random.Range(-1f, 1f),
            0f, // No vertical force
            Random.Range(-1f, 1f)
        ).normalized;

        // Implement a logic whereby there's a random chance (1 out of 10) of the ball "calm spawning" (for better learning)
        if (Random.Range(0, 10) == 0)
        {
            calmSpawn = true;
        }
        else
        {
            calmSpawn = false;
        }

        float impulseMultiplier = calmSpawn ? 0.5f : 1f;
        float forceMag = Random.Range(3f, 7f);
        ballRb.AddForce(forceDir * forceMag * impulseMultiplier, ForceMode.Impulse);

        // DEBUG
        if (calmSpawn)
        {
            Debug.Log("💤 Calm start!");
        }

        // Apply random spin at spawn
        Vector3 spin = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ) * 3f;

        ballRb.AddTorque(spin * impulseMultiplier, ForceMode.Impulse);
    }

    public Transform GetBallTransform()
    {
        return currentBall?.transform;
    }

    public Rigidbody GetBallRigidbody()
    {
        return ballRb;
    }

}
