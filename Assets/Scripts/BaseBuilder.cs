using UnityEngine;

public class BaseBuilder : MonoBehaviour
{
    [SerializeField] private BaseBotCommander _template;

    private BaseBotCommander _mainBase;
    private BotMover _botBuilder;
    private bool _isBuilt;
    
    private void OnTriggerEnter(Collider other)
    {
        if (_isBuilt == false && other.TryGetComponent(out BotMover bot))
        {
            if (bot == _botBuilder)
            {
                _isBuilt = true;
                BuildNewBase(bot);
            }
        }
    }

    public void SetMainBase(BaseBotCommander mainBase)
    {
        _mainBase = mainBase;
    }

    public void SetBotBuilder(BotMover botBuilder)
    {
        _botBuilder = botBuilder;
    }

    private void BuildNewBase(BotMover bot)
    {
        BaseBotCommander newBase = Instantiate(_template, transform.position, Quaternion.Euler(transform.eulerAngles));
        _mainBase.ResetControlUnderBot(bot);
        newBase.TakeControlUnderBot(bot);
        newBase.SetResourceSpawner(_mainBase.GetResourceSpawner());

        Destroy(gameObject);
    }
}
