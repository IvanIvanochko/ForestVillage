using System;
using UnityEngine;

public class ItemProduction : MonoBehaviour
{
    [SerializeField] Item _itemToProduce;
    [SerializeField] ItemCollectorZone _collectorZone;
    [SerializeField] ProgressBarCircle _progressBar;
    [SerializeField] float _timeProductionPerItem = 1f;

    public event EventHandler OnItemProduced;

    float _cooldown = 0;
    int _itemsInQueue = 0;
    private void Awake()
    {
        OnItemProduced += ItemProduced;
    }

    void FixedUpdate()
    {
        if (_itemsInQueue == 0)
        {
            _cooldown = _timeProductionPerItem;
        }
        else if (_cooldown >= 0 && _itemsInQueue > 0)
        {
            _cooldown -= Time.deltaTime;
            _progressBar.SetSliderValue((_timeProductionPerItem - _cooldown) / _timeProductionPerItem);
        }
        else if(_cooldown <= 0 && _itemsInQueue > 0)
        {
            OnItemProduced?.Invoke(this, EventArgs.Empty);
        }
    }
    private void ItemProduced(object sender, EventArgs e)
    {
        _collectorZone.TransferItemToCollectorZone(_itemToProduce.Instantiate().GetComponent<Item>());

        _itemsInQueue--;

        if (_itemsInQueue > 0)
            _cooldown = _timeProductionPerItem;

        _progressBar.SetSliderValue(1);
    }

    public void ProduceItemRequest() => _itemsInQueue++;
}
