using UnityEngine;
using UnityEngine.UI;

public class DefenseTimingSystem : MonoBehaviour
{
    [Header("타이밍 바 설정")]
    public float barSpeed = 7f; // 조준선 이동 속도 (공격보다 빠르게)
    public float barWidth = 800f; // 타이밍 바 전체 너비
    
    [Header("타이밍 존 설정")]
    public RectTransform defenseZone; // 방어 성공 구간
    public RectTransform indicator; // 조준선
    
    [Header("방어 시스템 설정")]
    public float perfectDefenseBonus = 0.3f; // Perfect 방어 시 추가 방어 확률
    
    private float currentPosition;
    private bool isMoving = true;
    private bool isMovingRight = true;
    private bool defenseSuccess = false;
    
    void Start()
    {
        // 조준선 초기 위치 설정
        currentPosition = -barWidth / 2;
        UpdateIndicatorPosition();
    }
    
    void Update()
    {
        if (!isMoving) return;
        
        // 조준선 이동
        if (isMovingRight)
        {
            currentPosition += barSpeed * Time.deltaTime * 100f;
            if (currentPosition >= barWidth / 2)
            {
                currentPosition = barWidth / 2;
                isMovingRight = false;
            }
        }
        else
        {
            currentPosition -= barSpeed * Time.deltaTime * 100f;
            if (currentPosition <= -barWidth / 2)
            {
                currentPosition = -barWidth / 2;
                isMovingRight = true;
            }
        }
        
        UpdateIndicatorPosition();
        
        // 스페이스바로 타이밍 체크
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckTiming();
        }
    }
    
    void UpdateIndicatorPosition()
    {
        if (indicator != null)
        {
            indicator.anchoredPosition = new Vector2(currentPosition, 0);
        }
    }
    
    public void StartTimingGame()
    {
        isMoving = true;
        currentPosition = -barWidth / 2;
        isMovingRight = true;
        defenseSuccess = false;
        UpdateIndicatorPosition();
    }
    
    public void CheckTiming()
    {
        isMoving = false;
        string result = "Miss";
        float bonusDefense = 0f;
        
        // 방어 존에 대한 판정
        if (IsInZone(defenseZone))
        {
            defenseSuccess = true;
            bonusDefense = perfectDefenseBonus;
            result = "Perfect Defense!";
        }
        
        Debug.Log($"방어 타이밍 결과: {result} (추가 방어 확률: +{bonusDefense * 100}%)");
        
        // CombatManager에 결과 전달
        var combatManager = FindAnyObjectByType<CombatManager>();
        if (combatManager != null)
        {
            combatManager.SetDefenseResult(defenseSuccess, bonusDefense);
        }
    }
    
    bool IsInZone(RectTransform zone)
    {
        if (zone == null) return false;
        
        float zoneLeft = zone.anchoredPosition.x - zone.rect.width / 2;
        float zoneRight = zone.anchoredPosition.x + zone.rect.width / 2;
        
        return currentPosition >= zoneLeft && currentPosition <= zoneRight;
    }
    
    public void Reset()
    {
        isMoving = false;
        currentPosition = -barWidth / 2;
        defenseSuccess = false;
        UpdateIndicatorPosition();
    }
} 