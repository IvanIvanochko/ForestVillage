using System.Linq;
using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.ComponentModel;

public class Item : MonoBehaviour
{
    public Quaternion rotation; // spawning preferences
    public float heightGap = 1; // for stacking this item with others items that are these

    public Quaternion Rotation()
    {
        return Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }

    public GameObject Instantiate(Transform parent)
    {
        GameObject newItem = Instantiate(this.gameObject, parent);

        newItem.transform.localRotation = Rotation();

        return newItem;
    }

    public GameObject Instantiate()
    {
        GameObject newItem = Instantiate(this.gameObject);

        newItem.transform.localRotation = Rotation();

        return newItem;
    }

    // https://www.reddit.com/r/Unity3D/comments/fdc2on/easily_generate_unique_ids_for_your_game_objects/
    #region Unique ID
    [Serializable]
    private struct UniqueID
    {
        public string Value;
    }

    [SerializeField]
    private UniqueID _id;

    public string ID
    {
        get { return _id.Value; }
    }

    [ContextMenu("Force reset ID")]
    private void ResetId()
    {
        _id.Value = Guid.NewGuid().ToString();
        Debug.Log("Setting new ID on object: " + gameObject.name, gameObject);
    }

    //Need to check for duplicates when copying a gameobject/component
    public static bool IsUnique(string ID)
    {
        return Resources.FindObjectsOfTypeAll<Item>().Count(x => x.ID == ID) == 1;
    }

#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(UniqueID))]
    private class UniqueIdDrawer : UnityEditor.PropertyDrawer
    {
        private const float buttonWidth = 120;
        private const float padding = 2;

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            UnityEditor.EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = UnityEditor.EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            GUI.enabled = false;
            Rect valueRect = position;
            valueRect.width -= buttonWidth + padding;
            UnityEditor.SerializedProperty idProperty = property.FindPropertyRelative("Value");
            UnityEditor.EditorGUI.PropertyField(valueRect, idProperty, GUIContent.none);

            GUI.enabled = true;

            Rect buttonRect = position;
            buttonRect.x += position.width - buttonWidth;
            buttonRect.width = buttonWidth;
            if (GUI.Button(buttonRect, "Copy to clipboard"))
            {
                UnityEditor.EditorGUIUtility.systemCopyBuffer = idProperty.stringValue;
            }

            UnityEditor.EditorGUI.EndProperty();
        }
    }
#endif
    #endregion
}

