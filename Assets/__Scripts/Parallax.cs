using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Header("Set in Inspector")]
    public GameObject poi;
    public GameObject[] panels;
    public float scrollSpeed = -30f;
    public float motionMult = 0.25f; // motionMult ���������� ������� ������� ������� �� ����������� ������� ������

    [Header("Set Dynamically")]
    private float panelHeight; // ������ ������ ������
    private float depth; // ������� ������� (�� ���� pos.z)
    float posY, posX = 0;
    private void Start()
    {
        panelHeight = panels[0].transform.localScale.y;
        depth = panels[0].transform.position.z;

        // ���������� ������ � ��������� �������
        panels[0].transform.position = new Vector3(0, 0, depth);
        panels[1].transform.position = new Vector3(0, panelHeight, depth);
    }
    private void Update()
    {
        posY = Time.time * scrollSpeed % panelHeight + (panelHeight * 0.5f);
        if (poi != null)
        {
            posX = -poi.transform.position.x * motionMult;
        }

        // �������� ������ panels[0]
        panels[0].transform.position = new Vector3(posX, posY, depth);
        // �������� ������ panels[1], ����� ������� ������ �������������
        // ��������� ����
        if (posY >= 0)
        {
            panels[1].transform.position = new Vector3(posX, posY-panelHeight, depth);
        }
        else
        {
            panels[1].transform.position = new Vector3(posX, posY+panelHeight, depth);
        }
    }
}