using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ItemDefinition : ScriptableObject
{
    [SerializeField, ReadOnlyInInspector] protected int _id;
    [SerializeField] protected string _displayName;
    [SerializeField] protected Sprite _icon;
    [SerializeField, Min(0f)] protected float _weight;
    [SerializeField, Min(1)] protected int _maxStack = 1;

    public int ID => _id;
    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
    public float Weight => _weight;
    public int MaxStack => Mathf.Max(1, _maxStack);

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        string assetPath = AssetDatabase.GetAssetPath(this);
        if (string.IsNullOrEmpty(assetPath))
            return;

        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        if (string.IsNullOrEmpty(guid) || guid.Length < 8)
            return;

        int generatedId = (int)(System.Convert.ToUInt32(guid.Substring(0, 8), 16) & 0x7FFFFFFF);
        if (generatedId == 0)
            generatedId = 1;

        if (_id != generatedId)
        {
            _id = generatedId;
            EditorUtility.SetDirty(this);
        }
    }
#endif
}
