using UnityEngine;
using UnityEngine.UI;

public class ReportBundle : Item
{
    [Header("리포트 번들 속성")]
    [SerializeField] private float reportBundleDamageCoefficient = 1.5f;
    
    [Header("테스트용")]
    [SerializeField] private PlayerCharacter targetPlayer; // Inspector에서 직접 설정용
    
    private Button reportBundleButton;
    
    protected override void Start()
    {
        DamageCoefficient = reportBundleDamageCoefficient;
        
        // 타겟 플레이어를 자동으로 찾기
        if (targetPlayer == null)
        {
            targetPlayer = FindObjectOfType<PlayerCharacter>();
        }
        
        reportBundleButton = GetComponent<Button>();
        
        if (reportBundleButton != null)
        {
            reportBundleButton.onClick.AddListener(OnReportBundleClick);
            Debug.Log("버튼 이벤트 등록 완료");
        }
        else
        {
            Debug.LogWarning("Button 컴포넌트가 없습니다!");
        }
        
        Debug.Log("ReportBundle 초기화 완료. 타겟 플레이어: " + (targetPlayer != null ? targetPlayer.gameObject.name : "없음"));
    }
    
    public void OnReportBundleClick()
    {
        Debug.Log("=== 리포트 번들 클릭 시작 ===");
        
        if (targetPlayer == null)
        {
            Debug.LogError("타겟 플레이어가 없습니다!");
            targetPlayer = FindObjectOfType<PlayerCharacter>();
            Debug.Log("다시 찾은 플레이어: " + (targetPlayer != null ? targetPlayer.gameObject.name : "없음"));
        }
        
        if (targetPlayer != null)
        {
            Debug.Log("플레이어 찾음: " + targetPlayer.gameObject.name);
            
            // EquipItem 메서드도 호출
            targetPlayer.EquipItem(this);
            Debug.Log("EquipItem 메서드 호출 완료!");
            
            // 즉시 확인
            if (targetPlayer.CurrentItem != null)
            {
                Debug.Log("설정 성공! 현재 아이템: " + targetPlayer.CurrentItem.gameObject.name);
            }
            else
            {
                Debug.LogError("설정 실패! currentItem이 여전히 null입니다!");
            }
        }
        else
        {
            Debug.LogError("PlayerCharacter를 찾을 수 없습니다!");
        }
        
        Debug.Log("=== 리포트 번들 클릭 종료 ===");
    }
    
    // 수동 테스트용 메서드 (키보드로 호출)
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[T키] 수동 테스트 시작");
            OnReportBundleClick();
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