using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // 아이템 스크립트.
    public enum ItemType  // 아이템 유형
    {
        Equipment,
        Used,
        ETC,
    }

    public int itemCode; // 아이템의 코드
    public string itemName; // 아이템의 이름
    public ItemType itemType; // 아이템 유형

    public string weaponType;  // 무기 유형
    public Sprite itemInfo; // 아이템 정보 이미지

    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 2.5f,ForceMode2D.Impulse); // 아이템이 생길때 위로 올라왔다 떨어지는 효과를 주기위해 AddForce
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Floor") // 바닥과 충돌했다면
        {
            rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
        }
    }
}
