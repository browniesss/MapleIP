using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    static public DragSlot instance; // �̱���
    public Slot dragSlot; // �巡�� �ϴ� ������ �޾ƿ�.

    [SerializeField]
    public Image imageItem; // �巡���ϴ� ������ ������ �̹���.

    Image MyImage;

    void Start()
    {
        instance = this; // �̱��� 
        MyImage = GetComponent<Image>();
    }

    public void DragSetImage(Image _itemImage) // �巡�� �ϴ� ������ �̹����� �ٲ��ֱ� ���� �Լ�.
    {
        imageItem = _itemImage;
        MyImage.sprite = imageItem.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha) // �ش� ������ Color ���� �ٲ��ֱ� ���� �Լ�.
    {
        Color color = new Color(255,255, 255, _alpha);
        Color originColor = new Color(255, 255, 255, 0);

        imageItem.color = originColor;

        MyImage.color = color;
    }
}