using UnityEngine;

[RequireComponent (typeof(Collider))]
public class ItemPickZone : MonoBehaviour
{
    [SerializeField] Item _item;

    MainCharacterController _characterController;
    private void Start()
    {
        _characterController = MainCharacterController.Instance;
    }

    public void AddItem()
    {
        _characterController?.AddItemToBackpack(_item);
    }
}
