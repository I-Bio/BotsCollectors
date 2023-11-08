using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BotMover))]
public class BotResourceHolder : MonoBehaviour
{
    [SerializeField] private Transform _stackPoint;
    [SerializeField] private Transform _unloadPoint;
    [SerializeField] private int _maxStackCapacity;
    [SerializeField] private float _unloadDelay;
    
    private Stack<Resource> _resources;
    private BotMover _botMover;
    private Vector3 _availableStackPosition;
    private bool _isStartUnloading;

    public event Action CapacityFilled;
    public event Action NeedResources;

    public Transform TargetResourcePoint { get; private set; }
    public bool IsEnoughCapacity => _maxStackCapacity > _resources.Count;
    public bool IsNotEmpty => _resources.Count > 0;
    
    private void OnEnable()
    {
        _resources = new Stack<Resource>();
        _botMover = GetComponent<BotMover>();
        
        _availableStackPosition = Vector3.zero;

        _botMover.StockReached += UnloadResources;
    }

    private void OnDisable()
    {
        _botMover.StockReached -= UnloadResources;
    }

    public void TakeResource(Resource resource)
    {
        Transform resourcePoint = resource.transform;
        resourcePoint.SetParent(_stackPoint);
        resourcePoint.localPosition = _availableStackPosition;
        TargetResourcePoint = null;
        
        _resources.Push(resource);
        _availableStackPosition += Vector3.up * resourcePoint.localScale.y;

        if (IsEnoughCapacity == false)
        {
            CapacityFilled?.Invoke();
        }
        else
        {
            NeedResources?.Invoke();
        }
    }

    public void SetTargetResource(Transform resourcePoint)
    {
        TargetResourcePoint = resourcePoint;
    }

    private void UnloadResources(Stock stock)
    {
        if (_isStartUnloading == false)
        {
            _isStartUnloading = true;
            StartCoroutine(UnloadCycle(stock));
        }
    }

    private void RemoveResource(Stock stock)
    {
        Resource resource = _resources.Pop();
        Destroy(resource.gameObject);
        stock.IncreaseResource();
    }
    
    private void DropResource()
    {
        Resource resource = _resources.Peek();
        Transform resourcePoint = resource.transform;
        
        resourcePoint.SetParent(_unloadPoint);
        resourcePoint.localPosition = Vector3.zero;
    }

    private IEnumerator UnloadCycle(Stock stock)
    {
        var waitTime = new WaitForSeconds(_unloadDelay);

        while (IsNotEmpty == true)
        {
            DropResource();
            yield return waitTime;
            RemoveResource(stock);
        }

        _isStartUnloading = false;
        _availableStackPosition = Vector3.zero;
        NeedResources?.Invoke();
    }
}
