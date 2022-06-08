using System.Collections.Generic;
using UnityEngine;
public class Utils : MonoBehaviour
{
    //===================== Функции для работы с материалами =====================\\
    // Возвращает список всех материалов в данном игровом объекте
    // и его дочерних объектах
    static public Material[] GetAllMaterials(GameObject GameObj)
    {
        Renderer[] renderers = GameObj.GetComponentsInChildren<Renderer>();

        List<Material> materials = new List<Material>();
        foreach (Renderer renderer in renderers)
        {
            materials.Add(renderer.material);
        }
        return materials.ToArray();
    }
}