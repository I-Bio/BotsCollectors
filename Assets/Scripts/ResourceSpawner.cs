using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Resource _template;
    [SerializeField] private Transform _holder;
    [SerializeField] private float _radius;
    [SerializeField] private float _delay;

    public event Action<Transform> Spawned;

    private void Start()
    {
        StartCoroutine(SpawnCycle());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    private void Spawn()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-_radius, _radius), 0 , Random.Range(-_radius, _radius))  + transform.position;
        Resource resource = Instantiate(_template, spawnPosition, Quaternion.identity, _holder);
        Spawned?.Invoke(resource.transform);
    }

    private IEnumerator SpawnCycle()
    {
        var waitTime = new WaitForSeconds(_delay);
        bool isWorking = true;
        
        while (isWorking == true)
        {
            Spawn();
            yield return waitTime;
        }
    }
}
