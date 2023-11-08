using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Resource _template;
    [SerializeField] private Transform _holder;
    [SerializeField] private float _radius;
    [SerializeField] private float _delay;

    private List<Transform> _resourcePoints;

    private void Start()
    {
        _resourcePoints = new List<Transform>();
        StartCoroutine(SpawnCycle());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    public List<Transform> GetExistsResources()
    {
        return _resourcePoints;
    }

    private void Spawn()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-_radius, _radius), 0 , Random.Range(-_radius, _radius))  + transform.position;
        Resource resource = Instantiate(_template, spawnPosition, Quaternion.identity, _holder);
        
        _resourcePoints.Add(resource.transform);
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
