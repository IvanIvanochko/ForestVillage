using System;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [SerializeField] ManageBuilding[] buildings;
    [SerializeField] UnlockBuildingPair[] unlockBuildings;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        foreach (UnlockBuildingPair pair in unlockBuildings)
        {
            pair.unlock.SetActive(pair.isAvailable);
        }
        foreach (UnlockBuildingPair pair in unlockBuildings)
        {
            pair.building.OnBuildingSpawned += (sender, e) => Building_OnBuildingSpawned(pair);
        }
    }

    private void Building_OnBuildingSpawned(UnlockBuildingPair pair)
    {
        if (pair != null && pair.unlock != null)
        {
            pair.unlock.SetActive(true);
        }
    }

    public ManageBuilding GetBuildingManager(Building building)
    {
        foreach(ManageBuilding manage in buildings)
        {
            if (manage.building == building)
                return manage;
        }
        return null;
    }
}

[Serializable]
public class UnlockBuildingPair
{
    public string name;
    public Building building;
    public GameObject unlock;
    public bool isAvailable = false;
}

[Serializable]
public class ManageBuilding
{
    public string name;
    public Building building;
    public bool isUnlocked;
}