using UnityEngine;
using UnityEngine.UI;

public class ReportBundle : Item
{
    [Header("리포트 번들 속성")]
    [SerializeField] private float reportBundleDamageCoefficient = 1.5f;
    
    private Button reportBundleButton;
    private CombatManager combatManager;
    
    protected override void Start()
    {
        DamageCoefficient = reportBundleDamageCoefficient;
        
        // CombatManager 찾기
        combatManager = FindObjectOfType<CombatManager>();
        
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
        
        Debug.Log("리포트 번들 초기화 완료: 데미지 계수 " + DamageCoefficient);
    }
    
    public void OnReportBundleClick()
    {
        Debug.Log("리포트 번들 클릭됨!");
        
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
        return "리포트 번들 - 데미지 계수: " + DamageCoefficient;
    }
    
    public override void UseItem()
    {
        Debug.Log("리포트 번들 사용! 데미지 계수: " + DamageCoefficient);
    }
    
    private void OnDestroy()
    {
        if (reportBundleButton != null)
        {
            reportBundleButton.onClick.RemoveListener(OnReportBundleClick);
        }
    }
}