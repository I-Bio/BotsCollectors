using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    [SerializeField] private BotMover _template;
    [SerializeField] private Transform _botSpawnPoint;
    [SerializeField] private int _maxBotsCount;
    [SerializeField] private float _botSpawnDelay;
    
    private List<BotMover> _bots;

    public event Action<BotMover> Spawned;

    private void Start()
    {
        _bots = new List<BotMover>();
        
        StartCoroutine(BotSpawnCycle());
    }

    public List<BotMover> GetExistsBots()
    {
        return _bots;
    }
    
    private void SpawnBot()
    {
        if (_bots.Count < _maxBotsCount)
        {
            BotMover bot = Instantiate(_template, _botSpawnPoint.position, Quaternion.identity);
            Spawned?.Invoke(bot);
            _bots.Add(bot);
        }
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
}
