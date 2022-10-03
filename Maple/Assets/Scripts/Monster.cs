using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    // ���� ��ũ��Ʈ
    Animator animator;
    Player player;
    Rigidbody2D rigid;

    private string curAnim;

    [SerializeField] private int moveSpeed; // �����̴� ����
    [SerializeField] private float mob_Speed; // �����̴� �ӵ�

    [SerializeField] private int hp; // �ִ� ü��
    [SerializeField] private int cur_hp; // ���� ü��
    [SerializeField] private int give_exp; // �÷��̾�� �ִ� ����ġ

    [SerializeField] private int damage; // ������

    public Vector3 hpBarOffset = new Vector3(-0.5f, 2.4f, 0); // ü�¹� ��ġ offset

    public bool isDie = false; // �׾����� �Ǻ��� bool ����.

    GameObject hpBarCanvas; // ü�¹� Canvas
    public GameObject hpBar_prf; // ü�¹� ������
    public GameObject damageText_prf; // ������ �ؽ�Ʈ ������

    public Image real_hp_bar; // ü�¹�
    public Text real_damage_Text; // ������ �ؽ�Ʈ

    RectTransform hpBar; // ü�¹� RectTransform
    RectTransform damageText; // ������ �ؽ�Ʈ RectTransform

    public float height = 1.7f; // ����

    void Start()
    {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        hpBarCanvas = FindObjectOfType<Canvas>().gameObject;
        rigid = GetComponent<Rigidbody2D>();

        hpBar = Instantiate(hpBar_prf, hpBarCanvas.transform).GetComponent<RectTransform>(); // ü�¹� ����
        real_hp_bar = hpBar.transform.GetChild(0).GetComponent<Image>(); // ü�¹� �̹��� �־���.

        damageText = Instantiate(damageText_prf, hpBarCanvas.transform).GetComponent<RectTransform>(); // ü�� �ؽ�Ʈ 
        real_damage_Text = damageText.transform.GetChild(0).GetComponent<Text>();
        damageText.gameObject.SetActive(false);

        cur_hp = hp;

        Think(); // ���� �������� �����ϴ� �Լ�
    }

    void hpBar_Set() // ü�¹� ���� �Լ�.
    {
        Vector3 HpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + height, 0)); // ��ũ�� ��ǥ�� �޾ƿͼ�
        // ��ǥ�� ������ �� 
        hpBar.position = HpBarPos; // ��ǥ�� �־���.

        real_hp_bar.fillAmount = (float)cur_hp / (float)hp; // fillAmount �� ���� ü�º������ ä����.
    }

    public void ReInit() // �ٽ� ���������� �ʱ�ȭ
    {
        cur_hp = hp;
        isDie = false;
        hpBar.gameObject.SetActive(true);
        GetComponent<BoxCollider2D>().isTrigger = false;
        rigid.constraints = RigidbodyConstraints2D.None;
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    IEnumerator Rethink_Time(int time) // �ٽ� �����ϰ� ���ִ� �ڷ�ƾ.
    {
        yield return new WaitForSeconds(time);

        Think(); // ��� ������ �ٽ� �����ϰ� ��.
    }

    IEnumerator hit_Time() // �¾����� �ڷ�ƾ.
    {
        yield return new WaitForSeconds(1f);

        animator.SetBool("isHit", false);
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject.layer == 6) // 6 = Player ���̾�� �浹������
        {
            int hitDir = transform.position.x < collision.transform.position.x ? 1 : -1; // ���ʿ��� �浹�ߴ���, �����ʿ��� �浹�ߴ��� �˻� ��
            player.IsHit(damage, hitDir); // ���⿡ �°� IsHit �Լ� ȣ��.
        }
    }

    int thinkTime = 0;
    void Think() // �������� �����ϱ� ���� �����ϴ� �Լ�.
    {
        if (isDie) // �׾��ٸ� return
            return;

        moveSpeed = Random.Range(-1, 2); // �����̴� ������ �������� -1 , 0 , 1 �� �ϳ�
        thinkTime = Random.Range(3, 7); // ���� ������ �� �� �Ŀ� ������ ��������.

        StartCoroutine(Rethink_Time(thinkTime)); // �ٽ� �����ϴ� �ڷ�ƾ ����.

        animator.SetInteger("Moving", moveSpeed); // �����ӿ� �´� �ִϸ��̼�
    }

    void Move() // �����̴� �Լ�
    {
        if (isDie)  // �׾��ٸ� return
            return;

        Vector3 tempVec = transform.position; // ���� ��ġ�� �޾ƿ� �� 

        tempVec.x += moveSpeed * mob_Speed * Time.deltaTime; // ������ ����, ������ ���ǵ忡 ���� ��ǥ�� ������.

        if (moveSpeed > 0) // �����̴� ���⿡ ���� ��������Ʈ ���� ����.
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (moveSpeed < 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }

        // Ray �� ������� ���� �տ� �߻��� ������������ �˻�.
        Vector2 frontVec = new Vector2(tempVec.x + moveSpeed * 0.2f, tempVec.y);
        Debug.DrawRay(frontVec, Vector3.down * 0.5f, new Color(1, 0, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 0.5f, LayerMask.GetMask("Floor", "UnFloor"));

        if (rayHit.collider == null) // ���� �������� ���
        {
            moveSpeed *= -1;
        }

        transform.position = tempVec; // �������� ��ǥ�� �ٽ� �־���.
    }

    IEnumerator Damage_Text_Off_Time() // �ǰݴ��� �� ��Ÿ���� ������ �ؽ�Ʈ�� ������ �� ���ֱ� ���� �ڷ�ƾ.
    {
        yield return new WaitForSeconds(0.5f);

        damageText.gameObject.SetActive(false);
    }

    void On_DamageText(int oppos_dmg) // �ǰݴ��� �� ��Ÿ�� �������� �����ֱ� ���� �Լ�.
    {
        // ������ �ؽ�Ʈ�� ���� ���� ��ǥ�� ��ũ������ �޾ƿ�.
        Vector3 damagePos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x + 0.8f, transform.position.y + 0.2f, 0)); 
        damageText.position = damagePos; // ��ǥ ����

        damageText.gameObject.SetActive(true); // Ȱ��ȭ ���� ������.

        real_damage_Text.text = oppos_dmg.ToString(); // �ؽ�Ʈ�� ���� ������

        StartCoroutine(Damage_Text_Off_Time());
    }

    IEnumerator die_Time() // �׾����� �ڷ�ƾ
    {
        yield return new WaitForSeconds(1.2f);

        damageText.gameObject.SetActive(false); // ������ �ؽ�Ʈ ��Ȱ��ȭ.
        MonsterManager.ReturnObject(this); // ������Ʈ Ǯ�� ����
    }

    public void isAttacked(int oppos_dmg) // �������� �Ծ��� ��
    {
        On_DamageText(oppos_dmg); // ������ �ؽ�Ʈ�� ����� ��

        cur_hp -= oppos_dmg; // ü���� ���� ������.

        if (cur_hp <= 0) // ü���� 0 ���ϰ� �Ǹ�.
        {
            GetComponent<BoxCollider2D>().isTrigger = true; // �÷��̾�� �浹�� ���� �� 

            rigid.constraints = RigidbodyConstraints2D.FreezePosition; // ��ǥ�� ����.

            isDie = true; 
            player.catchCount += 1; // �÷��̾��� ��� �� ����

            animator.SetBool("isDie", true); // ��� �ִϸ��̼� Ȱ��ȭ 

            player.cur_exp += give_exp; // �÷��̾� ����ġ ���� 

            if (player.cur_exp >= player.need_exp[player.level]) // �÷��̾� ����ġ�� ���� ���� ���� �ִ� ����ġ �̻��̶��
            {
                player.LevelUp(); // �÷��̾� ������ �Լ� ȣ��
            }

            hpBar.gameObject.SetActive(false); // ü�¹� ��Ȱ��ȭ

            StartCoroutine(die_Time()); // ��� �ڷ�ƾ ����.
        }

        int forceDir = player.GetComponent<SpriteRenderer>().flipX == true ? 1 : -1; // �÷��̾����� �ǰݴ��� ���� �˻� ��
        rigid.AddForce(new Vector2(forceDir, 2), ForceMode2D.Impulse); // ���⿡ �´� �ǰ� ó��.

        animator.SetBool("isHit", true); // �ǰ� �ִϸ��̼� Ȱ��ȭ

        StartCoroutine(hit_Time()); // �ǰ� �ڷ�ƾ ����.
    }

    void Update()
    {
        Move();

        hpBar_Set();
    }
}
