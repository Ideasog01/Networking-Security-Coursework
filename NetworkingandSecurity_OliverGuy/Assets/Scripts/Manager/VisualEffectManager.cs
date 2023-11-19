using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualEffectManager : MonoBehaviour
{
    //Spawns a visual effect objects at the given position and rotation
    public static void SpawnVisualEffect(GameObject prefab, Vector3 position, Quaternion rotation, float duration) 
    {
        GameObject obj = Instantiate(prefab, position, rotation);
        Destroy(obj, duration);
    }
}
