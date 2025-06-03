using UnityEngine;
using UnityEngine.UI;

public class LargeUSB : Item
{
    [Header("대용량 USB 속성")]
    [SerializeField] private float usbDamageCoefficient = 1.2f;
    [SerializeField] private bool veryHardDefense = true;
    [SerializeField] private float defenseReduction = 0.3f;
    
    private Button usbButton;
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
    
    protected override void Start()
    {
        DamageCoefficient = usbDamageCoefficient;
        
        // CombatManager 찾기
        combatManager = FindObjectOfType<CombatManager>();
        
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
        
        Debug.Log("대용량 USB 초기화 완료: 데미지 계수 " + DamageCoefficient);
    }
    
    public void OnLargeUSBClick()
    {
        Debug.Log("대용량 USB 클릭됨!");
        
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
        return "대용량 USB - 데미지 계수: " + DamageCoefficient + " (방어 어려움)";
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