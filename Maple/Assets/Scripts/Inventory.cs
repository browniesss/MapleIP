using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    // 인벤토리 스크립트. ( 인벤토리와 퀘스트 UI 스크립트 )

    public GameObject Inventory_UI = null; // 인벤토리 창 UI
    public GameObject QuestList_UI = null; // 퀘스트 창 UI
    // 인벤토리, 퀘스트 창의 Open 여부
    [SerializeField]
    private bool isInventory_Open = false;
    [SerializeField]
    private bool isQuestList_Open = false;


    void Start()
    {
    }


    public RectTransform window; // Drag Move Window
    private Vector2 downPosition;

    public void OnPointerDown(PointerEventData data) // 마우스가 눌렸을 때
    {
        downPosition = data.position; // 마우스의 위치로 downPosition을 잡아줌.
    }

    public void OnDrag(PointerEventData data) // 드래그가 시작했을 때
    {
        Vector2 offset = data.position - downPosition; // offset을 잡아줌.
        downPosition = data.position;

        window.anchoredPosition += offset;  // 좌표 변경.
    }

    void toggleInventory() // 인벤토리를 열고 닫는 함수.
    {
        if (Input.GetKeyDown(KeyCode.I)) // I키를 눌렀다면 
        {
            isInventory_Open = !isInventory_Open; // 현재 상태의 반대 상태를 넣어준 후 
        }

        Inventory_UI.SetActive(isInventory_Open); // 그 상태에 맞게 활성화 혹은 비활성화
    }

    void toggleQuestList() // 퀘스트 창을 열고 닫는 함수.
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Q키를 눌렀다면 
        {
            isQuestList_Open = !isQuestList_Open; // 현재 상태의 반대 상태를 넣어준 후 
        }

        QuestList_UI.SetActive(isQuestList_Open); // 그 상태에 맞게 활성화 혹은 비활성화
    }

    void Update()
    {
        toggleInventory();
        toggleQuestList();
    }
}
