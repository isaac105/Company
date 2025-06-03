using UnityEngine;
using UnityEngine.UI;

public class ApprovalDocuments : Item
{
    [Header("결재 서류철 속성")]
    [SerializeField] private float documentsDamageCoefficient = 3.0f;
    [SerializeField] private bool veryHardDefense = true;
    [SerializeField] private float defenseReduction = 0.2f;
    [SerializeField] private bool highDamage = true;
    
    private Button documentsButton;
    private CombatManager combatManager;
    
    public bool VeryHardDefense 
    { 
        get { return veryHardDefense; } 
        set { veryHardDefense = value; }
    }
    
    public float DefenseReduction 
    { 
        get { return defenseReduction; } 
        set { defenseReduction = value; }
    }
    
    public bool HighDamage 
    { 
        get { return highDamage; } 
        set { highDamage = value; }
    }
    
    protected override void Start()
    {
        DamageCoefficient = documentsDamageCoefficient;
        
        // CombatManager 찾기
        combatManager = FindObjectOfType<CombatManager>();
        
        documentsButton = GetComponent<Button>();
        
        if (documentsButton != null)
        {
            documentsButton.onClick.AddListener(OnApprovalDocumentsClick);
            Debug.Log("결재 서류철 버튼 이벤트 등록 완료");
        }
        else
        {
            Debug.LogWarning("ApprovalDocuments에 Button 컴포넌트가 없습니다!");
        }
        
        Debug.Log("결재 서류철 초기화 완료: 데미지 계수 " + DamageCoefficient);
    }
    
    public void OnApprovalDocumentsClick()
    {
        Debug.Log("결재 서류철 클릭됨!");
        
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
        return "결재 서류철 (미결재) - 데미지 계수: " + DamageCoefficient + " (최강 무기)";
    }
    
    public override void UseItem()
    {
        Debug.Log("결재 서류철 사용! 최강의 공격력과 방어 무력화!");
    }
    
    private void OnDestroy()
    {
        if (documentsButton != null)
        {
            documentsButton.onClick.RemoveListener(OnApprovalDocumentsClick);
        }
    }
}