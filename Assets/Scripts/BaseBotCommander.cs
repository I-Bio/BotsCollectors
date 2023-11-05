using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBotCommander : MonoBehaviour
{
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private BotMover _template;
    [SerializeField] private Transform _stockPoint;
    [SerializeField] private Transform _botSpawnPoint;
    [SerializeField] private int _maxBotsCount;
    [SerializeField] private float _botSpawnDelay;
    [SerializeField] private float _botOrderDelay;
    
    private List<BotMover> _bots;
    private List<Transform> _availableResourcePoints;
    private Queue<BotMover> _waitingBots;

    private void OnEnable()
    {
        _bots = new List<BotMover>();
        _availableResourcePoints = new List<Transform>();
        _waitingBots = new Queue<BotMover>();

        StartCoroutine(BotSpawnCycle());
        StartCoroutine(BotOrderCycle());
        
        _resourceSpawner.Spawned += OnSpawnResource;
    }

    private void OnDisable()
    {
        foreach (BotMover bot in _bots)
        {
            bot.Waited -= OnWaitingBot;
        }
        
        _resourceSpawner.Spawned -= OnSpawnResource;
    }

    private void OnSpawnResource(Transform resourcePoint)
    {
        _availableResourcePoints.Add(resourcePoint);
    }

    private void SpawnBot()
    {
        if (_bots.Count < _maxBotsCount)
        {
            BotMover bot = Instantiate(_template, _botSpawnPoint.position, Quaternion.identity);
            bot.Waited += OnWaitingBot;
            bot.SetStock(_stockPoint);
            _bots.Add(bot);
        }
    }

    private void GiveBotOrder()
    {
        if (_availableResourcePoints.Count > 0 && _waitingBots.Count > 0)
        {
            BotMover bot = _waitingBots.Dequeue();
            Transform target = GetLessDistancePoint(bot.transform.position);
            
            bot.SetTarget(target);
            bot.StartWork();
            return;
        }

        if (_availableResourcePoints.Count <= 0 && _waitingBots.Count > 0 && AreNotEmptyBots() == true)
        {
            if (_waitingBots.Peek().IsWaitNotEmpty == true)
            {
                BotMover bot = _waitingBots.Dequeue();
                bot.SetTargetToStock();
                bot.StartWork();
            }
            else
            {
                BotMover bot = _waitingBots.Dequeue();
                _waitingBots.Enqueue(bot);
            }
        }
    }
    
    private void OnWaitingBot(BotMover bot)
    {
        _waitingBots.Enqueue(bot);
    }

    private Transform GetLessDistancePoint(Vector3 botPosition)
    {
        Transform point = _availableResourcePoints[0];
        float distance = Vector3.Distance(botPosition, point.position);

        foreach (Transform resourcePoint in _availableResourcePoints)
        {
            if (IsLessDistance(botPosition, resourcePoint.position, distance, out float newDistance))
            {
                distance = newDistance;
                point = resourcePoint;
            }
        }

        _availableResourcePoints.Remove(point);
        
        return point;
    }

    private bool IsLessDistance(Vector3 botPosition, Vector3 resourcePosition, float currentDistance, out float distance)
    {
        distance = Vector3.Distance(botPosition, resourcePosition);
        return distance < currentDistance;
    }
    
    private bool AreNotEmptyBots()
    {
        bool result = false;

        foreach (BotMover bot in _bots)
        {
            if (bot.IsWaitNotEmpty == true)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    private IEnumerator BotSpawnCycle()
    {
        bool isWorking = true;
        var waitTime = new WaitForSeconds(_botSpawnDelay);

        while (isWorking == true)
        {
            SpawnBot();
            yield return waitTime;
        }
    }

    private IEnumerator BotOrderCycle()
    {
        bool isWorking = true;
        var waitTime = new WaitForSeconds(_botOrderDelay);

        while (isWorking == true)
        {
            GiveBotOrder();
            yield return waitTime;
        }
    }
}
