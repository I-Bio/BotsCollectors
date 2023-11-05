using TMPro;
using UnityEngine;

public class CollectedResourceShow : MonoBehaviour
{
    [SerializeField] private Stock _stock;
    [SerializeField] private TextMeshProUGUI _text;

    private void OnEnable()
    {
        _stock.ResourceChanged += OnChangeResource;
    }

    private void OnDisable()
    {
        _stock.ResourceChanged -= OnChangeResource;
    }

    private void OnChangeResource()
    {
        _text.SetText(_stock.CollectedResourceCount.ToString());
    }
}
