using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    [Header("UI 패널들")]
    public GameObject itemPanel;
    public GameObject attackTimingPanel;
    public GameObject defenseTimingPanel;
    
    [Header("캐릭터")]
    public PlayerCharacter playerCharacter;
    public AssistantManagerEnemy enemyCharacter;
    
    [Header("UI 텍스트")]
    public Text playerHpText;
    public Text enemyHpText;
    public Text stageText;
    
    private enum State { ItemSelect, AttackTiming, EnemyAttack, DefenseTiming }
    private State currentState;
    private Item selectedItem;
    
    void Start()
    {
        if (playerCharacter == null)
            playerCharacter = FindObjectOfType<PlayerCharacter>();
            
        if (enemyCharacter == null)
            enemyCharacter = FindObjectOfType<AssistantManagerEnemy>();
            
        SetState(State.ItemSelect);
        
        Debug.Log("CombatManager 초기화 완료!");
        Debug.Log("PlayerCharacter: " + (playerCharacter != null ? playerCharacter.gameObject.name : "없음"));
        Debug.Log("AssistantManagerEnemy: " + (enemyCharacter != null ? enemyCharacter.gameObject.name : "없음"));
        
        if (enemyCharacter != null)
        {
            Debug.Log("현재 적: " + enemyCharacter.EnemyName + " (" + enemyCharacter.EnemyRank + ")");
        }
    }
    
    void Update()
    {
        UpdateUI();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentState == State.AttackTiming)
                ExecuteAttack();
            else if (currentState == State.DefenseTiming)
                ExecuteDefense();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCombat();
        }
    }
    
    void SetState(State newState)
    {
        currentState = newState;
        
        if (itemPanel) itemPanel.SetActive(newState == State.ItemSelect);
        if (attackTimingPanel) attackTimingPanel.SetActive(newState == State.AttackTiming);
        if (defenseTimingPanel) defenseTimingPanel.SetActive(newState == State.DefenseTiming);
        
        Debug.Log("전투 상태 변경: " + newState);
    }
    
    public void OnItemSelected(Item item)
    {
        selectedItem = item;
        Debug.Log("아이템 선택됨: " + (item != null ? item.GetItemInfo() : "None"));
        SetState(State.AttackTiming);
    }
    
    void ExecuteAttack()
    {
        if (playerCharacter == null || enemyCharacter == null)
        {
            Debug.LogError("PlayerCharacter 또는 AssistantManagerEnemy가 없습니다!");
            return;
        }
        
        float playerDamage = playerCharacter.CalculateAttackDamage();
        
        if (selectedItem != null)
        {
            playerDamage *= selectedItem.DamageCoefficient;
            
            // 연속공격 효과 확인
            if (selectedItem.HasEffect(ItemEffectType.DoubleAttack))
            {
                int attackCount = (int)selectedItem.GetEffectValue(ItemEffectType.DoubleAttack);
                ExecuteMultipleAttacks(playerDamage, attackCount);
                return;
            }
            
            // 다른 특수 효과 적용
            selectedItem.ApplyItemEffects(playerCharacter, enemyCharacter, this);
        }
            
        enemyCharacter.TakeDamage(playerDamage);
        Debug.Log("플레이어 공격! " + enemyCharacter.EnemyName + "에게 데미지: " + playerDamage);
        
        // 플레이어 턴 종료 - 적의 일회성 효과 리셋
        enemyCharacter.EndTurn();
        
        if (enemyCharacter.HP <= 0)
        {
            Debug.Log(enemyCharacter.EnemyName + " 사망! 승리!");
            SetState(State.ItemSelect);
        }
        else
        {
            SetState(State.DefenseTiming);
        }
    }
    
    // 연속공격 처리 메서드
    void ExecuteMultipleAttacks(float baseDamage, int attackCount)
    {
        Debug.Log("=== 연속공격 시작! 총 " + attackCount + "회 공격 ===");
        
        // 다른 특수 효과들 먼저 적용 (방어 봉인 등)
        if (selectedItem != null)
        {
            selectedItem.ApplyItemEffects(playerCharacter, enemyCharacter, this);
        }
        
        int successfulAttacks = 0;
        float totalDamage = 0f;
        
        for (int i = 0; i < attackCount; i++)
        {
            Debug.Log("--- 공격 " + (i + 1) + "/" + attackCount + " ---");
            
            // 각 공격마다 적이 개별적으로 방어 시도
            if (enemyCharacter.TryDefense())
            {
                Debug.Log(enemyCharacter.EnemyName + "이 " + (i + 1) + "번째 공격을 방어했습니다! (데미지: 0)");
            }
            else
            {
                float finalDamage = baseDamage / enemyCharacter.DefenseCoefficient;
                enemyCharacter.HP -= finalDamage;
                totalDamage += finalDamage;
                successfulAttacks++;
                
                Debug.Log(enemyCharacter.EnemyName + "이 " + (i + 1) + "번째 공격으로 데미지: " + finalDamage + " - 현재 HP: " + enemyCharacter.HP + "/" + enemyCharacter.MaxHP);
            }
            
            // 적이 사망했으면 연속공격 중단
            if (enemyCharacter.HP <= 0)
            {
                Debug.Log(enemyCharacter.EnemyName + " 사망! " + (i + 1) + "번째 공격에서 쓰러졌습니다!");
                break;
            }
        }
        
        Debug.Log("=== 연속공격 완료! 성공한 공격: " + successfulAttacks + "/" + attackCount + ", 총 데미지: " + totalDamage + " ===");
        
        // 플레이어 턴 종료 - 적의 일회성 효과 리셋
        enemyCharacter.EndTurn();
        
        if (enemyCharacter.HP <= 0)
        {
            Debug.Log(enemyCharacter.EnemyName + " 사망! 승리!");
            SetState(State.ItemSelect);
        }
        else
        {
            SetState(State.DefenseTiming);
        }
    }
    
    void ExecuteDefense()
    {
        if (playerCharacter == null || enemyCharacter == null)
        {
            Debug.LogError("PlayerCharacter 또는 AssistantManagerEnemy가 없습니다!");
            return;
        }
        
        float enemyDamage = enemyCharacter.CalculateAttackDamage();
        
        playerCharacter.TakeDamage(enemyDamage);
        Debug.Log(enemyCharacter.EnemyName + " 공격! 받은 데미지: " + enemyDamage);
        
        if (playerCharacter.HP <= 0)
        {
            Debug.Log("플레이어 사망! 패배!");
            SetState(State.ItemSelect);
        }
        else
        {
            SetState(State.ItemSelect);
        }
    }
    
    void UpdateUI()
    {
        if (playerHpText && playerCharacter)
            playerHpText.text = "Player HP: " + playerCharacter.HP.ToString("F0") + "/" + playerCharacter.MaxHP.ToString("F0");
            
        if (enemyHpText && enemyCharacter)
            enemyHpText.text = enemyCharacter.EnemyName + " HP: " + enemyCharacter.HP.ToString("F0") + "/" + enemyCharacter.MaxHP.ToString("F0");
            
        if (stageText)
            stageText.text = "상태: " + GetStateDisplayText() + " | 적: " + (enemyCharacter != null ? enemyCharacter.EnemyRank : "없음");
    }
    
    string GetStateDisplayText()
    {
        switch (currentState)
        {
            case State.ItemSelect: return "아이템 선택";
            case State.AttackTiming: return "공격 타이밍";
            case State.EnemyAttack: return "적 공격 중";
            case State.DefenseTiming: return "방어 타이밍";
            default: return "알 수 없음";
        }
    }
    
    void ResetCombat()
    {
        if (playerCharacter != null)
        {
            playerCharacter.HP = playerCharacter.MaxHP;
        }
        
        if (enemyCharacter != null)
        {
            enemyCharacter.HP = enemyCharacter.MaxHP;
        }
        
        selectedItem = null;
        SetState(State.ItemSelect);
        Debug.Log("전투 리셋! " + (enemyCharacter != null ? enemyCharacter.EnemyName : "대리") + "와의 전투 재시작");
    }
}