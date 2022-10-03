using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    // ��ų ��ũ��Ʈ 
    Player player;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] Animator anim;

    public bool usable_Skill = true; // true�� ��Ÿ���� �� �� ��. 

    public int skill_Code; // �ش� ��ų �ڵ带 ����.

    public Sprite skill_Image; // ��ų �̹���
    public Sprite skill_Info; // ��ų ���� �̹��� 

    public float CoolTime; // ��ų ��Ÿ��.

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        // �ش� ������Ʈ�� �ִ� ��ų�鸸 ����.
        if (GetComponent<Rigidbody2D>() != null)
            rigid = GetComponent<Rigidbody2D>();
        if (GetComponent<BoxCollider2D>() != null)
            boxCollider = GetComponent<BoxCollider2D>();
        if (GetComponent<Animator>() != null)
            anim = GetComponent<Animator>();
    }

    void Start()
    {
        usable_Skill = true;
    }

    IEnumerator Snail_Time() // ������ ��ų �ڷ�ƾ.
    {
        yield return new WaitForSeconds(0.5f); 

        player.isAttack = false;
        player.Player_Anim.SetBool("isAttack", false);

        GameObject.Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Monster" && skill_Code == 1) // ������ ������ ��ų�϶�
        {
            collision.GetComponent<Monster>().isAttacked(40); // 40�� �������� ��.

            anim.SetBool("isHit", true);

            rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
            rigid.constraints = RigidbodyConstraints2D.FreezePositionY;

            StartCoroutine(Snail_Time()); // �ڷ�ƾ ����.
        }
    }

    public void Act_Skill(int dir) // ���⿡ ���� ��ų ���.
    {
        switch (skill_Code)
        {
            case 1: // 1�� ��ų ������ ������
                rigid.AddForce(Vector2.one * dir * 7f, ForceMode2D.Impulse); // ���⿡ ���� Addforce�� �ش� ��ų�� �߻���.

                player.isAttack = true;

                player.Player_Anim.SetBool("isAttack", true); // �÷��̾��� �ִϸ��̼� Ȱ��ȭ

                StartCoroutine(player.AttackTime()); // �÷��̾��� ���� �ڷ�ƾ ����.

                GameObject.Destroy(this.gameObject, 1.5f); // 1.5�� �� �ش� ������Ʈ ����.
                return;
            case 2: // 2�� ��ų ��ø�� ���
                player.moveSpeed += 1.5f; // �÷��̾��� �̵� �ӵ��� �÷���

                GameObject.Destroy(this.gameObject, 1f); // 1�� �� �ش� ������Ʈ�� ����
                return;
            case 3: // 3�� ��ų ȸ�� 
                if (player.cur_hp < player.hp[player.level] - 50) // �ִ� ü�� ���� 50�̻� ���� ���
                {
                    player.cur_hp += 50;
                }
                else // 50 �̸� ���� ��쿣 �ִ� ü������ �ٲ� ��.
                {
                    player.cur_hp = player.hp[player.level]; 
                }

                player.hp_bar.fillAmount = (float)player.cur_hp / (float)player.hp[player.level]; // ü�� ��
                GameObject.Destroy(this.gameObject, 1f); // 1�� �� �ش� ������Ʈ�� ����
                return;
        }
    }

    void Update()
    {
    }
}
