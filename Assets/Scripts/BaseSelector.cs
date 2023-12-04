using UnityEngine;
using UnityEngine.UI;

public class BaseSelector : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _panel;
    [SerializeField] private FlagSpaceChecker _buildFlag;
    
    private BaseBotCommander _currentSelected;

    private void Update()
    {
        CheckClick();
    }
    
    private void CheckClick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) == true)
        {
            if (_buildFlag.CanMoveFlag == false)
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

                var baseBotCommander = GetFirstComponentOnClick<BaseBotCommander>(ray);
                
                if (baseBotCommander != null)
                {
                    SetCurrentSelected(baseBotCommander);
                }
            }
            else
            {
                _buildFlag.CreateBaseBuilder(_currentSelected);
                DeselectCurrentBase();
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (_buildFlag.CanMoveFlag == true)
            {
                _buildFlag.HideBuildFlag();
            }

            DeselectCurrentBase();
        }
    }

    private void SetCurrentSelected(BaseBotCommander baseBotCommander)
    {
        if (_currentSelected != null)
        {
            _currentSelected.Deselect();
        }
        else
        {
            _panel.SetActive(true);
        }
        
        _currentSelected = baseBotCommander;
        _currentSelected.Select();
    }

    private void DeselectCurrentBase()
    {
        if (_buildFlag.CanMoveFlag == false)
        {
            if (_currentSelected != null)
            {
                _currentSelected.Deselect();
                _currentSelected = null;
            }

            _panel.SetActive(false);
        }
    }
    
    private T GetFirstComponentOnClick<T>(Ray ray) where T : class
    {
        var result = Physics.RaycastAll(ray);

        foreach (var raycast in result)
            if (raycast.transform.TryGetComponent(out T component))
                return component;
            
        return null;
    }
}
