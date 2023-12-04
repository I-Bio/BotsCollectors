using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseScanner : MonoBehaviour
{
    [SerializeField] private float _scanDelay;
    [SerializeField] private BaseBotCommander _baseBotCommander;
    
    private List<Transform> _availableResourcePoints;
    private bool _isRequestResources;

    private void OnEnable()
    {
        _availableResourcePoints = new List<Transform>();
    }

    private void Start()
    {
        StartCoroutine(ScanResourcesCycle());
    }

    public void TakeResourcePointsList(List<Transform> resourcePoints)
    {
        resourcePoints.ForEach(point => _availableResourcePoints.Add(point));
        _isRequestResources = false;
    }

    public List<Transform> GetResourcePoints()
    {
        return _availableResourcePoints;
    }

    private void ScanForResources()
    {
        if (_isRequestResources == false && _baseBotCommander.GetWaitingBotsCount() > _availableResourcePoints.Count)
        {
            _isRequestResources = true;
            
            KeyValuePair<BaseScanner, int> request = new KeyValuePair<BaseScanner, int>(this, _baseBotCommander.GetWaitingBotsCount());
            
            _baseBotCommander.GetResourceSpawner().AddResourceRequest(request);
        }
    }
    
    private IEnumerator ScanResourcesCycle()
    {
        bool isWorking = true;
        var waitTime = new WaitForSeconds(_scanDelay);

        while (isWorking == true)
        {
            ScanForResources();
            yield return waitTime;
        }
    }
}
