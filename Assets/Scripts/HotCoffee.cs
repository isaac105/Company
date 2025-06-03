using UnityEngine;
using UnityEngine.UI;

public class HotCoffee : Item
{
    [Header("뜨거운 커피잔 속성")]
    [SerializeField] private float coffeeDamageCoefficient = 1.0f;
    [SerializeField] private bool blockNextDefense = true;
    
    private Button coffeeButton;
    private CombatManager combatManager;
    
    public bool BlockNextDefense 
    { 
        get { return blockNextDefense; } 
        set { blockNextDefense = value; }
    }
    
    protected override void Start()
    {
        DamageCoefficient = coffeeDamageCoefficient;
        
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
        
        Debug.Log("뜨거운 커피잔 초기화 완료: 데미지 계수 " + DamageCoefficient);
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
    
    public override string GetItemInfo()
    {
        return "뜨거운 커피잔 - 데미지 계수: " + DamageCoefficient + " (방어 봉인)";
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