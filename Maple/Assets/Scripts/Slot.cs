using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item = null; // 획득한 아이템
    public int itemCount; // 획득한 아이템의 개수
    public Image itemImage;  // 아이템의 이미지
    public int slotIndex; // 해당 슬롯의 번호
    public Image itemInfo_Image; //아이템 정보 이미지

    [SerializeField]
    private ItemManager itemManager = null;
    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private Inventory_Base inventory = null;
    [SerializeField]
    private Player player = null;

    void Awake()
    {
        player = FindObjectOfType<Player>();
        itemManager = FindObjectOfType<ItemManager>();
        inventory = GetComponentInParent<Inventory_Base>();
    }

    int mouse_count = 0; // 마우스 더블클릭 카운트
    public void OnPointerClick(PointerEventData eventData) // 마우스 클릭 이벤트 발생시 
    {
        if (eventData.button == PointerEventData.InputButton.Left) // 왼쪽 마우스 버튼을 클릭했다면 
        {
            mouse_count++; // 마우스 클릭 카운트를 늘려주고 
            StartCoroutine(Mouse_DblCheck()); // 몇초안에 다시 클릭하지 않을 시 마우스 클릭을 0으로 돌려주기 위한 코루틴 시작 ( 더블클릭 체크 )
            if (mouse_count == 2) // 더블클릭 했다면
            {
                if (item != null)
                {
                    if (item.itemType == Item.ItemType.Equipment) // 장비 아이템 이라면 
                    {
                        // 장착

                    }
                    else
                    {
                        // 소비
                        player.audioSource.PlayOneShot(player.soundManager.audioList[7]); // 아이템 사용 사운드
                        Debug.Log(item.itemName + " 을 사용했습니다.");
                        SetSlotCount(-1); // 해당 슬롯의 아이템 수를 깎음.
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) // 마우스 포인터가 들어왔다면 
    {
        if (item != null) // 슬롯에 아이템이 있다면
        {
            itemInfo_Image.sprite = item.itemInfo; // 아이템 정보를 보여줌.
            itemInfo_Image.color = new Color(1, 1, 1, 1);
        }
    }

    public void OnPointerExit(PointerEventData eventData) // 포인터가 나갔다면
    {
        itemInfo_Image.color = new Color(1, 1, 1, 0); // 아이템 정보 이미지의 투명도를 0으로 해서 화면에 보이지 않게 함 .
    }

    IEnumerator Mouse_DblCheck() // 마우스 더블클릭 타이머 
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 후 마우스 클릭 카운트 초기화. 

        mouse_count = 0;
    }

    // 마우스 드래그가 시작 됐을 때 발생하는 이벤트
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null) // 아이템이 해당 슬롯에 있다면 
        {
            DragSlot.instance.dragSlot = this;  // DragSlot 스크립트의 슬롯에 해당 슬롯을 넣어줌
            DragSlot.instance.DragSetImage(itemImage); // 이미지를 해당 이미지로 변경 후 
            DragSlot.instance.transform.position = eventData.position; // 좌표를 변경
        }
    }

    // 마우스 드래그 중일 때 계속 발생하는 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            DragSlot.instance.transform.position = eventData.position; // 좌표를 마우스 이벤트 좌표로 받아옴.
    }

    // 마우스 드래그가 끝났을 때 발생하는 이벤트
    public void OnEndDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            if (!eventData.pointerCurrentRaycast.gameObject) // 인벤토리 바깥에서 놓았을 경우.
            {
                DropItem(); // 아이템을 떨어뜨리는 함수 호출 
                DragSlot.instance.dragSlot = null; // dragSlot 을 null 로 비워준 후 
                return; // return 
            } 

            if (DragSlot.instance.dragSlot == this) // 원래 슬롯에 놓았을 경우.
            {
                DragSlot.instance.SetColor(0); // 슬롯의 투명도를 0으로 해 화면에 보이지 않게 한 후 
                DragSlot.instance.dragSlot = null; // dragSlot 을 null 로 비워준 후
                itemImage.color = new Color(255, 255, 255, 1);  // 아이템 이미지의 투명도와 색을 원래대로 해준 후 
                return; // return 
            }

            if (DragSlot.instance.dragSlot != null) // 만약 드래그가 끝났는데 dragSlot 이 null 이 아니라면 - 정상적으로 드래그가 종료되지 않았다면 
            {
                DragSlot.instance.SetColor(0); // 투명도를 0으로해 화면에 보이지 않게 해준 후 
                DragSlot.instance.dragSlot = null; // dragSlot 을 null 로 비워줌.
            }
        }
    }

    // 해당 슬롯에 무언가가 마우스 드롭 됐을 때 발생하는 이벤트
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null) // 내려놓을 인벤토리 슬롯이 있다면 
        {
            Slot tempSlot = DragSlot.instance.dragSlot; // tempSlot 이라는 Slot 변수를 만들어서 현재 드래그 중인 슬롯을 넣어줌. 

            if (tempSlot == this) // 원래 슬롯에 놓았을 경우.
            {
                DragSlot.instance.SetColor(0); // 드래그 중인 슬롯의 투명도를 0으로 만들어 줌 .
                DragSlot.instance.dragSlot = null; // dragSlot 을 null로 해  비워줌.
                itemImage.color = new Color(255, 255, 255, 1); // 아이템 이미지의 투명도를 원래대로 해준 후 
                return; // return 
            }
            else // 원래 슬롯이 아니라면 
            {
                ChangeSlot(); // 슬롯을 바꿔준 후 
                DragSlot.instance.SetColor(0); // 드래그 슬롯을 투명도로 0으로 해줌.
            }
        }
    }

    private void ChangeSlot() // 슬롯을 바꿔주는 함수. 
    {
        Item _tempItem = item; // 아이템과 
        int _tempItemCount = itemCount; // 아이템 수를 잠시 저장해두기 위한 변수들.

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount); // 드래그 중인 슬롯의 아이템과 아이템 개수를 받아와서
        // 해당 슬롯에 넣어줌.

        if (_tempItem != null) // 해당 슬롯이 빈 슬롯이 아니었다면 
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount); // 드래그 중인 슬롯에 넣어줌.
        else // 만약 해당 슬롯이 빈 슬롯이었다면 
            DragSlot.instance.dragSlot.ClearSlot(); // 드래그 중이었던 슬롯을 빈 슬롯으로 만들어줌. 

        DragSlot.instance.dragSlot = null; // 그리고 dragSlot 을 null로 해 비워줌.
    }

    // 아이템 이미지의 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.GetComponent<SpriteRenderer>().sprite;

        if (item.itemType != Item.ItemType.Equipment)
        {
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = "0";
        }

        SetColor(1);
    }

    // 해당 슬롯의 아이템 갯수 업데이트
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // 해당 슬롯 하나 삭제
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        player.haveItem[slotIndex] = 0;

        text_Count.text = "0";
    }

    private void DropItem() // 아이템을 떨어뜨리는 함수 . 
    {
        player.audioSource.PlayOneShot(player.soundManager.audioList[10]); // 아이템 떨어뜨리는 사운드 재생.
        GameObject dropItem = GameObject.Instantiate(itemManager.itemList[item.itemCode - 1].gameObject,
            player.transform.position, Quaternion.identity); // 해당 슬롯에서 보관중인 아이템을 플레이어 위치에서 떨어뜨리는 효과와 함께 생성해줌.

        dropItem.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1f, ForceMode2D.Impulse); 

        DragSlot.instance.SetColor(0); // 그 슬롯의 투명도를 0으로 해준 후 

        ClearSlot(); // 슬롯을 비워줌.
    }
}
