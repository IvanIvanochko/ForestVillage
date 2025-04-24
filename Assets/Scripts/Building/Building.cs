using DG.Tweening;
using System;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Item Donate UI")]
    [SerializeField] ItemDonateZone itemDonate;
    [SerializeField] float heightBelowTerrainUI;
    [Header("Building")]
    [SerializeField] GameObject building;
    [SerializeField] float heightBelowTerrainB;
    [Header("Construction")]
    [SerializeField] GameObject construction;
    [SerializeField] float heightBelowTerrainC;

    public event EventHandler OnBuildingSpawned;

    private void Awake()
    {
        building.SetActive(false);
        construction.SetActive(true);

        construction.transform.localPosition = Vector3.zero;
        building.transform.localPosition = new Vector3(0, heightBelowTerrainB, 0);
    }

    private void Start()
    {
        if (BuildingManager.Instance != null
            && BuildingManager.Instance.GetBuildingManager(this).isUnlocked)
        {
            SpawnBuilding(this, EventArgs.Empty);
        }
        else
        {
            itemDonate.OnAllAmountDonated += SpawnBuilding;
        }
    }

    private void SpawnBuilding(object sender, System.EventArgs e)
    {
        building.SetActive(true);
        construction.SetActive(false);

        building.transform.DOMove(transform.position, 2f);
        construction.transform.DOMove(transform.position - 
            new Vector3(0, heightBelowTerrainC, 0), 2f);

        itemDonate.transform.DOMove(itemDonate.transform.position +
            new Vector3(0, heightBelowTerrainUI, 0), 2f)
            .OnComplete(() => itemDonate.gameObject.SetActive(false));

        OnBuildingSpawned?.Invoke(this, EventArgs.Empty);
        Debug.Log("Building Spawned: " + gameObject);
    }
}
