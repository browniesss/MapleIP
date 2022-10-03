using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Base : MonoBehaviour
{
    // 인벤토리 스크립트 - 직접 인벤토리에 들어갈 스크립트
    Player player; 
    ItemManager itemManager;
    public GameObject inventory_base = null; 
    [SerializeField]
    public Slot[] slots;  // 슬롯들 배열

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        itemManager = FindObjectOfType<ItemManager>();
    }

    void LoadInventory() // 씬이 바뀌었을 때 인벤토리를 리로드 하는 함수.
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (player.haveItem[i] != 0) // 플레이어가 가지고 있는 아이템이 있다면.
            {
                slots[i].AddItem(itemManager.itemList[player.haveItem[i] - 1]);
            }
        }
    }

    void Start()
    {
        slots = GetComponentsInChildren<Slot>();

        LoadInventory();

        gameObject.SetActive(false);
    }


    void UpdateInventoryPos() // 인벤토리 좌표를 업데이트 해주는 함수.
    {
        transform.position = new Vector3(inventory_base.transform.position.x,
            inventory_base.transform.position.y - 187.3f, 0);
    }

    void Update()
    {
        UpdateInventoryPos();
    }

    public void AcquireItem(Item _item, int _count = 1) // 아이템 획득 함수.
    {
        if (Item.ItemType.Equipment != _item.itemType) // 획득한 아이템 Type 이 장비가 아니라면 
        {
            for (int i = 0; i < slots.Length; i++) // 인벤토리 내 아이템 검사. 
            {
                if (slots[i].item != null)  // null 이라면 slots[i].item.itemName 할 때 런타임 에러 나서
                {
                    if (slots[i].item.itemName == _item.itemName) // 획득한 아이템과 같은 아이템이 있다면 
                    {
                        slots[i].SetSlotCount(_count); // 해당 아이템 슬롯에 개수를 추가해줌.
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++) // 만약 위 과정이 없었다면 
        {
            if (slots[i].item == null) // 빈 슬롯을 찾아서
            {
                slots[i].AddItem(_item, _count); // 아이템을 추가해줌.
                slots[i].slotIndex = i;
                player.haveItem[i] = _item.itemCode; // 플레이어가 해당 아이템을 가지고 있다는 것을 저장.
                return;
            }
        }
    }
}
