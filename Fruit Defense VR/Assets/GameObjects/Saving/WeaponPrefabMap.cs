using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/WeaponPrefabMap")]
public class WeaponPrefabMap : ScriptableObject
{
    public GameObject[] weapons;

    public GameObject GetPrefabFromWeaponId(string name)
    {
        GameObject w = null;
        foreach( GameObject prefab in weapons)
        {
            var weaponScript = prefab.GetComponent<BaseWeapon>();
            if (weaponScript.mId == name)
            {
                w = prefab;
            }
        }
        return w;
    }
}
