using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public Item item = null; // ȹ���� ������
    public int itemCount; // ȹ���� �������� ����
    public Image itemImage;  // �������� �̹���
    public int slotIndex; // �ش� ������ ��ȣ
    public Image itemInfo_Image; //������ ���� �̹���

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

    int mouse_count = 0; // ���콺 ����Ŭ�� ī��Ʈ
    public void OnPointerClick(PointerEventData eventData) // ���콺 Ŭ�� �̺�Ʈ �߻��� 
    {
        if (eventData.button == PointerEventData.InputButton.Left) // ���� ���콺 ��ư�� Ŭ���ߴٸ� 
        {
            mouse_count++; // ���콺 Ŭ�� ī��Ʈ�� �÷��ְ� 
            StartCoroutine(Mouse_DblCheck()); // ���ʾȿ� �ٽ� Ŭ������ ���� �� ���콺 Ŭ���� 0���� �����ֱ� ���� �ڷ�ƾ ���� ( ����Ŭ�� üũ )
            if (mouse_count == 2) // ����Ŭ�� �ߴٸ�
            {
                if (item != null)
                {
                    if (item.itemType == Item.ItemType.Equipment) // ��� ������ �̶�� 
                    {
                        // ����

                    }
                    else
                    {
                        // �Һ�
                        player.audioSource.PlayOneShot(player.soundManager.audioList[7]); // ������ ��� ����
                        Debug.Log(item.itemName + " �� ����߽��ϴ�.");
                        SetSlotCount(-1); // �ش� ������ ������ ���� ����.
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData) // ���콺 �����Ͱ� ���Դٸ� 
    {
        if (item != null) // ���Կ� �������� �ִٸ�
        {
            itemInfo_Image.sprite = item.itemInfo; // ������ ������ ������.
            itemInfo_Image.color = new Color(1, 1, 1, 1);
        }
    }

    public void OnPointerExit(PointerEventData eventData) // �����Ͱ� �����ٸ�
    {
        itemInfo_Image.color = new Color(1, 1, 1, 0); // ������ ���� �̹����� ������ 0���� �ؼ� ȭ�鿡 ������ �ʰ� �� .
    }

    IEnumerator Mouse_DblCheck() // ���콺 ����Ŭ�� Ÿ�̸� 
    {
        yield return new WaitForSeconds(0.5f); // 0.5�� �� ���콺 Ŭ�� ī��Ʈ �ʱ�ȭ. 

        mouse_count = 0;
    }

    // ���콺 �巡�װ� ���� ���� �� �߻��ϴ� �̺�Ʈ
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null) // �������� �ش� ���Կ� �ִٸ� 
        {
            DragSlot.instance.dragSlot = this;  // DragSlot ��ũ��Ʈ�� ���Կ� �ش� ������ �־���
            DragSlot.instance.DragSetImage(itemImage); // �̹����� �ش� �̹����� ���� �� 
            DragSlot.instance.transform.position = eventData.position; // ��ǥ�� ����
        }
    }

    // ���콺 �巡�� ���� �� ��� �߻��ϴ� �̺�Ʈ
    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            DragSlot.instance.transform.position = eventData.position; // ��ǥ�� ���콺 �̺�Ʈ ��ǥ�� �޾ƿ�.
    }

    // ���콺 �巡�װ� ������ �� �߻��ϴ� �̺�Ʈ
    public void OnEndDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            if (!eventData.pointerCurrentRaycast.gameObject) // �κ��丮 �ٱ����� ������ ���.
            {
                DropItem(); // �������� ����߸��� �Լ� ȣ�� 
                DragSlot.instance.dragSlot = null; // dragSlot �� null �� ����� �� 
                return; // return 
            } 

            if (DragSlot.instance.dragSlot == this) // ���� ���Կ� ������ ���.
            {
                DragSlot.instance.SetColor(0); // ������ ������ 0���� �� ȭ�鿡 ������ �ʰ� �� �� 
                DragSlot.instance.dragSlot = null; // dragSlot �� null �� ����� ��
                itemImage.color = new Color(255, 255, 255, 1);  // ������ �̹����� ������ ���� ������� ���� �� 
                return; // return 
            }

            if (DragSlot.instance.dragSlot != null) // ���� �巡�װ� �����µ� dragSlot �� null �� �ƴ϶�� - ���������� �巡�װ� ������� �ʾҴٸ� 
            {
                DragSlot.instance.SetColor(0); // ������ 0������ ȭ�鿡 ������ �ʰ� ���� �� 
                DragSlot.instance.dragSlot = null; // dragSlot �� null �� �����.
            }
        }
    }

    // �ش� ���Կ� ���𰡰� ���콺 ��� ���� �� �߻��ϴ� �̺�Ʈ
    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null) // �������� �κ��丮 ������ �ִٸ� 
        {
            Slot tempSlot = DragSlot.instance.dragSlot; // tempSlot �̶�� Slot ������ ���� ���� �巡�� ���� ������ �־���. 

            if (tempSlot == this) // ���� ���Կ� ������ ���.
            {
                DragSlot.instance.SetColor(0); // �巡�� ���� ������ ������ 0���� ����� �� .
                DragSlot.instance.dragSlot = null; // dragSlot �� null�� ��  �����.
                itemImage.color = new Color(255, 255, 255, 1); // ������ �̹����� ������ ������� ���� �� 
                return; // return 
            }
            else // ���� ������ �ƴ϶�� 
            {
                ChangeSlot(); // ������ �ٲ��� �� 
                DragSlot.instance.SetColor(0); // �巡�� ������ ������ 0���� ����.
            }
        }
    }

    private void ChangeSlot() // ������ �ٲ��ִ� �Լ�. 
    {
        Item _tempItem = item; // �����۰� 
        int _tempItemCount = itemCount; // ������ ���� ��� �����صα� ���� ������.

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount); // �巡�� ���� ������ �����۰� ������ ������ �޾ƿͼ�
        // �ش� ���Կ� �־���.

        if (_tempItem != null) // �ش� ������ �� ������ �ƴϾ��ٸ� 
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount); // �巡�� ���� ���Կ� �־���.
        else // ���� �ش� ������ �� �����̾��ٸ� 
            DragSlot.instance.dragSlot.ClearSlot(); // �巡�� ���̾��� ������ �� �������� �������. 

        DragSlot.instance.dragSlot = null; // �׸��� dragSlot �� null�� �� �����.
    }

    // ������ �̹����� ���� ����
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // �κ��丮�� ���ο� ������ ���� �߰�
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

    // �ش� ������ ������ ���� ������Ʈ
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // �ش� ���� �ϳ� ����
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        player.haveItem[slotIndex] = 0;

        text_Count.text = "0";
    }

    private void DropItem() // �������� ����߸��� �Լ� . 
    {
        player.audioSource.PlayOneShot(player.soundManager.audioList[10]); // ������ ����߸��� ���� ���.
        GameObject dropItem = GameObject.Instantiate(itemManager.itemList[item.itemCode - 1].gameObject,
            player.transform.position, Quaternion.identity); // �ش� ���Կ��� �������� �������� �÷��̾� ��ġ���� ����߸��� ȿ���� �Բ� ��������.

        dropItem.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 1f, ForceMode2D.Impulse); 

        DragSlot.instance.SetColor(0); // �� ������ ������ 0���� ���� �� 

        ClearSlot(); // ������ �����.
    }
}
