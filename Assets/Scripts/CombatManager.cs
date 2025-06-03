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
    public EnemyCharacter enemyCharacter;
    
    [Header("UI 텍스트")]
    public Text playerHpText;
    public Text enemyHpText;
    public Text stageText;
    
    private enum State { ItemSelect, AttackTiming, EnemyAttack, DefenseTiming }
    private State currentState;
    private Item selectedItem;
    
    void Start()
    {
        // 자동으로 캐릭터들 찾기
        if (playerCharacter == null)
            playerCharacter = FindObjectOfType<PlayerCharacter>();
            
        if (enemyCharacter == null)
            enemyCharacter = FindObjectOfType<EnemyCharacter>();
            
        SetState(State.ItemSelect);
        
        Debug.Log("CombatManager 초기화 완료!");
        Debug.Log("PlayerCharacter: " + (playerCharacter != null ? playerCharacter.gameObject.name : "없음"));
        Debug.Log("EnemyCharacter: " + (enemyCharacter != null ? enemyCharacter.gameObject.name : "없음"));
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
        
        // R키로 전투 리셋
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
            Debug.LogError("PlayerCharacter 또는 EnemyCharacter가 없습니다!");
            return;
        }
        
        // 플레이어의 공격 데미지 계산
        float playerDamage = playerCharacter.CalculateAttackDamage();
        
        // 선택된 아이템 보너스 적용
        if (selectedItem != null)
            playerDamage *= selectedItem.DamageCoefficient;
            
        // 적에게 데미지 적용
        enemyCharacter.TakeDamage(playerDamage);
        Debug.Log("플레이어 공격! 데미지: " + playerDamage);
        
        // 적 사망 체크
        if (enemyCharacter.HP <= 0)
        {
            Debug.Log("적 사망! 승리!");
            SetState(State.ItemSelect); // 승리 후 아이템 선택으로 돌아감
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
            Debug.LogError("PlayerCharacter 또는 EnemyCharacter가 없습니다!");
            return;
        }
        
        // 적의 공격 데미지 계산
        float enemyDamage = enemyCharacter.CalculateAttackDamage();
        
        // 플레이어에게 데미지 적용
        playerCharacter.TakeDamage(enemyDamage);
        Debug.Log("적 공격! 받은 데미지: " + enemyDamage);
        
        // 플레이어 사망 체크
        if (playerCharacter.HP <= 0)
        {
            Debug.Log("플레이어 사망! 패배!");
            SetState(State.ItemSelect); // 패배 후 아이템 선택으로 돌아감
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
            enemyHpText.text = "Enemy HP: " + enemyCharacter.HP.ToString("F0") + "/" + enemyCharacter.MaxHP.ToString("F0");
            
        if (stageText)
            stageText.text = "상태: " + GetStateDisplayText();
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
        Debug.Log("전투 리셋!");
    }
}