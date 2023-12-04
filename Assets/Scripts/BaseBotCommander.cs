using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseBotCommander : MonoBehaviour
{
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private BotSpawner _botSpawner;
    [SerializeField] private Stock _stock;
    [SerializeField] private BaseScanner _baseScanner;
    [SerializeField] private UnityEvent _selected;
    [SerializeField] private UnityEvent _deselected;
    [SerializeField] private float _botOrderDelay;
    [SerializeField] private int _buildCost;

    private Queue<BotMover> _waitingBots;
    private BaseBuilder _baseBuilder;
    private bool _isNeedBuild;

    
    private void OnEnable()
    {
        _waitingBots = new Queue<BotMover>();

        _botSpawner.Spawned += OnSpawnBot;
    }

    private void Start()
    {
        StartCoroutine(BotOrderCycle());
    }

    private void OnDisable()
    {
        _botSpawner.Spawned -= OnSpawnBot;
        
        foreach (BotMover bot in _botSpawner.GetExistsBots())
        {
            bot.Waited -= OnWaitingBot;
        }
    }

    public void Select()
    {
        _selected?.Invoke();
    }

    public void Deselect()
    {
        _deselected?.Invoke();
    }

    public void TakeControlUnderBot(BotMover bot)
    {
        _botSpawner.TakeControl(bot);
    }
    
    public void ResetControlUnderBot(BotMover bot)
    {
        bot.Waited -= OnWaitingBot;
        _botSpawner.GetExistsBots().Remove(bot);
        _botSpawner.SetCanSpawn(true);
    }

    public void CreateBuildOrder(BaseBuilder baseBuilder)
    {
        _baseBuilder = baseBuilder;
        _isNeedBuild = true;
        _botSpawner.SetCanSpawn(false);
    }

    public void SetResourceSpawner(ResourceSpawner resourceSpawner)
    {
        _resourceSpawner = resourceSpawner;
    }

    public ResourceSpawner GetResourceSpawner()
    {
        return _resourceSpawner;
    }

    public int GetWaitingBotsCount()
    {
        return _waitingBots.Count;
    }

    private void OnSpawnBot(BotMover bot)
    {
        bot.Waited += OnWaitingBot;
        bot.SetStock(_stock.StockPoint);
    }
    
    private void OnWaitingBot(BotMover bot)
    {
        _waitingBots.Enqueue(bot);
    }

    private void GiveBotOrder()
    {
        if (_isNeedBuild == true && _waitingBots.Count > 0 && _stock.CollectedResourceCount >= _buildCost)
        {
            BotMover bot = _waitingBots.Dequeue();
            
            if (bot.IsWaitNotEmpty == false)
            {
                _isNeedBuild = false;
                _stock.DecreaseResource(_buildCost);
                _baseBuilder.SetBotBuilder(bot);
                bot.SetTarget(_baseBuilder.transform);
                bot.StartWork();
            }
            else
            {
                _waitingBots.Enqueue(bot);
            }
            
            return;
        }
        
        if (_baseScanner.GetResourcePoints().Count > 0 && _waitingBots.Count > 0)
        {
            BotMover bot = _waitingBots.Dequeue();
            Transform target = GetLessDistancePoint(bot.transform.position);
            
            bot.SetTarget(target);
            bot.StartWork();
            return;
        }

        if (_baseScanner.GetResourcePoints().Count <= 0 && _waitingBots.Count > 0 && AreNotEmptyBots() == true)
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
    
    private Transform GetLessDistancePoint(Vector3 botPosition)
    {
        List<Transform> availableResourcePoints = _baseScanner.GetResourcePoints();
        Transform point = availableResourcePoints[0];
        float distance = Vector3.Distance(botPosition, point.position);

        foreach (Transform resourcePoint in availableResourcePoints)
        {
            if (IsLessDistance(botPosition, resourcePoint.position, distance, out float newDistance) == true)
            {
                distance = newDistance;
                point = resourcePoint;
            }
        }
        
        availableResourcePoints.Remove(point);
        
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

        foreach (BotMover bot in _botSpawner.GetExistsBots())
        {
            if (bot.IsWaitNotEmpty == true)
            {
                result = true;
                break;
            }
        }

        return result;
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
