using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class BotSpawner : MonoBehaviour
{
    [SerializeField] private BotMover _template;
    [SerializeField] private Stock _stock;
    [SerializeField] private Transform _botSpawnPoint;
    [SerializeField] private Material _standard;
    [SerializeField] private Material _selected;
    [SerializeField] private int _maxBotsCount;
    [SerializeField] private int _spawnCost;
    [SerializeField] private float _botSpawnDelay;
    
    private List<BotMover> _bots;
    private MeshRenderer _meshRenderer;
    private bool _canSpawn;

    public event Action<BotMover> Spawned;

    private void OnEnable()
    {
        _bots = new List<BotMover>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _canSpawn = true;
    }

    private void Start()
    {
        StartCoroutine(BotSpawnCycle());
    }

    public List<BotMover> GetExistsBots()
    {
        return _bots;
    }

    public void OnSelect()
    {
        _meshRenderer.material = _selected;
    }

    public void OnDeselect()
    {
        _meshRenderer.material = _standard;
    }

    public void TakeControl(BotMover bot)
    {
        Spawned?.Invoke(bot);
        _bots.Add(bot);
    }

    public void SetCanSpawn(bool canSpawn)
    {
        _canSpawn = canSpawn;
    }

    private void SpawnBot()
    {
        if (_canSpawn == true && _bots.Count < _maxBotsCount && _stock.CollectedResourceCount >= _spawnCost)
        {
            CreateBot();
            _stock.DecreaseResource(_spawnCost);
            return;
        }

        if (_bots.Count == 0)
        {
            CreateBot();
        }
    }

    private void CreateBot()
    {
        BotMover bot = Instantiate(_template, _botSpawnPoint.position, Quaternion.identity);
        Spawned?.Invoke(bot);
        _bots.Add(bot);
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
