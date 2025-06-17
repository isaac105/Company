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
    
    [Header("방어 시스템")]
    [SerializeField] protected float baseDefenseChance = 0.3f; // 기본 30% 방어 확률
    [SerializeField] protected bool isDefenseBlocked = false; // 방어 봉인 상태
    [SerializeField] protected float defenseReduction = 0f; // 방어 확률 감소량

    [Header("이미지 관리")]
    [SerializeField] protected CharacterImageManager imageManager;

    [Header("디버그 정보")]
    [SerializeField] private string currentItemDebugInfo = "None";
    [SerializeField] private bool lastActionWasDefense = false;

    [Header("HP UI")]
    [SerializeField] protected GameObject hpUIPrefab; // Inspector에서 할당할 HP UI 프리팹
    protected HpDisplay enemyHpDisplay; // HP 표시 스크립트 참조

    public float HP
    {
        get { return hp; }
        set 
        { 
            hp = Mathf.Clamp(value, 0f, maxHp);
            CheckHPState();
            // HP 변경 시 UI 업데이트
            if (enemyHpDisplay != null)
            {
                enemyHpDisplay.UpdateHpImage(hp, maxHp);
            }
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
    
    public float BaseDefenseChance
    {
        get { return baseDefenseChance; }
        set { baseDefenseChance = Mathf.Clamp01(value); }
    }
    
    public bool IsDefenseBlocked
    {
        get { return isDefenseBlocked; }
        set { isDefenseBlocked = value; }
    }
    
    public bool LastActionWasDefense
    {
        get { return lastActionWasDefense; }
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
        if (baseDefenseChance <= 0) baseDefenseChance = 0.3f; // 기본 30%
        
        if (imageManager == null)
        {
            imageManager = GetComponent<CharacterImageManager>();
        }

        // HP UI 프리팹 인스턴스화 및 연결
        if (hpUIPrefab != null)
        {
            GameObject hpUIInstance = Instantiate(hpUIPrefab, transform); // 적의 자식으로 생성
            enemyHpDisplay = hpUIInstance.GetComponent<HpDisplay>();
            if (enemyHpDisplay != null)
            {
                // HP UI 위치 조정 (적 캐릭터 위쪽으로)
                hpUIInstance.transform.localPosition = new Vector3(0, 220, 0); // 예시 위치
                enemyHpDisplay.UpdateHpImage(hp, maxHp); // 초기 HP 상태 반영
            }
        }
        
        Debug.Log(enemyName + " (" + enemyRank + ") 초기화 완료: HP " + hp + "/" + maxHp + ", 방어 확률: " + (baseDefenseChance * 100) + "%");
    }
    
    public virtual bool TryDefense()
    {
        lastActionWasDefense = false;
        
        if (isDefenseBlocked)
        {
            Debug.Log(enemyName + "의 방어가 봉인되어 있습니다!");
            return false;
        }
        
        float finalDefenseChance = baseDefenseChance * (1f - defenseReduction);
        finalDefenseChance = Mathf.Clamp01(finalDefenseChance);
        
        float roll = Random.Range(0f, 1f);
        bool defended = roll < finalDefenseChance;
        
        if (defended)
        {
            lastActionWasDefense = true;
            if (imageManager != null)
            {
                imageManager.ShowDodgeSprite();
            }
            Debug.Log(enemyName + "이 방어에 성공했습니다! (확률: " + (finalDefenseChance * 100).ToString("F1") + "%)");
        }
        else
        {
            Debug.Log(enemyName + "의 방어 실패 (확률: " + (finalDefenseChance * 100).ToString("F1") + "%)");
        }
        
        return defended;
    }
    
    public virtual bool TryTakeDamage(float damage)
    {
        // 방어 시도
        if (TryDefense())
        {
            Debug.Log(enemyName + "이 공격을 완전히 방어했습니다!");
            return false;
        }
        
        // 방어 실패 시 데미지 계산
        float finalDamage = damage / defenseCoefficient;
        HP -= finalDamage;
        
        // 데미지를 받을 때 회피 모션 표시
        if (imageManager != null)
        {
            imageManager.ShowDodgeSprite();
        }
        
        Debug.Log(enemyName + "이 데미지 받음: " + finalDamage + " - 현재 HP: " + HP + "/" + maxHp);
        
        if (HP <= 0)
        {
            OnEnemyDeath();
        }
        
        return true;
    }
    
    // 아이템 효과로 방어 봉인
    public virtual void BlockNextDefense()
    {
        isDefenseBlocked = true;
        Debug.Log(enemyName + "의 다음 방어가 봉인되었습니다!");
    }
    
    // 아이템 효과로 방어력 감소
    public virtual void ReduceDefenseChance(float reductionAmount)
    {
        defenseReduction = Mathf.Clamp01(reductionAmount);
        Debug.Log(enemyName + "의 방어 확률이 " + (reductionAmount * 100) + "% 감소됩니다!");
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
        Debug.Log("방어 확률: " + (baseDefenseChance * 100) + "%");
        Debug.Log("방어 봉인: " + (isDefenseBlocked ? "예" : "아니오"));
        Debug.Log("방어력 감소: " + (defenseReduction * 100) + "%");
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

    private void CheckHPState()
    {
        if (imageManager != null)
        {
            // HP가 30% 이하일 때 화난 상태로 변경
            imageManager.SetAngryState(HP <= maxHp * 0.3f);
        }
    }

    public virtual float CalculateAttackDamage()
    {
        // 공격할 때 던지기 모션 표시
        if (imageManager != null)
        {
            imageManager.ShowThrowAnimation();
        }

        float itemDamageBonus = currentItem != null ? currentItem.DamageCoefficient : 1.0f;
        float finalDamage = damage * attackCoefficient * itemDamageBonus;
        
        Debug.Log(enemyName + " 공격 데미지 계산: " + finalDamage);
        return finalDamage;
    }

    public virtual void EndTurn()
    {
        if (isDefenseBlocked)
        {
            isDefenseBlocked = false;
            Debug.Log(enemyName + "의 방어 봉인이 해제되었습니다.");
        }
        
        if (defenseReduction > 0)
        {
            defenseReduction = 0f;
            Debug.Log(enemyName + "의 방어력 감소 효과가 해제되었습니다.");
        }
    }
}