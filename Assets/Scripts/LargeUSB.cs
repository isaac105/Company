using UnityEngine;
using UnityEngine.UI;

public class LargeUSB : Item
{
    private Button usbButton;
    private CombatManager combatManager;
    
    protected override void Start()
    {
        // 기본 속성 설정
        itemName = "대용량 USB";
        damageCoefficient = 1.2f;
        
        // 특수 효과 추가
        AddEffect(ItemEffectType.ReduceEnemyDefense, 0.3f, "방어력 30% 감소");
        
        // CombatManager 찾기
        combatManager = FindAnyObjectByType<CombatManager>();
        
        usbButton = GetComponent<Button>();
        
        if (usbButton != null)
        {
            usbButton.onClick.AddListener(OnLargeUSBClick);
            Debug.Log("대용량 USB 버튼 이벤트 등록 완료");
        }
        else
        {
            Debug.LogWarning("LargeUSB에 Button 컴포넌트가 없습니다!");
        }
        
        Debug.Log("대용량 USB 초기화 완료: " + GetItemInfo());
    }
    
    public void OnLargeUSBClick()
    {
        Debug.Log("대용량 USB 클릭됨!");
        
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
        // ItemSelectionManager에도 선택 알림
        var itemSelector = FindFirstObjectByType<ItemSelectionManager>();
        if (itemSelector != null)
        {
            itemSelector.SelectItemByMouse(this);
        }
    }
    
    public override void UseItem()
    {
        Debug.Log("대용량 USB 사용! 적의 방어력을 크게 감소시킵니다!");
    }
    
    private void OnDestroy()
    {
        if (usbButton != null)
        {
            usbButton.onClick.RemoveListener(OnLargeUSBClick);
        }
    }
}