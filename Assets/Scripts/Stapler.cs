using UnityEngine;
using UnityEngine.UI;

public class Stapler : Item
{
    [Header("스테이플러 속성")]
    [SerializeField] private float staplerDamageCoefficient = 1.0f;
    [SerializeField] private bool doubleAttack = true;
    [SerializeField] private int attackCount = 2;
    
    private Button staplerButton;
    private CombatManager combatManager;
    
    public bool DoubleAttack 
    { 
        get { return doubleAttack; } 
        set { doubleAttack = value; }
    }
    
    public int AttackCount 
    { 
        get { return attackCount; } 
        set { attackCount = value; }
    }
    
    protected override void Start()
    {
        DamageCoefficient = staplerDamageCoefficient;
        
        // CombatManager 찾기
        combatManager = FindObjectOfType<CombatManager>();
        
        staplerButton = GetComponent<Button>();
        
        if (staplerButton != null)
        {
            staplerButton.onClick.AddListener(OnStaplerClick);
            Debug.Log("스테이플러 버튼 이벤트 등록 완료");
        }
        else
        {
            Debug.LogWarning("Stapler에 Button 컴포넌트가 없습니다!");
        }
        
        Debug.Log("스테이플러 초기화 완료: 데미지 계수 " + DamageCoefficient);
    }
    
    public void OnStaplerClick()
    {
        Debug.Log("스테이플러 클릭됨!");
        
        if (combatManager != null)
        {
            // CombatManager에 아이템 선택 알림
            combatManager.OnItemSelected(this);
            Debug.Log("CombatManager에 아이템 선택 전달: " + GetItemInfo());
        }
        else
        {
            Debug.LogError("CombatManager를 찾을 수 없습니다!");
        }
        
        // PlayerCharacter에도 장착
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        if (player != null)
        {
            player.EquipItem(this);
        }
    }
    
    public override string GetItemInfo()
    {
        return "스테이플러 - 데미지 계수: " + DamageCoefficient + " (" + attackCount + "회 공격)";
    }
    
    public override void UseItem()
    {
        Debug.Log("스테이플러 사용! " + attackCount + "번 연속 공격!");
    }
    
    private void OnDestroy()
    {
        if (staplerButton != null)
        {
            staplerButton.onClick.RemoveListener(OnStaplerClick);
        }
    }
}