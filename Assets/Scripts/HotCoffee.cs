using UnityEngine;
using UnityEngine.UI;

public class HotCoffee : Item
{
    private Button coffeeButton;
    private CombatManager combatManager;
    
    protected override void Start()
    {
        // 기본 속성 설정
        itemName = "뜨거운 커피잔";
        damageCoefficient = 1.0f;
        
        // 특수 효과 추가
        AddEffect(ItemEffectType.BlockNextDefense, 1f, "방어 봉인");
        
        // CombatManager 찾기
        combatManager = FindObjectOfType<CombatManager>();
        
        coffeeButton = GetComponent<Button>();
        
        if (coffeeButton != null)
        {
            coffeeButton.onClick.AddListener(OnHotCoffeeClick);
            Debug.Log("뜨거운 커피잔 버튼 이벤트 등록 완료");
        }
        else
        {
            Debug.LogWarning("HotCoffee에 Button 컴포넌트가 없습니다!");
        }
        
        Debug.Log("뜨거운 커피잔 초기화 완료: " + GetItemInfo());
    }
    
    public void OnHotCoffeeClick()
    {
        Debug.Log("뜨거운 커피잔 클릭됨!");
        
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
        Debug.Log("뜨거운 커피잔 사용! 적의 다음 방어를 봉인합니다!");
    }
    
    private void OnDestroy()
    {
        if (coffeeButton != null)
        {
            coffeeButton.onClick.RemoveListener(OnHotCoffeeClick);
        }
    }
}