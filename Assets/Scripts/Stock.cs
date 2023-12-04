using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshRenderer))]
public class Stock : MonoBehaviour
{
    [SerializeField] private Material _standard;
    [SerializeField] private Material _selected;
    [SerializeField] private Transform _stockPoint;

    private MeshRenderer _meshRenderer;
    
    public event Action ResourceChanged;
    
    public Transform StockPoint => _stockPoint;
    public int CollectedResourceCount { get; private set; }

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BotMover botMover))
        {
            if (botMover.IsGoingToStock == true && botMover.StockPoint == _stockPoint)
            {
                botMover.ReachStock(this);
            }
        }
    }
    
    public void OnSelect()
    {
        _meshRenderer.material = _selected;
    }

    public void OnDeselect()
    {
        _meshRenderer.material = _standard;
    }

    public void IncreaseResource()
    {
        CollectedResourceCount++;
        ResourceChanged?.Invoke();
    }

    public void DecreaseResource(int resourceCount)
    {
        CollectedResourceCount -= resourceCount;
        ResourceChanged?.Invoke();
    }
}
