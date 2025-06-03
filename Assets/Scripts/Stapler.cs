using UnityEngine;
using UnityEngine.UI;

public class Stapler : Item
{
    private Button staplerButton;
    private CombatManager combatManager;
    
    protected override void Start()
    {
        // 기본 속성 설정
        itemName = "스테이플러";
        damageCoefficient = 1.0f;
        
        // 특수 효과 추가
        AddEffect(ItemEffectType.DoubleAttack, 2f, "2회 공격");
        
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
        
        Debug.Log("스테이플러 초기화 완료: " + GetItemInfo());
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
    
    public override void UseItem()
    {
        Debug.Log("스테이플러 사용! 2번 연속 공격!");
    }
    
    private void OnDestroy()
    {
        if (staplerButton != null)
        {
            staplerButton.onClick.RemoveListener(OnStaplerClick);
        }
    }
}