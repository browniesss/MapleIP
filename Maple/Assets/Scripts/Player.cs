using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // �÷��̾� ��ũ��Ʈ
    // �Ŵ�����.
    public GameManager gameManager = null;
    public ItemManager itemManager = null;
    public SkillManager skillManager = null;
    public SoundManager soundManager = null;

    public AudioSource audioSource = null; // ����� �ҽ� ( �Ҹ� ����� �� )

    public float moveSpeed = 5f; // �̵��ӵ�
    public float jumpPower = 3f; // ���� �Ŀ�

    Rigidbody2D rigid = null;

    // ���� ���� BOOL ������
    protected bool isDown = false;
    protected bool isJump = false;
    protected bool isDie = false;
    public bool isAttack = false;
    public bool isLadder = false;
    protected bool isPortal = false;

    public QuickSlot quickSlot = null; // ������

    public Animator Player_Anim = null; // �ִϸ�����

    public BoxCollider2D attackCollider = null; // ���� ����

    // scan������Ʈ��
    public GameObject scanObject;
    public GameObject scanNpc;

    public Inventory_Base inventory = null; // �κ��丮

    public GameObject levelUp_Eff = null; // ������ ����Ʈ
    public GameObject die_Tomb = null; // ����� ������ ��

    public Sprite die_Sprite = null; // �׾����� ��������Ʈ

    // �÷��̾� ����
    public int[] mp;
    public int[] hp;
    public int[] need_exp;
    public int[] damage;
    public int catchCount;

    // PlayerPrefs �� ������ ���� 
    public int level;
    public int cur_mp;
    public int cur_hp;
    public int cur_exp;
    public int cur_MapIndex;
    public int cur_MapMoving = 0; // 1�̸� ���� �ʿ��� �°� , 2�� ���� �ʿ��� �°�
    public string cur_QuestName; // ���� ����Ʈ �̸� 
    public int cur_QuestID; // ���� ����Ʈ ID
    public int cur_QuestIndex;
    public int[] haveItem = new int[24]; // ���� ������ �ִ� ������. 
    public int[] have_QuickSlot_Skill = new int[7];

    // HP , MP , EXP , LEVEL �� UI
    public Image hp_bar;
    public Image mp_bar;
    public Image exp_bar;

    private void Awake()
    {
        //PlayerPrefs.DeleteAll();

        rigid = GetComponent<Rigidbody2D>();
        Player_Anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (PlayerPrefs.HasKey("MovingMap")) // ������ִ� ���� �ִٸ� �ٽ� �ҷ���.
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

            string[] itemDataArr = PlayerPrefs.GetString("ItemList").Split(','); // PlayerPrefs���� �ҷ��� ���� Split �Լ��� ���� ���ڿ���
                                                                                 // ,�� �����Ͽ� �迭�� ����

            for (int i = 0; i < itemDataArr.Length; i++)
            {
                haveItem[i] = System.Convert.ToInt32(itemDataArr[i]); // ���ڿ� ���·� ����� ����
                                                                      // ���������� ��ȯ�� ����
            }

            string[] quickSlotData = PlayerPrefs.GetString("QuickSlot").Split(','); // PlayerPrefs���� �ҷ��� ���� Split �Լ��� ���� ���ڿ���
                                                                                    // ,�� �����Ͽ� �迭�� ����

            for (int i = 0; i < quickSlotData.Length; i++)
            {
                have_QuickSlot_Skill[i] = System.Convert.ToInt32(quickSlotData[i]); // ���ڿ� ���·� ����� ����
                                                                                    // ���������� ��ȯ�� ����
            }
        }
        else // ����� ���� ���� ���
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
        gameManager.level_text.text = "Lv     " + (level + 1); // ���� �ؽ�Ʈ
        hp_bar.fillAmount = (float)cur_hp / (float)hp[level]; // ü�� �� 
        exp_bar.fillAmount = (float)cur_exp / (float)need_exp[level]; // ����ġ ��
    }

    public void sceneSave() // ���� ������ �Ѿ �� ������ ������ �� �Լ�.
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

        string itemArr = ""; // ���ڿ� ����

        for (int i = 0; i < haveItem.Length; i++) // �迭�� ','�� �����ư��� tempStr�� ����
        {
            itemArr = itemArr + haveItem[i];
            if (i < haveItem.Length - 1) // �ִ� ������ -1������ ,�� ����
            {
                itemArr = itemArr + ",";
            }
        }

        PlayerPrefs.SetString("ItemList", itemArr); // PlyerPrefs�� ���ڿ� ���·� ����

        string quickSlotArr = ""; // ���ڿ� ����

        for (int i = 0; i < have_QuickSlot_Skill.Length; i++) // �迭�� ','�� �����ư��� tempStr�� ����
        {
            quickSlotArr = quickSlotArr + have_QuickSlot_Skill[i];
            if (i < have_QuickSlot_Skill.Length - 1) // �ִ� ������ -1������ ,�� ����
            {
                quickSlotArr = quickSlotArr + ",";
            }
        }

        PlayerPrefs.SetString("QuickSlot", quickSlotArr); // PlyerPrefs�� ���ڿ� ���·� ����
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor" && rigid.velocity.y <= 0.5f) // ���ǿ� ���� �ߴٸ� 
        {
            Debug.Log("����");
            Player_Anim.SetBool("isJump", false);
            isJump = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ladder") // ��ٸ�
            isLadder = true;

        if (collision.tag == "Portal") // ��Ż
        {
            isPortal = true;
        }

        scanObject = collision.gameObject;  // ���� �浹 ������Ʈ ����.
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        if (collision.gameObject.tag == "Ladder") // �浹ü tag �� Ladder ��ٸ� ��� 
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow)) // �Ʒ�Ű�� ��Ű�� ���� ���
            {
                rigid.velocity = Vector2.zero;

                Player_Anim.SetBool("isLadder", true);
                rigid.gravityScale = 0f; // gravityScale �� 0���� �ٲ��༭ �߷��� ���� �ʰ� �� �� 
                if (Input.GetKey(KeyCode.DownArrow)) // �Ʒ� Ű�� �����ٸ� ��ٸ��� �޷��ִ� Floor�� �浹���� �ʰ� ��� ����.
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

    private void OnTriggerExit2D(Collider2D collision) // �浹 üũ ������ ���ֱ� ���� ����.
    {
        if (collision.gameObject.tag == "Ladder") // ��ٸ����� ����ٸ�
        {
            Player_Anim.SetBool("isLadder", false); // �ִϸ��̼� ��Ȱ��ȭ
            isLadder = false;
            rigid.gravityScale = 1f; // �߷��� �ٽ� �ް� ����.
        }

        if (collision.tag == "Portal") // ��Ż
        {
            isPortal = false;
        }
    }

    IEnumerator hit_Timer() // �ǰݴ����� �� �ڷ�ƾ
    {
        yield return new WaitForSeconds(1f);

        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1); // ������ �ٽ� �÷���.
        this.gameObject.layer = 6; // Player ���̾�� �ٽ� �������� 
    }

    public void IsHit(int damage, int dir) // �ǰݴ����� ��
    {
        cur_hp -= damage; // �ǰݴ��� ������ ��ŭ ü���� ���� ���� �� ��

        this.gameObject.layer = 11; // UnPlayer ���̾�� ���� �����༭ ���Ϳ� �浹�� ��� ����.

        hp_bar.fillAmount = (float)cur_hp / (float)hp[level]; // ü�¹ٸ� ���� ü�º����� �����.

        if (cur_hp <= 0) // �׾�����
        {
            // gameObject.SetActive(false); 
            Die(); // ��� �Լ� ȣ��
        }

        if (isDie) // �׾��ٸ� return
            return;

        rigid.AddForce(new Vector2(dir * 2, 2), ForceMode2D.Impulse); // �ǰݴ��� ���⿡ �°� Addforce

        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f); // ������ ���缭 �ǰݴ����� �˷���.

        StartCoroutine(hit_Timer()); // �ǰ� �ڷ�ƾ ����.
    }

    void Die() // ��� �Լ�
    {
        GameObject die_eff = Instantiate(die_Tomb); // ��� �� ������Ʈ ���� 
        die_eff.SetActive(true); // ��� �� ������Ʈ Ȱ��ȭ
        die_eff.transform.position = new Vector3(transform.position.x, transform.position.y + 4.8f, 0); // ��ġ�� ���� ����� �÷��̾� ��ġ���� ����
        // ����� �� ����Ʈ����.
        audioSource.PlayOneShot(soundManager.audioList[9]); // ��� ���� ���
        isDie = true; 
        this.gameObject.layer = 11; // UnPlayer ���̾�� �ٲ��༭ ���̻� ���� �ʰ� ����.

        Player_Anim.enabled = false; // �ִϸ����� ��Ȱ��ȭ

        GetComponent<SpriteRenderer>().sprite = die_Sprite; // ����� �� �÷��̾� ��������Ʈ�� �ٲ���

        gameManager.revive_Image.gameObject.SetActive(true); // ��Ȱ ��ư�� ���� �� �ִ� �̹����� Ȱ��ȭ.
    }

    public void LevelUp() // ������ �Լ� 
    {
        audioSource.PlayOneShot(soundManager.audioList[6]); // �������� �´� ���� ���
        GameObject up_Effect = Instantiate(levelUp_Eff); // ������ ����Ʈ ����
        up_Effect.SetActive(true); // ����Ʈ Ȱ��ȭ
        up_Effect.transform.position = new Vector3(transform.position.x, transform.position.y + 2.5f, 0); // ��ġ ����
        level += 1; // ������ �÷��ְ� ������ ���� hp,mp �÷��� �� ���� ����ġ �ʱ�ȭ
        cur_hp = hp[level];
        cur_mp = mp[level];
        cur_exp = 0;
        gameManager.level_text.text = "Lv     " + (level + 1); // �ؽ�Ʈ�� ����.
        GameObject.Destroy(up_Effect, 1.5f); // 1.5�� �� ������ ����Ʈ ����.
    }

    void lie() // ���帮�� �Լ�
    {
        if (gameManager.isAction) // ��ȭ ���̶�� return
            return;

        if (Input.GetKeyDown(KeyCode.DownArrow)) // �Ʒ� ����Ű�� �����ٸ�.
        {
            this.GetComponent<BoxCollider2D>().size = this.GetComponent<BoxCollider2D>().size * 0.5f; // �浹���� (boxcollider2D) ũ�⸦ ��������Ʈ
            // �� �°� �ٿ��� ��

            if (!isLadder) // ���� ��ٸ��� Ÿ������ �ʾҴٸ�
            {
                isDown = true; // isDown ������ true�� �ٲ�����
                Player_Anim.SetBool("isProne", true); // ���帮�� �ִϸ��̼� Ȱ��ȭ
            }
        }

        if (Input.GetKeyUp(KeyCode.DownArrow)) // �Ʒ� ����Ű�� �ôٸ�
        {
            this.GetComponent<BoxCollider2D>().size = this.GetComponent<BoxCollider2D>().size * 2f; // �浹������ ���󺹱� ������ ��
            isDown = false; // isDown ������ false�� �ٲ��� �� 
            Player_Anim.SetBool("isProne", false); // ���帮�� �ִϸ��̼� ��Ȱ��ȭ
        }
    }

    void Jump() // ���� �Լ�
    {
        if (gameManager.isAction) // ��ȭ���̶�� return 
            return;

        if (isJump) // �̹� �������̶�� return - ���� ���� ����.
            return;

        if (Input.GetKeyDown(KeyCode.LeftAlt)) // Alt Ű�� ������ ��� 
        {
            if (isDown) // �Ʒ� ����Ű�� ������ �־��ٸ�
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 0.7f, LayerMask.GetMask("Floor"));
                // Layer �� Floor �� �浹ü�� �浹�ߴ��� �Ǻ��ϱ� ���� RayCast ���.
                Debug.DrawRay(transform.position, Vector2.down * 0.7f, Color.red);
                if (hit.collider != null) // RayCastHit2D �� �浹�� �浹ü�� �־��ٸ� 
                {
                    //Debug.Log(hit.collider.name);
                    hit.collider.gameObject.SetActive(false); // �� �浹ü�� ���� �� 

                    StartCoroutine(DownJump(hit.collider.gameObject)); // �ٽ� ����ֱ� ���� �ڷ�ƾ ����.
                }
            }
            else // �Ʒ� ����Ű�� ������ ���� �ʾҴٸ� 
            {
                rigid.AddForce(Vector3.up * jumpPower, ForceMode2D.Impulse); // ���� ����.
                Player_Anim.SetBool("isJump", true);
            }
            audioSource.PlayOneShot(soundManager.audioList[4]); // �ش� ���ۿ� �´� ���� ���.
            isJump = true;
        }       
    }

    IEnumerator DownJump(GameObject obj) // �ϴ� ���� ���� �� 1�� �� �ϴ� ������ �� �� ��Ƴ��� ��.
    {
        yield return new WaitForSeconds(1f);

        obj.SetActive(true);
    }

    public IEnumerator AttackTime() // ������ �� �ð� ������ ó�����ִ� �ڷ�ƾ.
    {
        yield return new WaitForSeconds(0.7f);

        isAttack = false;
        Player_Anim.SetBool("isAttack", false);
    }

    void Move() // �÷��̾� ������ �Լ�. 
    {
        if (isDown || gameManager.isAction || isAttack || isDie) // �ش� ���۵��� �������̶�� return 
            return;

        if (Input.GetAxisRaw("Horizontal") == 0) // �̵��ϴ� ���� ���ٸ� 
        {
            Player_Anim.SetBool("isWalking", false); // �̵� �ִϸ��̼� ��Ȱ��ȭ
        }
        else // �ִٸ� 
        {
            Player_Anim.SetBool("isWalking", true); // �̵� �ִϸ��̼� Ȱ��ȭ
        }

        Vector3 tempVec = transform.position; // ���� ��ġ�� �޾ƿ� �� 

        tempVec.x += Input.GetAxisRaw("Horizontal") * moveSpeed * Time.deltaTime; // �̵� ���⿡ �̵��ӵ��� ������ ���� ������.

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            this.GetComponent<SpriteRenderer>().flipX = true;
            attackCollider.offset = new Vector2(0.64f, 0); // ���� �浹 ���� ���� ���� ����
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            this.GetComponent<SpriteRenderer>().flipX = false;
            attackCollider.offset = new Vector2(-0.34f, 0); // ���� �浹 ���� ���� ���� ����
        }

        if (isLadder) // ��ٸ��� trigger �˻� �� true�϶���
        {
            tempVec.y += Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime; // �� �Ʒ� ��ٸ� Ÿ�� ������.
            Debug.Log(Input.GetAxisRaw("Vertical"));
        }

        transform.position = tempVec; // ��ġ�� �����.
    }

    void Pick_Up_Item() // ������ �ݱ� �Լ�.
    {
        Item tempItem = scanObject.GetComponent<Item>(); // ������ ��ũ��Ʈ�� �޾ƿ�.
        inventory.AcquireItem(itemManager.itemList[tempItem.itemCode - 1]); // ȹ���� �������� �ڵ带 �̿��� 
        // �������� �߰�����.
        Destroy(scanObject.gameObject);
    }

    void Attack() // ���� �Լ�
    {
        if (gameManager.isAction) // ��ȭ���̶�� return 
            return;

        if (isAttack) // �̹� �������̶�� return
            return;

        if (Input.GetKeyDown(KeyCode.LeftControl)) // ���� ��Ʈ�� Ű�� ������
        {
            isAttack = true; // ���� bool ������ true�� 

            Player_Anim.SetBool("isAttack", true); // ���� �ִϸ��̼� Ȱ��ȭ

            StartCoroutine(AttackTime()); // ���� �ڷ�ƾ ����

            audioSource.PlayOneShot(soundManager.audioList[3]); // ���� ���� ���
        }
    }

    void Use_QuickSlot() // ������ ���.
    {
        if (gameManager.isAction) // ��ȭ ���̶�� return 
            return;

        int direct; 
        if (this.GetComponent<SpriteRenderer>().flipX) // ���� ��������Ʈ�� ���⿡ ���� direct�� ������ �� 
            direct = 1;
        else
            direct = -1;


        // �����س��� ��ų �����Կ� ��ų�� ��ϵ��ִٸ� ��ų���
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

    IEnumerator boost_Time() // boost ��ų ���� �ڷ�ƾ
    {
        yield return new WaitForSeconds(4f);

        moveSpeed -= 1.5f;
    }

    void Use_Skill(int index, int direct) // ��ų ��� �Լ�.
    {
        if (have_QuickSlot_Skill[index] != 0) // �÷��̾ �ش� �����Կ� ��ų�� �����ϰ� �ִٸ�
        {
            if (isAttack) // �������̶�� return 
                return;

            Skill skillObj; // ��ų������Ʈ
            Skill quickSlot_Obj = quickSlot.Skill_slots[index].skill; // �������� ��ų�� �޾ƿ�.

            if (!quickSlot_Obj.usable_Skill) // ���� ��Ÿ���̶�� reutrn 
                return;

            if (have_QuickSlot_Skill[index] == 1) // ������ ������ ��ų
            {
                skillObj = GameObject.Instantiate(skillManager.skillList[have_QuickSlot_Skill[index] - 1]);
                skillObj.transform.position = transform.position;
            }
            else if(have_QuickSlot_Skill[index] == 2)// ��ø�� �߳ ��ų
            {
                skillObj = GameObject.Instantiate(skillManager.skillList[have_QuickSlot_Skill[index] - 1], this.transform);
                skillObj.transform.position = new Vector3(skillObj.transform.position.x, skillObj.transform.position.y + 0.3f, 0);

                StartCoroutine(boost_Time());
            }
            else // ȸ��
            {
                skillObj = GameObject.Instantiate(skillManager.skillList[have_QuickSlot_Skill[index] - 1], this.transform);
                skillObj.transform.position = new Vector3(skillObj.transform.position.x, skillObj.transform.position.y + 0.3f, 0);
            }

            skillObj.Act_Skill(direct); // ��ų ���.

            if (quickSlot.Skill_slots[index].skill.CoolTime != 0) // ��ų�� ��Ÿ���� �ִٸ�
            {
                quickSlot.Skill_slots[index].coolTime_Start = true;
                quickSlot.Skill_slots[index].startTime = Time.time;

                quickSlot_Obj.usable_Skill = false; // ��Ÿ���� ������.
            }
        }
    }

    void Action() // ��ȣ�ۿ�
    {
        if (isDie) // �׾������� return
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 2.5f, LayerMask.GetMask("Npc")); // �÷��̾� �ֺ����� npc�� ã����.
            if (colls.Length != 0) // npc �� 1���̶� �ִٸ�
                scanNpc = colls[0].gameObject; // ���� ����� npc�� �־���.
        }

        if (Input.GetKeyDown(KeyCode.Space) && scanNpc != null) // �����̽��ٸ� �������� �ֺ��� Npc�� �ִٸ� 
        {
            gameManager.Action(scanNpc); 
        }

        if (isPortal && Input.GetKeyDown(KeyCode.UpArrow)) // ��Ż ��ȣ�ۿ�
        {
            audioSource.PlayOneShot(soundManager.audioList[8]); // ��Ż Ÿ�� ���� ��� �� 
            scanObject.GetComponent<PortalData>().Map_Move(); // ���̵�
        }

        if (scanObject != null) // ���� �������� ��ȣ�ۿ��ߴٸ�
        {
            if (Input.GetKeyDown(KeyCode.Z) && scanObject.tag == "Item")
            {
                audioSource.PlayOneShot(soundManager.audioList[5]); // ������ �ݱ� ���� ���
                Pick_Up_Item(); // ������ �ݱ� �Լ� ȣ��.
            }
        }

        if (Input.GetKeyDown(KeyCode.F1)) // ����� �����͸� ����� Ű.
            PlayerPrefs.DeleteAll();

        exp_bar.fillAmount = (float)cur_exp / (float)need_exp[level]; // ����ġ ��
    }

    void Update()
    {
        Jump();

        lie(); // �Ʒ� ����Ű�� ������ ȣ��.

        Attack();

        Action();

        Use_QuickSlot();
    }

    private void FixedUpdate()
    {
        Move();
    }
}
