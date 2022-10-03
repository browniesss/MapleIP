using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // ������ ��ũ��Ʈ.
    public enum ItemType  // ������ ����
    {
        Equipment,
        Used,
        ETC,
    }

    public int itemCode; // �������� �ڵ�
    public string itemName; // �������� �̸�
    public ItemType itemType; // ������ ����

    public string weaponType;  // ���� ����
    public Sprite itemInfo; // ������ ���� �̹���

    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 2.5f,ForceMode2D.Impulse); // �������� ���涧 ���� �ö�Դ� �������� ȿ���� �ֱ����� AddForce
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Floor") // �ٴڰ� �浹�ߴٸ�
        {
            rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
        }
    }
}
