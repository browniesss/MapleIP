using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // 플레이어 스크립트
    // 매니저들.
    public GameManager gameManager = null;
    public ItemManager itemManager = null;
    public SkillManager skillManager = null;
    public SoundManager soundManager = null;

    public AudioSource audioSource = null; // 오디오 소스 ( 소리 재생할 때 )

    public float moveSpeed = 5f; // 이동속도
    public float jumpPower = 3f; // 점프 파워

    Rigidbody2D rigid = null;

    // 현재 상태 BOOL 변수들
    protected bool isDown = false;
    protected bool isJump = false;
    protected bool isDie = false;
    public bool isAttack = false;
    public bool isLadder = false;
    protected bool isPortal = false;

    public QuickSlot quickSlot = null; // 퀵슬롯

    public Animator Player_Anim = null; // 애니메이터

    public BoxCollider2D attackCollider = null; // 공격 범위

    // scan오브젝트들
    public GameObject scanObject;
    public GameObject scanNpc;

    public Inventory_Base inventory = null; // 인벤토리

    public GameObject levelUp_Eff = null; // 레벨업 이펙트
    public GameObject die_Tomb = null; // 사망시 나오는 비석

    public Sprite die_Sprite = null; // 죽었을때 스프라이트

    // 플레이어 정보
    public int[] mp;
    public int[] hp;
    public int[] need_exp;
    public int[] damage;
    public int catchCount;

    // PlayerPrefs 로 저장할 값들 
    public int level;
    public int cur_mp;
    public int cur_hp;
    public int cur_exp;
    public int cur_MapIndex;
    public int cur_MapMoving = 0; // 1이면 이전 맵에서 온것 , 2면 다음 맵에서 온것
    public string cur_QuestName; // 현재 퀘스트 이름 
    public int cur_QuestID; // 현재 퀘스트 ID
    public int cur_QuestIndex;
    public int[] haveItem = new int[24]; // 현재 가지고 있는 아이템. 
    public int[] have_QuickSlot_Skill = new int[7];

    // HP , MP , EXP , LEVEL 등 UI
    public Image hp_bar;
    public Image mp_bar;
    public Image exp_bar;

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();

        rigid = GetComponent<Rigidbody2D>();
        Player_Anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey("MovingMap")) // 저장돼있던 값이 있다면 다시 불러옴.
        {
            cur_MapMoving = PlayerPrefs.GetInt("MovingMap");
            cur_MapIndex = PlayerPrefs.GetInt("mapIndex");
            cur_QuestID = PlayerPrefs.GetInt("QuestID");
            cur_QuestIndex = PlayerPrefs.GetInt("QuestIndex");
            cur_QuestName = PlayerPrefs.GetString("QuestName");

            level = PlayerPrefs.GetInt("PlayerLevel");
            cur_hp = PlayerPrefs.GetInt("PlayerHp");
            cur_mp = PlayerPrefs.GetInt("PlayerMp");
            cur_exp = PlayerPrefs.GetInt("CurExp");

            string[] itemDataArr = PlayerPrefs.GetString("ItemList").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의
                                                                                 // ,로 구분하여 배열에 저장

            for (int i = 0; i < itemDataArr.Length; i++)
            {
                haveItem[i] = System.Convert.ToInt32(itemDataArr[i]); // 문자열 형태로 저장된 값을
                                                                      // 정수형으로 변환후 저장
            }

            string[] quickSlotData = PlayerPrefs.GetString("QuickSlot").Split(','); // PlayerPrefs에서 불러온 값을 Split 함수를 통해 문자열의
                                                                                    // ,로 구분하여 배열에 저장

            for (int i = 0; i < quickSlotData.Length; i++)
            {
                have_QuickSlot_Skill[i] = System.Convert.ToInt32(quickSlotData[i]); // 문자열 형태로 저장된 값을
                                                                                    // 정수형으로 변환후 저장
            }
        }
        else // 저장된 값이 없을 경우
        {
            cur_QuestID = 10;

            level = 0;
            cur_exp = 0;
            cur_hp = hp[0];
            cur_mp = mp[0];
        }
    }

    void Start()
    {
        gameManager.level_text.text = "Lv     " + (level + 1); // 레벨 텍스트
        hp_bar.fillAmount = (float)cur_hp / (float)hp[level]; // 체력 바 
        exp_bar.fillAmount = (float)cur_exp / (float)need_exp[level]; // 경험치 바
    }

    public void sceneSave() // 다음 맵으로 넘어갈 때 정보를 저장해 줄 함수.
    {
        PlayerPrefs.SetInt("MovingMap", cur_MapMoving);
        PlayerPrefs.SetInt("mapIndex", cur_MapIndex);
        PlayerPrefs.SetInt("QuestID", cur_QuestID);
        PlayerPrefs.SetInt("QuestIndex", cur_QuestIndex);

        PlayerPrefs.SetInt("PlayerLevel", level);
        PlayerPrefs.SetInt("PlayerHp", cur_hp);
        PlayerPrefs.SetInt("PlayerMp", cur_mp);
        PlayerPrefs.SetInt("CurExp", cur_exp);

        PlayerPrefs.SetString("QuestName", cur_QuestName);

        string itemArr = ""; // 문자열 생성

        for (int i = 0; i < haveItem.Length; i++) // 배열과 ','를 번갈아가며 tempStr에 저장
        {
            itemArr = itemArr + haveItem[i];
            if (i < haveItem.Length - 1) // 최대 길이의 -1까지만 ,를 저장
            {
                itemArr = itemArr + ",";
            }
        }

        PlayerPrefs.SetString("ItemList", itemArr); // PlyerPrefs에 문자열 형태로 저장

        string quickSlotArr = ""; // 문자열 생성

        for (int i = 0; i < have_QuickSlot_Skill.Length; i++) // 배열과 ','를 번갈아가며 tempStr에 저장
        {
            quickSlotArr = quickSlotArr + have_QuickSlot_Skill[i];
            if (i < have_QuickSlot_Skill.Length - 1) // 최대 길이의 -1까지만 ,를 저장
            {
                quickSlotArr = quickSlotArr + ",";
            }
        }

        PlayerPrefs.SetString("QuickSlot", quickSlotArr); // PlyerPrefs에 문자열 형태로 저장
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor" && rigid.velocity.y <= 0.5f) // 발판에 착지 했다면 
        {
            Debug.Log("발판");
            Player_Anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ladder") // 사다리
            isLadder = true;

        if (collision.tag == "Portal") // 포탈
        {
            isPortal = true;
        }

        scanObject = collision.gameObject;  // 현재 충돌 오브젝트 저장.
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.gameObject.tag == "Ladder") // 충돌체 tag 가 Ladder 사다리 라면 
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)) // 아래키나 위키를 누를 경우
            {
                rigid.velocity = Vector2.zero;

                Player_Anim.SetBool("isLadder", true);
                rigid.gravityScale = 0f; // gravityScale 을 0으로 바꿔줘서 중력을 받지 않게 한 후 
                if (Input.GetKey(KeyCode.DownArrow)) // 아래 키를 눌렀다면 사다리가 달려있는 Floor와 충돌하지 않게 잠시 꺼줌.
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 0.7f, LayerMask.GetMask("Floor"));
                    Debug.DrawRay(transform.position, Vector2.down * 0.7f, Color.red);
                    if (hit.collider != null)
                    {
                        hit.collider.gameObject.SetActive(false);

                        StartCoroutine(DownJump(hit.collider.gameObject));
                    }
                }
                isJump = false;
                Player_Anim.SetBool("isJump", false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) // 충돌 체크 해제를 해주기 위해 선언.
    {
        if (collision.gameObject.tag == "Ladder") // 사다리에서 벗어난다면
        {
            Player_Anim.SetBool("isLadder", false); // 애니메이션 비활성화
            isLadder = false;
            rigid.gravityScale = 1f; // 중력을 다시 받게 해줌.
        }

        if (collision.tag == "Portal") // 포탈
        {
            isPortal = false;
        }
    }

    IEnumerator hit_Timer() // 피격당했을 시 코루틴
    {
        yield return new WaitForSeconds(1f);

        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1); // 투명도를 다시 올려줌.
        this.gameObject.layer = 6; // Player 레이어로 다시 변경해줌 
    }

    public void IsHit(int damage, int dir) // 피격당했을 시
    {
        cur_hp -= damage; // 피격당한 데미지 만큼 체력을 감소 시켜 준 후

        this.gameObject.layer = 11; // UnPlayer 레이어로 변경 시켜줘서 몬스터와 충돌을 잠시 꺼줌.

        hp_bar.fillAmount = (float)cur_hp / (float)hp[level]; // 체력바를 현재 체력비율로 깎아줌.

        if (cur_hp <= 0) // 죽었을때
        {
            // gameObject.SetActive(false); 
            Die(); // 사망 함수 호출
        }

        if (isDie) // 죽었다면 return
            return;

        rigid.AddForce(new Vector2(dir * 2, 2), ForceMode2D.Impulse); // 피격당한 방향에 맞게 Addforce

        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f); // 투명도를 낮춰서 피격당함을 알려줌.

        StartCoroutine(hit_Timer()); // 피격 코루틴 시작.
    }

    void Die() // 사망 함수
    {
        GameObject die_eff = Instantiate(die_Tomb); // 사망 비석 오브젝트 생성 
        die_eff.SetActive(true); // 사망 비석 오브젝트 활성화
        die_eff.transform.position = new Vector3(transform.position.x, transform.position.y + 4.8f, 0); // 위치를 현재 사망한 플레이어 위치보다 높게
        // 잡아준 후 떨어트려줌.
        audioSource.PlayOneShot(soundManager.audioList[9]); // 사망 사운드 재생
        isDie = true; 
        this.gameObject.layer = 11; // UnPlayer 레이어로 바꿔줘서 더이상 맞지 않게 해줌.

        Player_Anim.enabled = false; // 애니메이터 비활성화

        GetComponent<SpriteRenderer>().sprite = die_Sprite; // 사망한 후 플레이어 스프라이트를 바꿔줌

        gameManager.revive_Image.gameObject.SetActive(true); // 부활 버튼을 누를 수 있는 이미지를 활성화.
    }

    public void LevelUp() // 레벨업 함수 
    {
        audioSource.PlayOneShot(soundManager.audioList[6]); // 레벨업에 맞는 사운드 재생
        GameObject up_Effect = Instantiate(levelUp_Eff); // 레벨업 이펙트 생성
        up_Effect.SetActive(true); // 이펙트 활성화
        up_Effect.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, 0); // 위치 지정
        level += 1; // 레벨을 올려주고 레벨에 따른 hp,mp 올려준 후 현재 경험치 초기화
        cur_hp = hp[level];
        cur_mp = mp[level];
        cur_exp = 0;
        gameManager.level_text.text = "Lv     " + (level + 1); // 텍스트도 변경.
        GameObject.Destroy(up_Effect, 1.5f); // 1.5초 후 레벨업 이펙트 삭제.
    }

    void lie() // 엎드리기 함수
    {
        if (gameManager.isAction) // 대화 중이라면 return
            return;

        if (Input.GetKeyDown(KeyCode.DownArrow)) // 아래 방향키를 눌렀다면.
        {
            this.GetComponent<BoxCollider2D>().size = this.GetComponent<BoxCollider2D>().size * 0.5f; // 충돌범위 (boxcollider2D) 크기를 스프라이트
            // 에 맞게 줄여준 후

            if (!isLadder) // 만약 사다리를 타고있지 않았다면
            {
                isDown = true; // isDown 변수를 true로 바꿔준후
                Player_Anim.SetBool("isProne", true); // 엎드리기 애니메이션 활성화
            }
        }

        if (Input.GetKeyUp(KeyCode.DownArrow)) // 아래 방향키를 뗐다면
        {
            this.GetComponent<BoxCollider2D>().size = this.GetComponent<BoxCollider2D>().size * 2f; // 충돌범위를 원상복구 시켜준 후
            isDown = false; // isDown 변수를 false로 바꿔준 후 
            Player_Anim.SetBool("isProne", false); // 엎드리기 애니메이션 비활성화
        }
    }

    void Jump() // 점프 함수
    {
        if (gameManager.isAction) // 대화중이라면 return 
            return;

        if (isJump) // 이미 점프중이라면 return - 더블 점프 방지.
            return;

        if (Input.GetKeyDown(KeyCode.LeftAlt)) // Alt 키를 눌렀을 경우 
        {
            if (isDown) // 아래 방향키를 누르고 있었다면
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 0.7f, LayerMask.GetMask("Floor"));
                // Layer 가 Floor 인 충돌체와 충돌했는지 판별하기 위해 RayCast 사용.
                Debug.DrawRay(transform.position, Vector2.down * 0.7f, Color.red);
                if (hit.collider != null) // RayCastHit2D 에 충돌된 충돌체가 있었다면 
                {
                    //Debug.Log(hit.collider.name);
                    hit.collider.gameObject.SetActive(false); // 그 충돌체를 꺼준 후 

                    StartCoroutine(DownJump(hit.collider.gameObject)); // 다시 살려주기 위해 코루틴 실행.
                }
            }
            else // 아래 방향키를 누르고 있지 않았다면 
            {
                rigid.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse); // 점프 실행.
                Player_Anim.SetBool("isJump", true);
            }
            audioSource.PlayOneShot(soundManager.audioList[4]); // 해당 동작에 맞는 사운드 재생.
            isJump = true;
        }       
    }

    IEnumerator DownJump(GameObject obj) // 하단 점프 했을 시 1초 뒤 하단 점프를 한 블럭 살아나게 함.
    {
        yield return new WaitForSeconds(1f);

        obj.SetActive(true);
    }

    public IEnumerator AttackTime() // 공격한 후 시간 지나면 처리해주는 코루틴.
    {
        yield return new WaitForSeconds(0.7f);

        isAttack = false;
        Player_Anim.SetBool("isAttack", false);
    }

    void Move() // 플레이어 움직임 함수. 
    {
        if (isDown || gameManager.isAction || isAttack || isDie) // 해당 동작들을 수행중이라면 return 
            return;

        if (Input.GetAxisRaw("Horizontal") == 0) // 이동하는 값이 없다면 
        {
            Player_Anim.SetBool("isWalking", false); // 이동 애니메이션 비활성화
        }
        else // 있다면 
        {
            Player_Anim.SetBool("isWalking", true); // 이동 애니메이션 활성화
        }

        Vector3 tempVec = transform.position; // 현재 위치를 받아온 후 

        tempVec.x += Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime; // 이동 방향에 이동속도를 곱해준 값을 더해줌.

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            this.GetComponent<SpriteRenderer>().flipX = true;
            attackCollider.offset = new Vector2(0.64f, 0); // 공격 충돌 판정 범위 방향 변경
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            this.GetComponent<SpriteRenderer>().flipX = false;
            attackCollider.offset = new Vector2(-0.34f, 0); // 공격 충돌 판정 범위 방향 변경
        }

        if (isLadder) // 사다리와 trigger 검사 후 true일때만
        {
            tempVec.y += Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime; // 위 아래 사다리 타는 움직임.
            Debug.Log(Input.GetAxisRaw("Vertical"));
        }

        transform.position = tempVec; // 위치를 잡아줌.
    }

    void Pick_Up_Item() // 아이템 줍기 함수.
    {
        Item tempItem = scanObject.GetComponent<Item>(); // 아이템 스크립트를 받아옴.
        inventory.AcquireItem(itemManager.itemList[tempItem.itemCode - 1]); // 획득한 아이템의 코드를 이용해 
        // 아이템을 추가해줌.
        Destroy(scanObject.gameObject);
    }

    void Attack() // 공격 함수
    {
        if (gameManager.isAction) // 대화중이라면 return 
            return;

        if (isAttack) // 이미 공격중이라면 return
            return;

        if (Input.GetKeyDown(KeyCode.LeftControl)) // 왼쪽 컨트롤 키를 누르면
        {
            isAttack = true; // 공격 bool 변수를 true로 

            Player_Anim.SetBool("isAttack", true); // 공격 애니메이션 활성화

            StartCoroutine(AttackTime()); // 공격 코루틴 시작

            audioSource.PlayOneShot(soundManager.audioList[3]); // 공격 사운드 재생
        }
    }

    void Use_QuickSlot() // 퀵슬롯 사용.
    {
        if (gameManager.isAction) // 대화 중이라면 return 
            return;

        int direct; 
        if (this.GetComponent<SpriteRenderer>().flipX) // 현재 스프라이트의 방향에 따라서 direct를 정해준 후 
            direct = 1;
        else
            direct = -1;


        // 지정해놓은 스킬 퀵슬롯에 스킬이 등록돼있다면 스킬사용
        if (Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            Use_Skill(0, direct);
        }

        if (Input.GetKeyDown(KeyCode.Insert))
        {
            Use_Skill(1, direct);
        }

        if (Input.GetKeyDown(KeyCode.Home))
        {
            Use_Skill(2, direct);
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            Use_Skill(3, direct);
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Use_Skill(4, direct);
        }

        if (Input.GetKeyDown(KeyCode.End))
        {
            Use_Skill(5, direct);
        }

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            Use_Skill(6, direct);
        }
    }

    IEnumerator boost_Time() // boost 스킬 사용시 코루틴
    {
        yield return new WaitForSeconds(4f);

        moveSpeed -= 1.5f;
    }

    void Use_Skill(int index, int direct) // 스킬 사용 함수.
    {
        if (have_QuickSlot_Skill[index] != 0) // 플레이어가 해당 퀵슬롯에 스킬을 착용하고 있다면
        {
            if (isAttack) // 공격중이라면 return 
                return;

            Skill skillObj; // 스킬오브젝트
            Skill quickSlot_Obj = quickSlot.Skill_slots[index].skill; // 퀵슬롯의 스킬을 받아옴.

            if (!quickSlot_Obj.usable_Skill) // 현재 쿨타임이라면 reutrn 
                return;

            if (have_QuickSlot_Skill[index] == 1) // 달팽이 세마리 스킬
            {
                skillObj = GameObject.Instantiate(skillManager.skillList[have_QuickSlot_Skill[index] - 1]);
                skillObj.transform.position = transform.position;
            }
            else if(have_QuickSlot_Skill[index] == 2)// 민첩한 발놀림 스킬
            {
                skillObj = GameObject.Instantiate(skillManager.skillList[have_QuickSlot_Skill[index] - 1], this.transform);
                skillObj.transform.position = new Vector3(skillObj.transform.position.x, skillObj.transform.position.y + 0.3f, 0);

                StartCoroutine(boost_Time());
            }
            else // 회복
            {
                skillObj = GameObject.Instantiate(skillManager.skillList[have_QuickSlot_Skill[index] - 1], this.transform);
                skillObj.transform.position = new Vector3(skillObj.transform.position.x, skillObj.transform.position.y + 0.3f, 0);
            }

            skillObj.Act_Skill(direct); // 스킬 사용.

            if (quickSlot.Skill_slots[index].skill.CoolTime != 0) // 스킬이 쿨타임이 있다면
            {
                quickSlot.Skill_slots[index].coolTime_Start = true;
                quickSlot.Skill_slots[index].startTime = Time.time;

                quickSlot_Obj.usable_Skill = false; // 쿨타임을 돌려줌.
            }
        }
    }

    void Action() // 상호작용
    {
        if (isDie) // 죽어있으면 return
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 2.5f, LayerMask.GetMask("Npc")); // 플레이어 주변에서 npc를 찾아줌.
            if (colls.Length != 0) // npc 가 1명이라도 있다면
                scanNpc = colls[0].gameObject; // 가장 가까운 npc를 넣어줌.
        }

        if (Input.GetKeyDown(KeyCode.Space) && scanNpc != null) // 스페이스바를 눌렀을때 주변에 Npc가 있다면 
        {
            gameManager.Action(scanNpc); 
        }

        if (isPortal && Input.GetKeyDown(KeyCode.UpArrow)) // 포탈 상호작용
        {
            audioSource.PlayOneShot(soundManager.audioList[8]); // 포탈 타는 사운드 재생 후 
            scanObject.GetComponent<PortalData>().Map_Move(); // 맵이동
        }

        if (scanObject != null) // 만약 아이템을 상호작용했다면
        {
            if (Input.GetKeyDown(KeyCode.Z) && scanObject.tag == "Item")
            {
                audioSource.PlayOneShot(soundManager.audioList[5]); // 아이템 줍기 사운드 재생
                Pick_Up_Item(); // 아이템 줍기 함수 호출.
            }
        }

        if (Input.GetKeyDown(KeyCode.F1)) // 저장된 데이터를 지우는 키.
            PlayerPrefs.DeleteAll();

        exp_bar.fillAmount = (float)cur_exp / (float)need_exp[level]; // 경험치 바
    }

    void Update()
    {
        Jump();

        lie(); // 아래 방향키를 눌러서 호출.

        Attack();

        Action();

        Use_QuickSlot();
    }

    private void FixedUpdate()
    {
        Move();
    }
}
