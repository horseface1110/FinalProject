using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]

public class Boss2StatusController : MonoBehaviour
{

    private int currentHealth;
    public BarScript healthBar;

    [SerializeField] int maxHealth = 45;
    bool now_is_sword;
    bool dropCoin = false;
    int playerDamage;
    float playerAttackRange;
    public LayerMask whatIsPlayer;
    GameObject playerObject;
    public GameObject coin;

    private float lastTime;   //�p�ɾ�
    private float curTime;

    public AudioClip die;
    AudioSource audiosource;

    bool guncanhit = true;

    void Start()
    {
        SetMaxHealth();
        playerObject = GameObject.Find("maincharacter");
        audiosource = GetComponent<AudioSource>();
    }

    void Update()
    {
        PlayerAttackCheck();
        StatusCheck(); // check if the mushroom die
    }

    // call the function below to change mushroom's health status
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            audiosource.PlayOneShot(die);
        }
    }

    public void SetMaxHealth()
    {
        healthBar.SetMaxHealth(maxHealth);
        currentHealth = maxHealth;
    }

    void StatusCheck()
    {
        if (currentHealth <= 0 && !dropCoin)
        {
            Debug.Log("die");
            dropCoin = true;
            transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0)); // rotate the enemy's corpse (lying on the ground)
            gameObject.GetComponent<CapsuleCollider>().enabled = false; // disable collider
            gameObject.GetComponent<NavMeshAgent>().enabled = false; // disable navMeshAgent
            gameObject.GetComponent<ArcherAIScript>().alive = false;
            Invoke(nameof(DropCoin), 2);
            Invoke(nameof(DestroyMushroom), 2);
            GameObject.Find("SceneControllerObject").GetComponent<SceneController>().LateGoToStore();
        }
    }

    void DestroyMushroom()
    {
        Destroy(gameObject);
    }

    // damage check functions
    bool IsSword()
    {
        playerDamage = GameObject.Find("maincharacter").GetComponent<WeaponChange>().swordDamage;
        playerAttackRange = GameObject.Find("maincharacter").GetComponent<WeaponChange>().swordAttackRange;
        return GameObject.Find("maincharacter").GetComponent<WeaponChange>().now_is_sword;
    }


    bool IsCloseToPlayer()
    {
        return Physics.CheckSphere(transform.position, playerAttackRange, whatIsPlayer);
    }
    void PlayerAttackCheck()
    {

        if (Input.GetMouseButtonDown(0) && IsSword() && IsCloseToPlayer())
        {
            //GameObject.Find("maincharacter").transform.LookAt(transform); // 玩家轉向敵人，但感覺有點生硬
            TakeDamage(playerDamage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "laser" && guncanhit)
        {
            curTime = Time.time;
            currentHealth -= GameObject.Find("maincharacter").GetComponent<WeaponChange>().gunDamage;
            healthBar.SetHealth(currentHealth);
            guncanhit = false;
            if (curTime - lastTime >= 0.5)   //�ɶ��t�j��0.5���L��
            {
                guncanhit = true;
            }
        }
    }

    private void DropCoin()
    {
        Transform c1 = Instantiate(coin.transform);
        Transform c2 = Instantiate(coin.transform);
        c1.localPosition = transform.position + new Vector3(0, 1, 0);
        c2.localPosition = transform.position + new Vector3(1, 1, 0);

    }
}
