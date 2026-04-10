using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Items/Ammo", fileName = "AmmoDefinition")]
public class AmmoDefinition : ItemDefinition
{
    public bool IsCompatibleWith(WeaponDefinition weaponDefinition)
    {
        if (weaponDefinition == null)
            return false;

        return weaponDefinition.CanUseAmmo(this);
    }
}
