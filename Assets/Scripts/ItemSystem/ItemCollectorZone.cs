using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollectorZone : MonoBehaviour
{
    [SerializeField] GameObject _stackPointsParent;
    //[SerializeField] float _itemsToPlayerTime = 1f;

    MainCharacterController _mainCharacterController;
    Dictionary<GameObject, List<Item>> _stackedItemsDict = new Dictionary<GameObject, List<Item>>();
    void Start()
    {
        foreach(Transform child in _stackPointsParent.transform)
        {
            _stackedItemsDict.Add(child.gameObject, new List<Item>());
        }

        foreach (GameObject gameObject in _stackedItemsDict.Keys)
        {
            var stackPoint = gameObject.GetComponent<ItemCollectorZoneStackPoint>();
            if (stackPoint != null)
            {
                stackPoint.OnTriggerEnterSP += PlayerEnterSP;
            }
            else
            {
                Debug.LogError("stackPoint is null");
            }
        }

        _mainCharacterController = MainCharacterController.Instance;
    }

    private void PlayerEnterSP(object sender, Collider e)
    {
        var character = _mainCharacterController;
        if(character != null)
        {
            var stackPoint = sender as ItemCollectorZoneStackPoint;
            if (stackPoint != null)
            {
                var itemsToTransfer = new List<Item>(_stackedItemsDict[stackPoint.gameObject]);

                if(itemsToTransfer.Count > 0)
                {
                    foreach (Item item in itemsToTransfer)
                    {
                        TransferItemFromCollectorZone(item);
                        character.TransferItemToBackpack(item);
                        //// Create a single tween and update its position dynamically
                        //var itemTween = item.transform.DOMove(character.transform.position, _itemsToPlayerTime)
                        //    .OnComplete(() =>
                        //    {
                        //        TransferItemFromCollectorZone(item);
                        //        character.TransferItemToBackpack(item);
                        //    });

                        //// Dynamically update the target position on each frame
                        //itemTween.OnUpdate(() =>
                        //{
                        //    if (Vector3.Distance(item.transform.position, character.transform.position) > 0.5f) 
                        //    {
                        //        itemTween.ChangeEndValue(character.transform.position, true); // Update target position
                        //    }
                        //});
                    }
                }
            }
        }
    }

    // test
    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0,0, 200, 200), "Butt"))
    //    {
    //        AddItemToCollectorZone(itemEx);
    //    }
    //}

    public void TransferItemToCollectorZone(Item item)
    {
        GameObject lowestKey = null;
        int lowestValue = int.MaxValue;

        foreach (var kvp in _stackedItemsDict)
        {
            if (kvp.Value.Count < lowestValue)
            {
                lowestKey = kvp.Key;
                lowestValue = kvp.Value.Count;
            }
        }
        
        if (lowestKey != null)
        {
            _stackedItemsDict[lowestKey].Add(item);

            item.transform.position = lowestKey.transform.position + new Vector3(0, item.heightGap * (_stackedItemsDict[lowestKey].Count - 1), 0);
            item.transform.localRotation = item.Rotation();
            item.transform.parent = lowestKey.transform;
        }
    }
    private void TransferItemFromCollectorZone(Item item)
    {
        foreach (var kvp in _stackedItemsDict)
        {
            if (kvp.Value.Contains(item))
            {
                kvp.Value.Remove(item);
                item.transform.parent = null;

                break;
            }
        }
    }
    public void RemoveItemFromCollectorZone(Item item)
    {
        foreach (var kvp in _stackedItemsDict)
        {
            if (kvp.Value.Contains(item))
            {
                kvp.Value.Remove(item);
                item.transform.parent = null;
                Destroy(item.gameObject);

                break;
            }
        }
    }
}
