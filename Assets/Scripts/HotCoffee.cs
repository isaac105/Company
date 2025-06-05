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
        AddEffect(ItemEffectType.ReduceEnemyDefense, 0.3f, "방어 확률 30% 감소");

        // CombatManager 찾기
        combatManager = FindAnyObjectByType<CombatManager>();
        
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