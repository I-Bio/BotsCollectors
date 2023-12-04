using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class FlagSpaceChecker : MonoBehaviour
{
    [SerializeField] private BaseBuilder _template;
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _ground;
    [SerializeField] private LayerMask _flagInteract;
    [SerializeField] private Material _allowed;
    [SerializeField] private Material _notAllowed;
    [SerializeField] private float _rayToGroundDistance;
    [SerializeField] private float _rotateAngle;
    [SerializeField] private float _yOffset;
    [SerializeField] private float _buildingYOffset;
    [SerializeField] private int _extentsMultiplier;

    private MeshRenderer _meshRenderer;
    private BaseBuilder _currentFlag;
    
    public bool CanMoveFlag { get; private set; }

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        MoveBuildFlag();
    }

    public void ShowBuildFlag()
    {
        gameObject.SetActive(true);
        CanMoveFlag = true;
    }

    public void HideBuildFlag()
    {
        gameObject.SetActive(false);
        CanMoveFlag = false;
    }
    
    public void CreateBaseBuilder(BaseBotCommander mainBase)
    {
        if (IsExistPlace() == true)
        {
            Vector3 flagPosition = transform.position;
            Vector3 baseBuilderPosition = new Vector3(flagPosition.x, flagPosition.y - _yOffset + _buildingYOffset,
                flagPosition.z);
            
            if (_currentFlag == null)
            {
                BaseBuilder baseBuilder = Instantiate(_template, baseBuilderPosition, Quaternion.Euler(transform.eulerAngles));
                _currentFlag = baseBuilder;
                
                baseBuilder.SetMainBase(mainBase);
                mainBase.CreateBuildOrder(baseBuilder);
            }
            else
            {
                Transform currentFlagPoint = _currentFlag.transform;
                
                currentFlagPoint.position = baseBuilderPosition;
                currentFlagPoint.eulerAngles = transform.eulerAngles;
            }
        }
    }
    
    private void MoveBuildFlag()
    {
        if (CanMoveFlag == true && Input.GetKeyDown(KeyCode.R) == true)
        {
            transform.Rotate(Vector3.up, _rotateAngle);
        }
        
        if (CanMoveFlag == true)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            float maxDistance = 100;

            if (Physics.Raycast(ray, out hit, maxDistance, _ground))
            {
                if (IsExistPlace() == true)
                {
                    _meshRenderer.material = _allowed;
                }
                else
                {
                    _meshRenderer.material = _notAllowed;
                }

                hit.point = new Vector3(hit.point.x, hit.point.y + _yOffset, hit.point.z);
                
                transform.position = hit.point;
            }
        }
    }

    private bool IsExistPlace()
    {
        bool result = Physics.CheckBox(transform.position,
            transform.localScale * _extentsMultiplier,
            Quaternion.Euler(transform.eulerAngles),
            _flagInteract);
        
        return result != true;
    }
}
