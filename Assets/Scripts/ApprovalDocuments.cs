using UnityEngine;
using UnityEngine.UI;

public class ApprovalDocuments : Item
{
    private Button documentsButton;
    private CombatManager combatManager;
    
    protected override void Start()
    {
        // 기본 속성 설정
        itemName = "결재 서류철 (미결재)";
        damageCoefficient = 3.0f;
        
        // 특수 효과 추가
        AddEffect(ItemEffectType.HighDamage, 3.0f, "고데미지");
        
        // CombatManager 찾기
        combatManager = FindAnyObjectByType<CombatManager>();
        
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
        
        Debug.Log("결재 서류철 초기화 완료: " + GetItemInfo());
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
        PlayerCharacter player = FindAnyObjectByType<PlayerCharacter>();
        if (player != null)
        {
            player.EquipItem(this);
        }
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