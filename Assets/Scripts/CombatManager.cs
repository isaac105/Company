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
    
    [Header("적 정보")]
    public float enemyMaxHP = 100f;
    public float enemyCurrentHP;
    
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
            
        enemyCurrentHP = enemyMaxHP;
        SetState(State.ItemSelect);
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
    }
    
    void SetState(State newState)
    {
        currentState = newState;
        
        if (itemPanel) itemPanel.SetActive(newState == State.ItemSelect);
        if (attackTimingPanel) attackTimingPanel.SetActive(newState == State.AttackTiming);
        if (defenseTimingPanel) defenseTimingPanel.SetActive(newState == State.DefenseTiming);
        
        Debug.Log("상태 변경: " + newState);
    }
    
    public void OnItemSelected(Item item)
    {
        selectedItem = item;
        SetState(State.AttackTiming);
    }
    
    void ExecuteAttack()
    {
        float damage = playerCharacter.Damage;
        if (selectedItem != null)
            damage *= selectedItem.DamageCoefficient;
            
        enemyCurrentHP -= damage;
        Debug.Log("공격! 데미지: " + damage);
        
        if (enemyCurrentHP <= 0)
        {
            Debug.Log("승리!");
        }
        else
        {
            SetState(State.DefenseTiming);
        }
    }
    
    void ExecuteDefense()
    {
        float damage = 15f;
        if (playerCharacter != null)
            playerCharacter.TakeDamage(damage);
            
        Debug.Log("방어! 받은 데미지: " + damage);
        
        if (playerCharacter.HP <= 0)
        {
            Debug.Log("패배!");
        }
        else
        {
            SetState(State.ItemSelect);
        }
    }
    
    void UpdateUI()
    {
        if (playerHpText && playerCharacter)
            playerHpText.text = "Player HP: " + playerCharacter.HP;
            
        if (enemyHpText)
            enemyHpText.text = "Enemy HP: " + enemyCurrentHP;
            
        if (stageText)
            stageText.text = "State: " + currentState;
    }
}