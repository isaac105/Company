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
            playerDamage *= selectedItem.DamageCoefficient;
            
        enemyCharacter.TakeDamage(playerDamage);
        Debug.Log("플레이어 공격! " + enemyCharacter.EnemyName + "에게 데미지: " + playerDamage);
        
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