using System.Collections.Generic;
using UnityEngine;
public class Utils : MonoBehaviour
{
    //===================== ������� ��� ������ � ����������� =====================\\
    // ���������� ������ ���� ���������� � ������ ������� �������
    // � ��� �������� ��������
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