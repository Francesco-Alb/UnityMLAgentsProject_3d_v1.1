using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLogicX : MonoBehaviour
{
    [SerializeField] private Vector3 spawnAreaMin = new Vector3(-4.5f, 0, -4f); // Min bounds for target spawn
    [SerializeField] private Vector3 spawnAreaMax = new Vector3(4.5f, 0, 4f);   // Max bounds for target spawn

    public void ResetTarget()
    {
        // Randomize the target's position
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomZ = Random.Range(spawnAreaMin.z, spawnAreaMax.z);
        transform.localPosition = new Vector3(randomX, 0.5f, randomZ); // Keep Y consistent

        // Reset target state (disabled by default)
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    public Vector3 GetTargetPosition()
    {
        return transform.localPosition;
    }
}
