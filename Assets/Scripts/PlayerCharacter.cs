using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [Header("플레이어 기본 속성")]
    [SerializeField] private float hp;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackCoefficient;
    [SerializeField] private float defenseCoefficient;
    [SerializeField] private Item currentItem;

    [Header("최대 HP 설정")]
    [SerializeField] private float maxHp;

    [Header("이미지 관리")]
    [SerializeField] private CharacterImageManager imageManager;

    [Header("디버그 정보")]
    [SerializeField] private string currentItemDebugInfo = "None";

    [Header("방어 시스템")]
    private bool defenseSuccess = false;

    public float HP
    {
        get { return hp; }
        set 
        { 
            hp = Mathf.Clamp(value, 0f, maxHp);
            CheckHPState();
        }
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
            
            Debug.Log("CurrentItem 프로퍼티 설정됨: " + (value != null ? value.GetItemInfo() : "null"));
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
    
    public bool DefenseSuccess
    {
        get { return defenseSuccess; }
        set { defenseSuccess = value; }
    }
    
    void Start()
    {
        if (maxHp <= 0) maxHp = 100f;
        if (hp <= 0) hp = maxHp;
        if (attackCoefficient <= 0) attackCoefficient = 1.0f;
        if (defenseCoefficient <= 0) defenseCoefficient = 1.0f;
        
        if (imageManager == null)
        {
            imageManager = GetComponent<CharacterImageManager>();
        }
        
        Debug.Log("플레이어 캐릭터 초기화 완료: HP " + hp + "/" + maxHp);
    }
    
    public bool TryTakeDamage(float damage)
    {
        // 방어 성공 여부 확인
        if (defenseSuccess)
        {
            Debug.Log("플레이어가 공격을 완전히 방어했습니다!");
            return false;
        }
        
        float finalDamage = damage / defenseCoefficient;
        HP -= finalDamage;
        
        // 데미지를 받을 때 회피 모션 표시
        if (imageManager != null)
        {
            imageManager.ShowDodgeSprite();
        }
        
        Debug.Log("데미지 받음: " + finalDamage + " - 현재 HP: " + HP + "/" + maxHp);
        
        if (HP <= 0)
        {
            OnPlayerDeath();
        }
        
        return true;
    }
    
    private void CheckHPState()
    {
        if (imageManager != null)
        {
            // HP가 20% 이하일 때 화난 상태로 변경
            imageManager.SetAngryState(HP <= maxHp * 0.2f);
        }
    }
    
    public float CalculateAttackDamage()
    {
        // 공격할 때 던지기 모션 표시
        if (imageManager != null)
        {
            imageManager.ShowThrowAnimation();
        }

        float itemDamageBonus = currentItem != null ? currentItem.DamageCoefficient : 1.0f;
        float finalDamage = damage * attackCoefficient * itemDamageBonus;
        
        Debug.Log("공격 데미지 계산: " + finalDamage);
        return finalDamage;
    }
    
    public void EquipItem(Item newItem)
    {
        Debug.Log("EquipItem 호출됨. 새 아이템: " + (newItem != null ? newItem.GetItemInfo() : "null"));
        
        // 직접 필드에 할당하고 디버그 정보도 업데이트
        currentItem = newItem;
        
        if (currentItem != null)
        {
            currentItemDebugInfo = currentItem.gameObject.name + " (데미지: " + currentItem.DamageCoefficient + ")";
            Debug.Log("아이템 장착 완료: " + currentItem.GetItemInfo());
        }
        else
        {
            currentItemDebugInfo = "None";
            Debug.Log("아이템 장착: None");
        }
        
        ShowPlayerStatus();
    }
    
    public void Heal(float healAmount)
    {
        HP += healAmount;
        Debug.Log("HP 회복: " + healAmount + " - 현재 HP: " + HP + "/" + maxHp);
    }
    
    private void OnPlayerDeath()
    {
        Debug.Log("플레이어가 사망했습니다!");
    }
    
    public void ShowPlayerStatus()
    {
        Debug.Log("=== 플레이어 상태 ===");
        Debug.Log("HP: " + HP + "/" + maxHp);
        Debug.Log("공격 계수: " + attackCoefficient);
        Debug.Log("방어 계수: " + defenseCoefficient);
        Debug.Log("현재 아이템: " + (currentItem != null ? currentItem.GetItemInfo() : "None"));
        Debug.Log("디버그 정보: " + currentItemDebugInfo);
        Debug.Log("=========================");
    }
    
    private void OnMouseDown()
    {
        ShowPlayerStatus();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("[스페이스바] currentItem 상태: " + (currentItem != null ? currentItem.GetItemInfo() : "None"));
            Debug.Log("[스페이스바] 디버그 정보: " + currentItemDebugInfo);
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