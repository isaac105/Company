using UnityEngine;

public class EnemyCharacter : MonoBehaviour
{
    [Header("적 기본 속성")]
    [SerializeField] private float hp;
    [SerializeField] private float damage = 15f;
    [SerializeField] private float attackCoefficient;
    [SerializeField] private float defenseCoefficient;
    [SerializeField] private Item currentItem;

    [Header("최대 HP 설정")]
    [SerializeField] private float maxHp;

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
            
            Debug.Log("적 CurrentItem 프로퍼티 설정됨: " + (value != null ? value.GetItemInfo() : "null"));
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
    
    void Start()
    {
        if (maxHp <= 0) maxHp = 100f;
        if (hp <= 0) hp = maxHp;
        if (attackCoefficient <= 0) attackCoefficient = 1.0f;
        if (defenseCoefficient <= 0) defenseCoefficient = 1.0f;
        
        Debug.Log("적 캐릭터 초기화 완료: HP " + hp + "/" + maxHp);
    }
    
    public void TakeDamage(float damage)
    {
        float finalDamage = damage / defenseCoefficient;
        HP -= finalDamage;
        
        Debug.Log("적이 데미지 받음: " + finalDamage + " - 현재 HP: " + HP + "/" + maxHp);
        
        if (HP <= 0)
        {
            OnEnemyDeath();
        }
    }
    
    public float CalculateAttackDamage()
    {
        float itemDamageBonus = currentItem != null ? currentItem.DamageCoefficient : 1.0f;
        float finalDamage = damage * attackCoefficient * itemDamageBonus;
        
        Debug.Log("적 공격 데미지 계산: " + finalDamage);
        return finalDamage;
    }
    
    public void EquipItem(Item newItem)
    {
        Debug.Log("적이 아이템 장착. 새 아이템: " + (newItem != null ? newItem.GetItemInfo() : "null"));
        
        currentItem = newItem;
        
        if (currentItem != null)
        {
            currentItemDebugInfo = currentItem.gameObject.name + " (데미지: " + currentItem.DamageCoefficient + ")";
            Debug.Log("적 아이템 장착 완료: " + currentItem.GetItemInfo());
        }
        else
        {
            currentItemDebugInfo = "None";
            Debug.Log("적 아이템 장착: None");
        }
        
        ShowEnemyStatus();
    }
    
    public void Heal(float healAmount)
    {
        HP += healAmount;
        Debug.Log("적 HP 회복: " + healAmount + " - 현재 HP: " + HP + "/" + maxHp);
    }
    
    private void OnEnemyDeath()
    {
        Debug.Log("적이 사망했습니다!");
    }
    
    public void ShowEnemyStatus()
    {
        Debug.Log("=== 적 상태 ===");
        Debug.Log("HP: " + HP + "/" + maxHp);
        Debug.Log("공격 계수: " + attackCoefficient);
        Debug.Log("방어 계수: " + defenseCoefficient);
        Debug.Log("현재 아이템: " + (currentItem != null ? currentItem.GetItemInfo() : "None"));
        Debug.Log("디버그 정보: " + currentItemDebugInfo);
        Debug.Log("=================");
    }
    
    private void OnMouseDown()
    {
        ShowEnemyStatus();
    }
    
    private void Update()
    {
        // E키로 적 상태 확인
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("[E키] 적 currentItem 상태: " + (currentItem != null ? currentItem.GetItemInfo() : "None"));
            Debug.Log("[E키] 적 디버그 정보: " + currentItemDebugInfo);
        }
        
        // currentItem과 currentItemDebugInfo 동기화
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