using System;
using UnityEngine;

public class Resource : MonoBehaviour
{
    private bool _isTook;

    private void OnTriggerStay(Collider other)
    {
        if (_isTook == false && other.TryGetComponent(out BotResourceHolder bot))
        {
            if (transform == bot.TargetResourcePoint && bot.IsEnoughCapacity == true) 
            {
                _isTook = true;
                bot.TakeResource(this);
            }
        }
    }
}
