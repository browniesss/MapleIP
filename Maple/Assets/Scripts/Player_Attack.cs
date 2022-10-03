using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    // �÷��̾� ���� ��ũ��Ʈ
    public enum AttackType  // ���� ����
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
        // �⺻�������̰�, �浹ü�� �����̰�, �������̶��
        {
            Monster monster = collision.gameObject.GetComponent<Monster>(); // �浹ü�� Monster ��ũ��Ʈ�� �޾ƿͼ�

            monster.isAttacked(player.damage[player.level]); // �ǰ� �Լ��� ȣ����.
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Attack_type == AttackType.Attack && collision.gameObject.tag == "Monster" && player.isAttack)
        // �⺻�������̰�, �浹ü�� �����̰�, �������̶��
        {
            Monster monster = collision.GetComponent<Monster>(); // �浹ü�� Monster ��ũ��Ʈ�� �޾ƿͼ� 

            monster.isAttacked(player.damage[player.level]); // �ǰ� �Լ��� ȣ����.
        }
    }

    void AttackCheck() // �÷��̾ �������϶��� �ش� ������Ʈ�� BoxCollider2D �� Ȱ��ȭ ��Ű�� ���� �Լ�
    {
        if (player.isAttack) // �÷��̾ �������̶�� 
        {
            transform.position = player.transform.position; // ��ġ�� ���� ��
            box_collider.enabled = true; // Ȱ��ȭ
        }
        else // �ƴ϶�� 
        {
            box_collider.enabled = false; // ��Ȱ��ȭ
        }
    }

    void Update()
    {
        AttackCheck();
    }
}
