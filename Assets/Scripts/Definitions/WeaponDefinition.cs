using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Items/Weapon", fileName = "WeaponDefinition")]
public class WeaponDefinition : ItemDefinition
{
    [Header("Weapon")]
    [SerializeField] private AmmoDefinition _ammoDefinition;
    [SerializeField, Min(0)] private int _damage;

    public AmmoDefinition AmmoDefinition => _ammoDefinition;
    public int Damage => _damage;

    public bool CanUseAmmo(AmmoDefinition ammoDefinition)
    {
        if (ammoDefinition == null || _ammoDefinition == null)
            return false;

        return ammoDefinition.ID == _ammoDefinition.ID;
    }

    public bool CanUseAmmo(int ammoItemId)
    {
        if (_ammoDefinition == null)
            return false;

        return _ammoDefinition.ID == ammoItemId;
    }
}
