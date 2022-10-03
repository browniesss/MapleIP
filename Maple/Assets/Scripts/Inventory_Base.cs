using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Base : MonoBehaviour
{
    // �κ��丮 ��ũ��Ʈ - ���� �κ��丮�� �� ��ũ��Ʈ
    Player player; 
    ItemManager itemManager;
    public GameObject inventory_base = null; 
    [SerializeField]
    public Slot[] slots;  // ���Ե� �迭

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        itemManager = FindObjectOfType<ItemManager>();
    }

    void LoadInventory() // ���� �ٲ���� �� �κ��丮�� ���ε� �ϴ� �Լ�.
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (player.haveItem[i] != 0) // �÷��̾ ������ �ִ� �������� �ִٸ�.
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


    void UpdateInventoryPos() // �κ��丮 ��ǥ�� ������Ʈ ���ִ� �Լ�.
    {
        transform.position = new Vector3(inventory_base.transform.position.x,
            inventory_base.transform.position.y - 187.3f, 0);
    }

    void Update()
    {
        UpdateInventoryPos();
    }

    public void AcquireItem(Item _item, int _count = 1) // ������ ȹ�� �Լ�.
    {
        if (Item.ItemType.Equipment != _item.itemType) // ȹ���� ������ Type �� ��� �ƴ϶�� 
        {
            for (int i = 0; i < slots.Length; i++) // �κ��丮 �� ������ �˻�. 
            {
                if (slots[i].item != null)  // null �̶�� slots[i].item.itemName �� �� ��Ÿ�� ���� ����
                {
                    if (slots[i].item.itemName == _item.itemName) // ȹ���� �����۰� ���� �������� �ִٸ� 
                    {
                        slots[i].SetSlotCount(_count); // �ش� ������ ���Կ� ������ �߰�����.
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++) // ���� �� ������ �����ٸ� 
        {
            if (slots[i].item == null) // �� ������ ã�Ƽ�
            {
                slots[i].AddItem(_item, _count); // �������� �߰�����.
                slots[i].slotIndex = i;
                player.haveItem[i] = _item.itemCode; // �÷��̾ �ش� �������� ������ �ִٴ� ���� ����.
                return;
            }
        }
    }
}
