using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Resource _template;
    [SerializeField] private Transform _holder;
    [SerializeField] private float _radius;
    [SerializeField] private float _spawnDelay;
    [SerializeField] private float _distributeDelay;

    private List<Transform> _resourcePoints;
    private Queue<KeyValuePair<BaseScanner, int>> _baseRequests;

    private void OnEnable()
    {
        _resourcePoints = new List<Transform>();
        _baseRequests = new Queue<KeyValuePair<BaseScanner, int>>();
    }

    private void Start()
    {
        StartCoroutine(SpawnCycle());
        StartCoroutine(DistributeCycle());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    public void AddResourceRequest(KeyValuePair<BaseScanner, int> request)
    {
        _baseRequests.Enqueue(request);
    }
    
    private List<Transform> GetExistsResources(int count)
    {
        List<Transform> result = _resourcePoints.GetRange(0, count);
        _resourcePoints = _resourcePoints.GetRange(count, _resourcePoints.Count - count);
        return result;
    }

    private void DistributeResources()
    {
        if (_baseRequests.Count > 0)
        {
            KeyValuePair<BaseScanner, int> baseRequest = _baseRequests.Peek();

            if (_resourcePoints.Count >= baseRequest.Value)
            {
                int bonusMultiplier = 2;
                int resourceCount;

                if (_resourcePoints.Count > baseRequest.Value * bonusMultiplier)
                {
                    resourceCount = baseRequest.Value * bonusMultiplier;
                }
                else
                {
                    resourceCount = baseRequest.Value;
                }
                
                baseRequest = _baseRequests.Dequeue();
                baseRequest.Key.TakeResourcePointsList(GetExistsResources(resourceCount));
            }
        }
    }

    private void Spawn()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-_radius, _radius), 0 , Random.Range(-_radius, _radius))  + transform.position;
        Resource resource = Instantiate(_template, spawnPosition, Quaternion.identity, _holder);
        
        _resourcePoints.Add(resource.transform);
    }

    private IEnumerator SpawnCycle()
    {
        var waitTime = new WaitForSeconds(_spawnDelay);
        bool isWorking = true;
        
        while (isWorking == true)
        {
            Spawn();
            yield return waitTime;
        }
    }

    private IEnumerator DistributeCycle()
    {
        var waitTime = new WaitForSeconds(_spawnDelay);
        bool isWorking = true;
        
        while (isWorking == true)
        {
            DistributeResources();
            yield return waitTime;
        }
    }
}
