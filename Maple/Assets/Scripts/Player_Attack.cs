using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    // 플레이어 공격 스크립트
    public enum AttackType  // 공격 유형
    {
        Attack,
        Skill,
        ETC,
    }

    Player player;
    BoxCollider2D box_collider = null;

    public AttackType Attack_type;

    void Start()
    {
        player = FindObjectOfType<Player>();
        box_collider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Attack_type == AttackType.Attack && collision.gameObject.tag == "Monster" && player.isAttack)
        // 기본공격형이고, 충돌체가 몬스터이고, 공격중이라면
        {
            Monster monster = collision.gameObject.GetComponent<Monster>(); // 충돌체의 Monster 스크립트를 받아와서

            monster.isAttacked(player.damage[player.level]); // 피격 함수를 호출함.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Attack_type == AttackType.Attack && collision.gameObject.tag == "Monster" && player.isAttack)
        // 기본공격형이고, 충돌체가 몬스터이고, 공격중이라면
        {
            Monster monster = collision.GetComponent<Monster>(); // 충돌체의 Monster 스크립트를 받아와서 

            monster.isAttacked(player.damage[player.level]); // 피격 함수를 호출함.
        }
    }

    void AttackCheck() // 플레이어가 공격중일때만 해당 오브젝트의 BoxCollider2D 를 활성화 시키기 위한 함수
    {
        if (player.isAttack) // 플레이어가 공격중이라면 
        {
            transform.position = player.transform.position; // 위치를 지정 후
            box_collider.enabled = true; // 활성화
        }
        else // 아니라면 
        {
            box_collider.enabled = false; // 비활성화
        }
    }

    void Update()
    {
        AttackCheck();
    }
}
