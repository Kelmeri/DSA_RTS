using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navMeshAgent; // Reference to the NavMeshAgent component
    [SerializeField] private Transform _locationToGoTo;
    void Start()
    {
        UnityEngine.Assertions.Assert.IsNotNull(_navMeshAgent, "NavMeshAgent reference is missing in UnitController."); // Assert that the NavMeshAgent is not null
        UnityEngine.Assertions.Assert.IsNotNull(_locationToGoTo, "Location to go to reference is missing in UnitController."); // Assert that the location to go to is not null
        _navMeshAgent.SetDestination(_locationToGoTo.position); // Set the destination for the NavMeshAgent to the specified location
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
