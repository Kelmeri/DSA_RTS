using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTS.Runtime
{
    public class DestroyAfterSeconds : MonoBehaviour
    {
        [SerializeField] private float _destroyTime = 3f; // Time in seconds before the object is destroyed
        IEnumerator Start()
        {
            yield return new WaitForSeconds(_destroyTime); // Wait for the specified time
            Destroy(gameObject); // Destroy the object

        }
    }
}
