using UnityEngine;
using UnityEngine.UI;

public class ReportBundle : Item
{
    private Button reportBundleButton;
    private CombatManager combatManager;
    
    protected override void Start()
    {
        // 기본 속성 설정
        itemName = "보고서 뭉치";
        damageCoefficient = 1.5f;
        
        // 기본 제공 아이템이므로 특수 효과 없음
        
        // CombatManager 찾기
        combatManager = FindAnyObjectByType<CombatManager>();
        
        reportBundleButton = GetComponent<Button>();
        
        if (reportBundleButton != null)
        {
            reportBundleButton.onClick.AddListener(OnReportBundleClick);
            Debug.Log("리포트 번들 버튼 이벤트 등록 완료");
        }
        else
        {
            Debug.LogWarning("ReportBundle에 Button 컴포넌트가 없습니다!");
        }
        
        Debug.Log("리포트 번들 초기화 완료: " + GetItemInfo());
    }
    
    public void OnReportBundleClick()
    {
        Debug.Log("리포트 번들 클릭됨!");
        
        if (combatManager != null)
        {
            // CombatManager에 아이템 선택 알림 (전투 시작)
            combatManager.OnItemSelected(this, true);
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
        Debug.Log("리포트 번들 사용! 기본 데미지를 줍니다.");
    }
    
    private void OnDestroy()
    {
        if (reportBundleButton != null)
        {
            reportBundleButton.onClick.RemoveListener(OnReportBundleClick);
        }
    }
}