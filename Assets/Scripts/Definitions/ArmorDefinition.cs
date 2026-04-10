using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Items/Armor", fileName = "ArmorDefinition")]
public class ArmorDefinition : ItemDefinition
{
    [Header("Armor")]
    [SerializeField, Min(0)] private int _protection;
    [SerializeField] private ArmorType _type;

    public int Protection => _protection;
    public ArmorType Type => _type;
}
