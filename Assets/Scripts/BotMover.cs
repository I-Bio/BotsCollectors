using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BotResourceHolder))]
public class BotMover : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Rigidbody _rigidbody;
    private BotResourceHolder _resourceHolder;
    private Transform _target;
    private Coroutine _workCycle;
    private bool _canMove;
    private bool _canStartedWork;

    public event Action<BotMover> Waited;
    public event Action<Stock> StockReached;

    public Transform StockPoint { get; private set; }
    public bool IsWaitNotEmpty => IsWait == true && _resourceHolder.IsNotEmpty == true;
    public bool IsGoingToStock => _target == StockPoint;
    private bool IsWait => _target == null;
    
    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _resourceHolder = GetComponent<BotResourceHolder>();

        _resourceHolder.CapacityFilled += SetTargetToStock;
        _resourceHolder.NeedResources += ResetTarget;
    }

    private void OnDisable()
    {
        _resourceHolder.CapacityFilled -= SetTargetToStock;
        _resourceHolder.NeedResources -= ResetTarget;
    }

    private void Update()
    {
        CheckWorkCycle();
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _resourceHolder.SetTargetResource(target);
    }

    public void SetStock(Transform stockPoint)
    {
        bool isChangeStock = StockPoint is null;

        StockPoint = stockPoint;
        
        if (isChangeStock == true)
        {
            _workCycle = StartCoroutine(WorkCycle());
        }
    }
    
    public void SetTargetToStock()
    {
        _target = StockPoint;
    }
    
    public void StartWork()
    {
        _canStartedWork = true;
        _canMove = true;
    }
    
    public void ReachStock(Stock stock)
    {
        _canMove = false;
        _rigidbody.velocity = Vector3.zero;
        StockReached?.Invoke(stock);
    }

    private void CheckWorkCycle()
    {
        if (_canStartedWork == true && IsWait == false)
        {
            _canStartedWork = false;
            _workCycle = StartCoroutine(WorkCycle());
        }
    }

    private void ResetTarget()
    {
        _target = null;
    }

    private void Move()
    {
        if (_canMove == true)
        {
            Vector3 moveDirection = (_target.position - transform.position).normalized;
            SetRotation(moveDirection);
            _rigidbody.velocity = moveDirection * _speed;
        }
    }
    
    private void SetRotation(Vector3 moveDirection)
    {
        float angleOfRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
        
        if (angleOfRotation < 0)
        {
            angleOfRotation += 360;
        }
        
        transform.eulerAngles = new Vector3(0, angleOfRotation,0);
    }

    private IEnumerator WorkCycle()
    {
        while (IsWait == false)
        {
            Move();
            yield return null;
        }
        
        _rigidbody.velocity = Vector3.zero;

        Waited?.Invoke(this);
    }
}
