using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereLogic : MonoBehaviour
{
    public PlaneBalancerAgent planeBalancerAgent;

    // Add cooldown to avoid reward inflation and dwarfing other signals
    private float lastStillRewardTime = 0f;
    private float stillRewardCooldown = 1f;

    public void Start()
    {
        planeBalancerAgent = FindObjectOfType<PlaneBalancerAgent>();
    }

    // Add score if ball is in ScoreArea
    private void OnTriggerStay(Collider other)
    {
        if (planeBalancerAgent != null && other.CompareTag("Target"))
        {
            if (Time.time - lastStillRewardTime >= stillRewardCooldown)
            {
                if (planeBalancerAgent.ballIsStill)
                {
                    planeBalancerAgent.AddReward(+ 0.5f);
                    Debug.Log("Still in target area!");
                }
                else
                {
                    planeBalancerAgent.AddReward(+ 0.1f); // CURRICULUM 1
                    planeBalancerAgent.AddReward(+ 0.001f); // CURRICULUM 2
                    Debug.Log("Moving in target area!");
                }
            }
        }
    }
}
