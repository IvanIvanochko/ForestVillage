using System;
using UnityEngine;

[RequireComponent (typeof(Collider))]
public class Tree : MonoBehaviour
{
    [SerializeField] ItemPickZone _itemPickZone;
    [SerializeField] int _itemsCount;
    [SerializeField] float cutSpeed;

    bool _isPlayerInside;
    int _currentAnimRep = 0;
    MainCharacterController _characterController;
    private void Start()
    {
        _characterController = MainCharacterController.Instance;
        _characterController.OnAxeAnimFinished += DamageTree;
    }
    private void OnTriggerEnter(Collider other)
    {
        _isPlayerInside = true;
        _characterController.AxeAnimation(cutSpeed);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerLeave();
    }
    private void DamageTree(object sender, EventArgs e)
    {
        if (!_isPlayerInside) return;

        _currentAnimRep++;
        _itemPickZone?.AddItem();

        if (_currentAnimRep == _itemsCount)
        {
            gameObject.SetActive(false);
            PlayerLeave();
        }
    }

    void PlayerLeave()
    {
        _characterController.DefaultUpperBodyAnimation();
        _isPlayerInside = false;
        _currentAnimRep = 0;
    }
}
