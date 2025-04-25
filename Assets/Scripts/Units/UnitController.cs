using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class UnitController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private Transform _locationToGoTo;
    void Start()
    {
        Assert.IsNotNull(_navMeshAgent, "NavMeshAgent reference is missing in UnitController.");
        Assert.IsNotNull(_locationToGoTo, "Location to go to reference is missing in UnitController.");
        _navMeshAgent.SetDestination(_locationToGoTo.position); // Set the destination for the NavMeshAgent to the specified location
    }
}
