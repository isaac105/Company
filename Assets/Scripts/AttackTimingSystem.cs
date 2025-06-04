using UnityEngine;
using UnityEngine.UI;

public class AttackTimingSystem : MonoBehaviour
{
    [Header("타이밍 바 설정")]
    public float barSpeed = 5f; // 조준선 이동 속도
    public float barWidth = 800f; // 타이밍 바 전체 너비
    
    [Header("타이밍 존 설정")]
    public RectTransform goodZone;
    public RectTransform greatZone;
    public RectTransform perfectZone;
    public RectTransform indicator; // 조준선
    
    [Header("데미지 계수")]
    public float goodDamageMultiplier = 1.0f;
    public float greatDamageMultiplier = 1.5f;
    public float perfectDamageMultiplier = 2.0f;
    
    private float currentPosition;
    private bool isMoving = true;
    private bool isMovingRight = true;
    
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
        UpdateIndicatorPosition();
    }
    
    void CheckTiming()
    {
        isMoving = false;
        float multiplier = 1.0f;
        string result = "Miss";
        
        // 각 존에 대한 판정
        if (IsInZone(perfectZone))
        {
            multiplier = perfectDamageMultiplier;
            result = "Perfect!";
        }
        else if (IsInZone(greatZone))
        {
            multiplier = greatDamageMultiplier;
            result = "Great!";
        }
        else if (IsInZone(goodZone))
        {
            multiplier = goodDamageMultiplier;
            result = "Good";
        }
        
        Debug.Log($"타이밍 결과: {result} (데미지 배율: x{multiplier})");
        
        // CombatManager에 결과 전달
        var combatManager = FindObjectOfType<CombatManager>();
        if (combatManager != null)
        {
            // TODO: CombatManager에 데미지 배율 적용 메서드 추가
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
        UpdateIndicatorPosition();
    }
} 