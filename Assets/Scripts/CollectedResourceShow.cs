using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class CollectedResourceShow : MonoBehaviour
{
    [SerializeField] private Stock _stock;
    
    private TextMeshPro _text;

    private void OnEnable()
    {
        _text = GetComponent<TextMeshPro>();
        
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
