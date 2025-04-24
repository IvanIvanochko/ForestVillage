using System;
using UnityEngine;

public class ItemCollectorZoneStackPoint : MonoBehaviour
{
    public event EventHandler<Collider> OnTriggerEnterSP;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterSP?.Invoke(this, other);
    }
}
