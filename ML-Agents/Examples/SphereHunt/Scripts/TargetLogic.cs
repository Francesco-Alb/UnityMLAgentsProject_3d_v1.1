using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLogic : MonoBehaviour
{
    [SerializeField] public GameObject targetPrefab; // Prefab of the target object

    private GameObject currentTargetInstance; // To keep track of the currently instantiated target

    // Method to spawn a new target
    public void SpawnTarget() // Made private as it's only called internally
    {
        // Randomize the target's spawn position
        float randomX = Random.Range(-4.5f,+4.5f);
        float randomZ = Random.Range(-4f,+4f);
        Vector3 spawnerPosition = new Vector3(randomX,2,randomZ);

        // Instantiate the target as a child of *this* GameObject, at the local position
        currentTargetInstance = Instantiate(targetPrefab, this.transform); // Store the instantiated target
        currentTargetInstance.transform.localPosition = spawnerPosition; // Set the local position
    }

    // Method to reset the target (Destroys old and spawns new)
    public void ResetTarget()
    {
        // Destroy any existing target instance before spawning a new one
        if (currentTargetInstance != null)
        {
            Destroy(currentTargetInstance);
        }
    }

    public Vector3 GetTargetPosition()
    {
        // Get position of the *current* target instance (environment-specific)
        if (currentTargetInstance != null)
        {
            return currentTargetInstance.transform.localPosition; // Access position of the instance we are tracking
        }
        return Vector3.zero; // Or handle case where target doesn't exist (though it should after ResetTarget)
    }
}