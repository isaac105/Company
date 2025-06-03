using UnityEngine;

public class EnemyCharacter : MonoBehaviour
{
    [Header("적 기본 속성")]
    [SerializeField] protected float hp;
    [SerializeField] protected float damage = 15f;
    [SerializeField] protected float attackCoefficient;
    [SerializeField] protected float defenseCoefficient;
    [SerializeField] protected Item currentItem;

    [Header("최대 HP 설정")]
    [SerializeField] protected float maxHp;

    [Header("적 정보")]
    [SerializeField] protected string enemyName = "적";
    [SerializeField] protected string enemyRank = "Unknown";

    [Header("디버그 정보")]
    [SerializeField] private string currentItemDebugInfo = "None";

    public float HP
    {
        get { return hp; }
        set { hp = Mathf.Clamp(value, 0f, maxHp); }
    }

    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }
    
    public float MaxHP 
    { 
        get { return maxHp; } 
        set { maxHp = value; } 
    }
    
    public string EnemyName
    {
        get { return enemyName; }
        set { enemyName = value; }
    }
    
    public string EnemyRank
    {
        get { return enemyRank; }
        set { enemyRank = value; }
    }
    
    public Item CurrentItem 
    { 
        get { return currentItem; } 
        set { 
            currentItem = value;
            
            if (currentItem != null)
            {
                currentItemDebugInfo = currentItem.gameObject.name + " (데미지: " + currentItem.DamageCoefficient + ")";
            }
            else
            {
                currentItemDebugInfo = "None";
            }
            
            Debug.Log(enemyName + " CurrentItem 프로퍼티 설정됨: " + (value != null ? value.GetItemInfo() : "null"));
        } 
    }
    
    public float AttackCoefficient 
    { 
        get { return attackCoefficient; } 
        set { attackCoefficient = value; } 
    }
    
    public float DefenseCoefficient 
    { 
        get { return defenseCoefficient; } 
        set { defenseCoefficient = value; } 
    }
    
    protected virtual void Start()
    {
        InitializeEnemy();
    }
    
    protected virtual void InitializeEnemy()
    {
        if (maxHp <= 0) maxHp = 100f;
        if (hp <= 0) hp = maxHp;
        if (attackCoefficient <= 0) attackCoefficient = 1.0f;
        if (defenseCoefficient <= 0) defenseCoefficient = 1.0f;
        
        Debug.Log(enemyName + " (" + enemyRank + ") 초기화 완료: HP " + hp + "/" + maxHp);
    }
    
    public virtual void TakeDamage(float damage)
    {
        float finalDamage = damage / defenseCoefficient;
        HP -= finalDamage;
        
        Debug.Log(enemyName + "이 데미지 받음: " + finalDamage + " - 현재 HP: " + HP + "/" + maxHp);
        
        if (HP <= 0)
        {
            OnEnemyDeath();
        }
    }
    
    public virtual float CalculateAttackDamage()
    {
        float itemDamageBonus = currentItem != null ? currentItem.DamageCoefficient : 1.0f;
        float finalDamage = damage * attackCoefficient * itemDamageBonus;
        
        Debug.Log(enemyName + " 공격 데미지 계산: " + finalDamage);
        return finalDamage;
    }
    
    public virtual void EquipItem(Item newItem)
    {
        Debug.Log(enemyName + "이 아이템 장착. 새 아이템: " + (newItem != null ? newItem.GetItemInfo() : "null"));
        
        currentItem = newItem;
        
        if (currentItem != null)
        {
            currentItemDebugInfo = currentItem.gameObject.name + " (데미지: " + currentItem.DamageCoefficient + ")";
            Debug.Log(enemyName + " 아이템 장착 완료: " + currentItem.GetItemInfo());
        }
        else
        {
            currentItemDebugInfo = "None";
            Debug.Log(enemyName + " 아이템 장착: None");
        }
        
        ShowEnemyStatus();
    }
    
    public virtual void Heal(float healAmount)
    {
        HP += healAmount;
        Debug.Log(enemyName + " HP 회복: " + healAmount + " - 현재 HP: " + HP + "/" + maxHp);
    }
    
    protected virtual void OnEnemyDeath()
    {
        Debug.Log(enemyName + " (" + enemyRank + ")이 사망했습니다!");
        OnEnemyDefeated();
    }
    
    protected virtual void OnEnemyDefeated()
    {
        // 각 적별 고유한 사망 처리 (자식 클래스에서 오버라이드)
    }
    
    public virtual void ShowEnemyStatus()
    {
        Debug.Log("=== " + enemyName + " (" + enemyRank + ") 상태 ===");
        Debug.Log("HP: " + HP + "/" + maxHp);
        Debug.Log("공격 계수: " + attackCoefficient);
        Debug.Log("방어 계수: " + defenseCoefficient);
        Debug.Log("현재 아이템: " + (currentItem != null ? currentItem.GetItemInfo() : "None"));
        Debug.Log("===========================");
    }
    
    private void OnMouseDown()
    {
        ShowEnemyStatus();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[E키] " + enemyName + " currentItem 상태: " + (currentItem != null ? currentItem.GetItemInfo() : "None"));
        }
        
        if (currentItem != null && currentItemDebugInfo == "None")
        {
            currentItemDebugInfo = currentItem.gameObject.name + " (데미지: " + currentItem.DamageCoefficient + ")";
        }
        else if (currentItem == null && currentItemDebugInfo != "None")
        {
            currentItemDebugInfo = "None";
        }
    }
}