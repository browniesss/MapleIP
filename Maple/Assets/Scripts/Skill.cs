using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill : MonoBehaviour
{
    // 스킬 스크립트 
    Player player;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] Animator anim;

    public bool usable_Skill = true; // true면 쿨타임이 다 된 것. 

    public int skill_Code; // 해당 스킬 코드를 저장.

    public Sprite skill_Image; // 스킬 이미지
    public Sprite skill_Info; // 스킬 정보 이미지 

    public float CoolTime; // 스킬 쿨타임.

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        // 해당 컴포넌트가 있는 스킬들만 저장.
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

    IEnumerator Snail_Time() // 달팽이 스킬 코루틴.
    {
        yield return new WaitForSeconds(0.5f); 

        player.isAttack = false;
        player.Player_Anim.SetBool("isAttack", false);

        GameObject.Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Monster" && skill_Code == 1) // 달팽이 세마리 스킬일때
        {
            collision.GetComponent<Monster>().isAttacked(40); // 40의 데미지를 줌.

            anim.SetBool("isHit", true);

            rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
            rigid.constraints = RigidbodyConstraints2D.FreezePositionY;

            StartCoroutine(Snail_Time()); // 코루틴 시작.
        }
    }

    public void Act_Skill(int dir) // 방향에 따라 스킬 사용.
    {
        switch (skill_Code)
        {
            case 1: // 1번 스킬 달팽이 세마리
                rigid.AddForce(Vector2.one * dir * 7f, ForceMode2D.Impulse); // 방향에 따라 Addforce로 해당 스킬을 발사함.

                player.isAttack = true;

                player.Player_Anim.SetBool("isAttack", true); // 플레이어의 애니메이션 활성화

                StartCoroutine(player.AttackTime()); // 플레이어의 공격 코루틴 시작.

                GameObject.Destroy(this.gameObject, 1.5f); // 1.5초 후 해당 오브젝트 삭제.
                return;
            case 2: // 2번 스킬 민첩한 몸놀림
                player.moveSpeed += 1.5f; // 플레이어의 이동 속도를 늘려줌

                GameObject.Destroy(this.gameObject, 1f); // 1초 후 해당 오브젝트를 삭제
                return;
            case 3: // 3번 스킬 회복 
                if (player.cur_hp < player.hp[player.level] - 50) // 최대 체력 보다 50이상 적을 경우
                {
                    player.cur_hp += 50;
                }
                else // 50 미만 적을 경우엔 최대 체력으로 바꿔 줌.
                {
                    player.cur_hp = player.hp[player.level]; 
                }

                player.hp_bar.fillAmount = (float)player.cur_hp / (float)player.hp[player.level]; // 체력 바
                GameObject.Destroy(this.gameObject, 1f); // 1초 후 해당 오브젝트를 삭제
                return;
        }
    }

    void Update()
    {
    }
}
