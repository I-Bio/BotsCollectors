using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Stock : MonoBehaviour
{
    [SerializeField] private float _minStopDelay;
    [SerializeField] private float _maxStopDelay;

    public event Action ResourceChanged;
    
    public int CollectedResourceCount { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out BotMover botMover))
        {
            StartCoroutine(StopBot(botMover));
        }
    }

    public void IncreaseResource()
    {
        CollectedResourceCount++;
        ResourceChanged?.Invoke();
    }

    private IEnumerator StopBot(BotMover botMover)
    {
        var waitTime = new WaitForSeconds(Random.Range(_minStopDelay, _maxStopDelay));
        yield return waitTime;
        botMover.ReachStock(this);
    }
}
