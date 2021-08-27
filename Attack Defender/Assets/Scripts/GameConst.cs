using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AssetPath
{
    DefenseStructure,
    DefenseUnit,
    AttackUnit,
    Projectile
}

public class GameConst
{
    // Map X, Z Grid size (how many cells?)
    public static int mapGridSizeX = 10;
    public static int mapGridSizeZ = 6;

    // X, Z Grid Cell size and Y for height
    public static Vector3 mapCellSize = new Vector3(2f, .5f, 2f);

    static string[] assetPaths =
    {
        "Defense Units",    // Defense Structures
        "Defense Units",    // Defense Units
        "Attack Units",     // Attack Units
        "Projectiles",      // Projectiles
    };

    public static GameObject LoadPrefab(AssetPath assetPath, string prefabName)
    {
        string fullPath = $"Assets/{assetPaths[(int)assetPath]}/{prefabName}.prefab";
        return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
    }

    public static float getRange(Vector3 unit, Vector3 target)
    {
        return Vector2.Distance(new Vector2(unit.x, unit.z), new Vector2(target.x, target.z));
    }

    public static bool isInRange(Vector3 unit, Vector3 target, float range)
    {
        return Vector2.Distance(new Vector2(unit.x, unit.z), new Vector2(target.x, target.z)) <= range;
    }

}
