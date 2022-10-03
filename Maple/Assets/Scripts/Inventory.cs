using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    // �κ��丮 ��ũ��Ʈ. ( �κ��丮�� ����Ʈ UI ��ũ��Ʈ )

    public GameObject Inventory_UI = null; // �κ��丮 â UI
    public GameObject QuestList_UI = null; // ����Ʈ â UI
    // �κ��丮, ����Ʈ â�� Open ����
    [SerializeField]
    private bool isInventory_Open = false;
    [SerializeField]
    private bool isQuestList_Open = false;


    void Start()
    {
    }


    public RectTransform window; // Drag Move Window
    private Vector2 downPosition;

    public void OnPointerDown(PointerEventData data) // ���콺�� ������ ��
    {
        downPosition = data.position; // ���콺�� ��ġ�� downPosition�� �����.
    }

    public void OnDrag(PointerEventData data) // �巡�װ� �������� ��
    {
        Vector2 offset = data.position - downPosition; // offset�� �����.
        downPosition = data.position;

        window.anchoredPosition += offset;  // ��ǥ ����.
    }

    void toggleInventory() // �κ��丮�� ���� �ݴ� �Լ�.
    {
        if (Input.GetKeyDown(KeyCode.I)) // IŰ�� �����ٸ� 
        {
            isInventory_Open = !isInventory_Open; // ���� ������ �ݴ� ���¸� �־��� �� 
        }

        Inventory_UI.SetActive(isInventory_Open); // �� ���¿� �°� Ȱ��ȭ Ȥ�� ��Ȱ��ȭ
    }

    void toggleQuestList() // ����Ʈ â�� ���� �ݴ� �Լ�.
    {
        if (Input.GetKeyDown(KeyCode.Q)) // QŰ�� �����ٸ� 
        {
            isQuestList_Open = !isQuestList_Open; // ���� ������ �ݴ� ���¸� �־��� �� 
        }

        QuestList_UI.SetActive(isQuestList_Open); // �� ���¿� �°� Ȱ��ȭ Ȥ�� ��Ȱ��ȭ
    }

    void Update()
    {
        toggleInventory();
        toggleQuestList();
    }
}
