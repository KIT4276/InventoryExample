using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AmmoRewardConfig", fileName = "AmmoRewardConfig")]
public class AmmoRewardConfig : ScriptableObject
{
    [SerializeField, Min(1)] private int _minAddAmount = 10;
    [SerializeField, Min(1)] private int _maxAddAmount = 30;
    [SerializeField] private AmmoDefinition[] _availableAmmoDefinitions;

    public int MinAddAmount => Mathf.Max(1, _minAddAmount);
    public int MaxAddAmount => Mathf.Max(MinAddAmount, _maxAddAmount);
    public AmmoDefinition[] AvailableAmmoDefinitions => _availableAmmoDefinitions;
}
