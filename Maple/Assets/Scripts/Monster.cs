using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    // 몬스터 스크립트
    Animator animator;
    Player player;
    Rigidbody2D rigid;

    private string curAnim;

    [SerializeField] private int moveSpeed; // 움직이는 방향
    [SerializeField] private float mob_Speed; // 움직이는 속도

    [SerializeField] private int hp; // 최대 체력
    [SerializeField] private int cur_hp; // 현재 체력
    [SerializeField] private int give_exp; // 플레이어에게 주는 경험치

    [SerializeField] private int damage; // 데미지

    public Vector3 hpBarOffset = new Vector3(-0.5f, 2.4f, 0); // 체력바 위치 offset

    public bool isDie = false; // 죽었는지 판별할 bool 변수.

    GameObject hpBarCanvas; // 체력바 Canvas
    public GameObject hpBar_prf; // 체력바 프리팹
    public GameObject damageText_prf; // 데미지 텍스트 프리팹

    public Image real_hp_bar; // 체력바
    public Text real_damage_Text; // 데미지 텍스트

    RectTransform hpBar; // 체력바 RectTransform
    RectTransform damageText; // 데미지 텍스트 RectTransform

    public float height = 1.7f; // 높이

    void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        hpBarCanvas = FindObjectOfType<Canvas>().gameObject;
        rigid = GetComponent<Rigidbody2D>();

        hpBar = Instantiate(hpBar_prf, hpBarCanvas.transform).GetComponent<RectTransform>(); // 체력바 생성
        real_hp_bar = hpBar.transform.GetChild(0).GetComponent<Image>(); // 체력바 이미지 넣어줌.

        damageText = Instantiate(damageText_prf, hpBarCanvas.transform).GetComponent<RectTransform>(); // 체력 텍스트 
        real_damage_Text = damageText.transform.GetChild(0).GetComponent<Text>();
        damageText.gameObject.SetActive(false);

        cur_hp = hp;

        Think(); // 어디로 움직일지 생각하는 함수
    }

    void hpBar_Set() // 체력바 세팅 함수.
    {
        Vector3 HpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + height, 0)); // 스크린 좌표를 받아와서
        // 좌표를 정해준 후 
        hpBar.position = HpBarPos; // 좌표를 넣어줌.

        real_hp_bar.fillAmount = (float)cur_hp / (float)hp; // fillAmount 를 통해 체력비율대로 채워줌.
    }

    public void ReInit() // 다시 생성됐을때 초기화
    {
        cur_hp = hp;
        isDie = false;
        hpBar.gameObject.SetActive(true);
        GetComponent<BoxCollider2D>().isTrigger = false;
        rigid.constraints = RigidbodyConstraints2D.None;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    IEnumerator Rethink_Time(int time) // 다시 생각하게 해주는 코루틴.
    {
        yield return new WaitForSeconds(time);

        Think(); // 재귀 식으로 다시 생각하게 함.
    }

    IEnumerator hit_Time() // 맞았을때 코루틴.
    {
        yield return new WaitForSeconds(1f);

        animator.SetBool("isHit", false);
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.layer == 6) // 6 = Player 레이어와 충돌했을때
        {
            int hitDir = transform.position.x < collision.transform.position.x ? 1 : -1; // 왼쪽에서 충돌했는지, 오른쪽에서 충돌했는지 검사 후
            player.IsHit(damage, hitDir); // 방향에 맞게 IsHit 함수 호출.
        }
    }

    int thinkTime = 0;
    void Think() // 움직임을 결정하기 위해 생각하는 함수.
    {
        if (isDie) // 죽었다면 return
            return;

        moveSpeed = Random.Range(-1, 2); // 움직이는 방향을 랜덤으로 -1 , 0 , 1 중 하나
        thinkTime = Random.Range(3, 7); // 다음 생각을 몇 초 후에 할지도 랜덤으로.

        StartCoroutine(Rethink_Time(thinkTime)); // 다시 생각하는 코루틴 시작.

        animator.SetInteger("Moving", moveSpeed); // 움직임에 맞는 애니메이션
    }

    void Move() // 움직이는 함수
    {
        if (isDie)  // 죽었다면 return
            return;

        Vector3 tempVec = transform.position; // 현재 위치를 받아온 후 

        tempVec.x += moveSpeed * mob_Speed * Time.deltaTime; // 움직일 방향, 몬스터의 스피드에 따라 좌표를 더해줌.

        if (moveSpeed > 0) // 움직이는 방향에 따라 스프라이트 방향 변경.
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (moveSpeed < 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }

        // Ray 를 진행방향 조금 앞에 발사해 낭떠러지인지 검사.
        Vector2 frontVec = new Vector2(tempVec.x + moveSpeed * 0.2f, tempVec.y);
        Debug.DrawRay(frontVec, Vector3.down * 0.5f, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 0.5f, LayerMask.GetMask("Floor", "UnFloor"));

        if (rayHit.collider == null) // 앞이 낭떠러지 라면
        {
            moveSpeed *= -1;
        }

        transform.position = tempVec; // 변경해준 좌표를 다시 넣어줌.
    }

    IEnumerator Damage_Text_Off_Time() // 피격당할 시 나타나는 데미지 텍스트를 보여준 후 꺼주기 위한 코루틴.
    {
        yield return new WaitForSeconds(0.5f);

        damageText.gameObject.SetActive(false);
    }

    void On_DamageText(int oppos_dmg) // 피격당할 시 나타는 데미지를 보여주기 위한 함수.
    {
        // 데미지 텍스트를 띄우기 위한 좌표를 스크린에서 받아옴.
        Vector3 damagePos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x + 0.8f, transform.position.y + 0.2f, 0)); 
        damageText.position = damagePos; // 좌표 지정

        damageText.gameObject.SetActive(true); // 활성화 시켜 보여줌.

        real_damage_Text.text = oppos_dmg.ToString(); // 텍스트를 입은 데미지

        StartCoroutine(Damage_Text_Off_Time());
    }

    IEnumerator die_Time() // 죽었을때 코루틴
    {
        yield return new WaitForSeconds(1.2f);

        damageText.gameObject.SetActive(false); // 데미지 텍스트 비활성화.
        MonsterManager.ReturnObject(this); // 오브젝트 풀로 리턴
    }

    public void isAttacked(int oppos_dmg) // 데미지를 입었을 때
    {
        On_DamageText(oppos_dmg); // 데미지 텍스트를 띄워준 후

        cur_hp -= oppos_dmg; // 체력을 감소 시켜줌.

        if (cur_hp <= 0) // 체력이 0 이하가 되면.
        {
            GetComponent<BoxCollider2D>().isTrigger = true; // 플레이어와 충돌을 꺼준 후 

            rigid.constraints = RigidbodyConstraints2D.FreezePosition; // 좌표를 고정.

            isDie = true; 
            player.catchCount += 1; // 플레이어의 사냥 수 증가

            animator.SetBool("isDie", true); // 사망 애니메이션 활성화 

            player.cur_exp += give_exp; // 플레이어 경험치 증가 

            if (player.cur_exp >= player.need_exp[player.level]) // 플레이어 경험치가 현재 레벨 기준 최대 경험치 이상이라면
            {
                player.LevelUp(); // 플레이어 레벨업 함수 호출
            }

            hpBar.gameObject.SetActive(false); // 체력바 비활성화

            StartCoroutine(die_Time()); // 사망 코루틴 시작.
        }

        int forceDir = player.GetComponent<SpriteRenderer>().flipX == true ? 1 : -1; // 플레이어한테 피격당한 방향 검사 후
        rigid.AddForce(new Vector2(forceDir, 2), ForceMode2D.Impulse); // 방향에 맞는 피격 처리.

        animator.SetBool("isHit", true); // 피격 애니메이션 활성화

        StartCoroutine(hit_Time()); // 피격 코루틴 시작.
    }

    void Update()
    {
        Move();

        hpBar_Set();
    }
}
