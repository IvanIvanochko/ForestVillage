using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public enum DonateBehavior
{
    Destroy,
    CollectZone,
    ItemProduction
}

[RequireComponent(typeof(Collider))]
public class ItemDonateZone : MonoBehaviour
{
    [Header("Donate Behavior")]
    [SerializeField] DonateBehavior _donateBehavior;
    [HideInInspector][SerializeField] ItemCollectorZone _collectorZone;
    [HideInInspector][SerializeField] ItemProduction _production;
    [Header("Essentials")]
    [SerializeField] Item _requiredItem;
    [SerializeField] TextMeshProUGUI _textMeshPro;
    [Header("Options")]
    [SerializeField] bool _isDonateAllAtOnce;
    [Header("Settings")]
    [SerializeField] int _amountToDonate;
    [SerializeField] float _despawnTime;
    [SerializeField] float _cooldownTime;

    public event EventHandler OnAllAmountDonated;

    MainCharacterController _mainCharacterController;
    float _cooldown;

    private void Start()
    {
        _cooldown = _cooldownTime;
        _textMeshPro.text = _amountToDonate.ToString();

        _mainCharacterController = MainCharacterController.Instance;
    }
    private void Update()
    {
        if (_cooldown > 0)
            _cooldown -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (_cooldown >= 0) return;

        if(_mainCharacterController != null 
            && _mainCharacterController.ContainsItemInBackpack(_requiredItem))
        {
            if(_isDonateAllAtOnce)
                DonateAllAtOnce();
            else
                DonateItemByItem();

            if (_amountToDonate <= 0)
            {
                OnAllAmountDonated?.Invoke(this, EventArgs.Empty);
            }
        }

        _cooldown = _cooldownTime;
    }

    void DonateItemByItem()
    {
        var lastItem = _mainCharacterController.PopBackpackItem(_requiredItem);

        if (lastItem == null)
        {
            Debug.LogWarning("lastItem is null");
            return;
        }

        _amountToDonate--;
        _textMeshPro.text = _amountToDonate.ToString();

        lastItem.transform.parent = transform;
        lastItem.transform.DOMove(transform.position, 1f)
            .OnComplete(() =>
            {
                HandleDonateBehavior(lastItem);
            });
    }

    void DonateAllAtOnce()
    {
        List<Item> listItem = _mainCharacterController.BackPackItems(_requiredItem);

        if(listItem == null) 
        {
            Debug.LogWarning("listItem is null");
            return; 
        }

        _amountToDonate -= listItem.Count;
        _textMeshPro.text = _amountToDonate.ToString();

        foreach (var item in listItem)
        {
            item.transform.parent = transform;
            item.transform.DOMove(transform.position, 1f)
                .OnComplete(() =>
                {
                    HandleDonateBehavior(item);
                });
        }

        _mainCharacterController.ClearBackPackItems();
    }

    void HandleDonateBehavior(Item item)
    {
        switch (_donateBehavior)
        {
            case DonateBehavior.Destroy:
                Destroy(item.gameObject, _despawnTime);
                break;
            case DonateBehavior.CollectZone:
                _collectorZone.TransferItemToCollectorZone(item);
                break;
            case DonateBehavior.ItemProduction:
                Destroy(item.gameObject, _despawnTime);
                _production.ProduceItemRequest();
                break;
            default:
                Debug.LogWarning("The chosen donate behavior required object is maybe null");
                break;
        }
    }
}

[CustomEditor(typeof(ItemDonateZone))]
public class ItemDonateZoneEditor : Editor
{
    SerializedProperty _collectorZone;
    SerializedProperty _production;
    SerializedProperty _donateBehavior;
    SerializedProperty _requiredItem;
    SerializedProperty _textMeshPro;
    SerializedProperty _isDonateAllAtOnce;
    SerializedProperty _amountToDonate;
    SerializedProperty _despawnTime;
    SerializedProperty _cooldownTime;

    private void OnEnable()
    {
        // Link the SerializedProperties
        _collectorZone = serializedObject.FindProperty("_collectorZone");
        _production = serializedObject.FindProperty("_production");
        _donateBehavior = serializedObject.FindProperty("_donateBehavior");
        _requiredItem = serializedObject.FindProperty("_requiredItem");
        _textMeshPro = serializedObject.FindProperty("_textMeshPro");
        _isDonateAllAtOnce = serializedObject.FindProperty("_isDonateAllAtOnce");
        _amountToDonate = serializedObject.FindProperty("_amountToDonate");
        _despawnTime = serializedObject.FindProperty("_despawnTime");
        _cooldownTime = serializedObject.FindProperty("_cooldownTime");
    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object
        serializedObject.Update();

        // Draw the Donate Behavior enum first
        EditorGUILayout.PropertyField(_donateBehavior, new GUIContent("Donate Behavior"));

        // Draw the dynamic fields based on the selected DonateBehavior
        switch ((DonateBehavior)_donateBehavior.enumValueIndex)
        {
            case DonateBehavior.CollectZone:
                EditorGUILayout.PropertyField(_collectorZone, new GUIContent("Collector Zone"));
                if (_collectorZone.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("Collector Zone is not assigned!", MessageType.Warning);
                }
                break;

            case DonateBehavior.ItemProduction:
                EditorGUILayout.PropertyField(_production, new GUIContent("Item Production"));
                if (_production.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("Item Production is not assigned!", MessageType.Warning);
                }
                break;

            case DonateBehavior.Destroy:
                // No additional fields needed for destroy behavior
                break;
        }

        // Draw the remaining fields
        EditorGUILayout.PropertyField(_requiredItem, new GUIContent("Required Item"));
        EditorGUILayout.PropertyField(_textMeshPro, new GUIContent("Text Mesh Pro"));
        EditorGUILayout.PropertyField(_isDonateAllAtOnce, new GUIContent("Donate All At Once"));
        EditorGUILayout.PropertyField(_amountToDonate, new GUIContent("Amount To Donate"));
        EditorGUILayout.PropertyField(_despawnTime, new GUIContent("Despawn Time"));
        EditorGUILayout.PropertyField(_cooldownTime, new GUIContent("Cooldown Time"));

        // Apply property modifications
        serializedObject.ApplyModifiedProperties();
    }
}
