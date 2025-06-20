using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CombatManager : MonoBehaviour
{
    [Header("UI 패널들")]
    public GameObject itemPanel;
    public GameObject attackTimingPanel;
    public GameObject defenseTimingPanel;
    public Canvas gameCanvas; // UI Canvas 참조 추가
    
    [Header("UI 버튼")]
    public Button attackButton;  // 공격 버튼
    public Button defenseButton; // 방어 버튼
    
    [Header("캐릭터")]
    public PlayerCharacter playerCharacter;
    public EnemyCharacter enemyCharacter;
    
    [Header("UI 텍스트")]
    public Text playerHpText;
    public Text enemyHpText;
    public Text stageText;
    public Text itemTitleText; // 아이템 제목 텍스트
    public Text itemEffectDescriptionText; // 아이템 효과 설명 텍스트
    
    [Header("전투 이펙트")]
    public GameObject damageTextPrefab;  // 데미지 텍스트 프리팹
    public float damageTextDuration = 1f;  // 데미지 텍스트 표시 시간
    public float damageTextRiseDistance = 100f;  // 데미지 텍스트가 올라가는 거리
    
    [Header("아이템 날라가기 효과")]
    public float itemFlyDuration = 0.5f;  // 아이템이 날아가는 시간
    public float itemArcHeight = 200f;  // 포물선 높이
    
    [Header("타이밍 시스템")]
    public AttackTimingSystem attackTimingSystem;
    public DefenseTimingSystem defenseTimingSystem;
    private float currentDamageMultiplier = 1.0f;
    private bool defenseSuccess = false;
    private float defenseBonus = 0f;
    
    [Header("스테이지 시스템")]
    public StageManager stageManager;
    
    [Header("BGM 설정")]
    private BGMManager bgmManager;
    
    [Header("효과음 설정")]
    public AudioClip sfxItemFly;
    public AudioClip sfxHit;
    public AudioClip sfxPlayerOh; // 플레이어 "오!" 효과음 추가
    public AudioClip sfxTting;    // 방어 성공 효과음 추가
    private AudioSource sfxAudioSource;
    
    public enum State { ItemSelect, AttackTiming, EnemyAttack, DefenseTiming }
    private State currentState;
    private Item selectedItem;
    
    private GameEndManager gameEndManager;
    
    void Start()
    {
        if (playerCharacter == null)
            playerCharacter = FindFirstObjectByType<PlayerCharacter>();
            
        if (enemyCharacter == null)
            enemyCharacter = FindFirstObjectByType<EnemyCharacter>();
            
        if (attackTimingSystem == null)
            attackTimingSystem = attackTimingPanel.GetComponent<AttackTimingSystem>();
            
        if (defenseTimingSystem == null)
            defenseTimingSystem = defenseTimingPanel.GetComponent<DefenseTimingSystem>();
            
        if (stageManager == null)
            stageManager = FindFirstObjectByType<StageManager>();
            
        // BGM 매니저 찾기
        bgmManager = FindFirstObjectByType<BGMManager>();
            
        // 버튼 이벤트 등록
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(OnAttackButtonClick);
            Debug.Log("공격 버튼 이벤트 등록 완료");
        }
        else
        {
            Debug.LogWarning("공격 버튼이 할당되지 않았습니다!");
        }
        
        if (defenseButton != null)
        {
            defenseButton.onClick.AddListener(OnDefenseButtonClick);
            Debug.Log("방어 버튼 이벤트 등록 완료");
        }
        else
        {
            Debug.LogWarning("방어 버튼이 할당되지 않았습니다!");
        }
            
        SetState(State.ItemSelect);
        
        Debug.Log("CombatManager 초기화 완료!");
        Debug.Log("PlayerCharacter: " + (playerCharacter != null ? playerCharacter.gameObject.name : "없음"));
        Debug.Log("EnemyCharacter: " + (enemyCharacter != null ? enemyCharacter.gameObject.name : "없음"));
        
        if (enemyCharacter != null)
        {
            Debug.Log("현재 적: " + enemyCharacter.EnemyName + " (" + enemyCharacter.EnemyRank + ")");
        }
        
        gameEndManager = FindFirstObjectByType<GameEndManager>();
        
        // 효과음용 AudioSource 추가
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
        sfxAudioSource.playOnAwake = false;
        sfxAudioSource.loop = false;
    }
    
    public void SetEnemy(EnemyCharacter newEnemy)
    {
        enemyCharacter = newEnemy;
        UpdateUI();
    }
    
    void Update()
    {
        // 게임 종료 시 모든 입력 무시
        if (gameEndManager != null && gameEndManager.IsGameEnded())
            return;
        
        UpdateUI();
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentState == State.AttackTiming)
                ExecuteAttack();
            else if (currentState == State.DefenseTiming)
                ExecuteDefense();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetCombat();
        }
    }
    
    public void SetState(State newState)
    {
        // 게임 종료 시 상태 변경 금지
        if (gameEndManager != null && gameEndManager.IsGameEnded())
            return;
        
        currentState = newState;
        
        // BGM 상태 변경
        if (bgmManager != null)
        {
            switch (newState)
            {
                case State.AttackTiming:
                case State.DefenseTiming:
                case State.ItemSelect:
                    bgmManager.PlayBGM("Battle");
                    break;
            }
        }
        
        if (itemPanel) itemPanel.SetActive(newState == State.ItemSelect);
        if (attackTimingPanel) 
        {
            attackTimingPanel.SetActive(newState == State.AttackTiming);
            if (newState == State.AttackTiming && attackTimingSystem != null)
            {
                attackTimingSystem.StartTimingGame();
            }
        }
        if (defenseTimingPanel) 
        {
            defenseTimingPanel.SetActive(newState == State.DefenseTiming);
            if (newState == State.DefenseTiming && defenseTimingSystem != null)
            {
                defenseTimingSystem.StartTimingGame();
            }
        }
        
        Debug.Log("전투 상태 변경: " + newState);
    }
    
    // 아이템 선택만 처리 (전투 시작하지 않음)
    public void OnItemSelected(Item item, bool startCombat = false)
    {
        selectedItem = item;
        // 아이템 제목 업데이트
        if (itemTitleText != null)
        {
            itemTitleText.text = "현재 아이템: " + (item != null ? item.ItemName : "없음");
        }

        // 아이템 효과 설명 업데이트
        if (itemEffectDescriptionText != null)
        {
            itemEffectDescriptionText.text = item != null ? item.GetEffectsDescription() : "";
            itemEffectDescriptionText.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 노란 형광색 (RGB 255, 255, 0)
        }

        Debug.Log("아이템 선택됨: " + (item != null ? item.GetItemInfo() : "None"));
        
        // startCombat이 true일 때만 전투 시작
        if (startCombat)
        {
            SetState(State.AttackTiming);
        }
    }
    
    public void SetDamageMultiplier(float multiplier)
    {
        currentDamageMultiplier = multiplier;
    }
    
    public void SetDefenseResult(bool success, float bonus)
    {
        defenseSuccess = success;
        defenseBonus = bonus;
    }
    
    void ExecuteAttack()
    {
        if (playerCharacter == null || enemyCharacter == null)
        {
            Debug.LogError("PlayerCharacter 또는 EnemyCharacter가 없습니다!");
            return;
        }
        
        float playerDamage = playerCharacter.CalculateAttackDamage() * currentDamageMultiplier;
        
        if (selectedItem != null)
        {
            playerDamage *= selectedItem.DamageCoefficient;
            
            // 연속공격 효과 확인
            if (selectedItem.HasEffect(ItemEffectType.DoubleAttack))
            {
                int attackCount = (int)selectedItem.GetEffectValue(ItemEffectType.DoubleAttack);
                ExecuteMultipleAttacks(attackCount);
                return;
            }
            
            // 아이템 날라가기 효과 시작 (일반 공격)
            StartCoroutine(FlyItemToEnemy(selectedItem.gameObject, playerDamage, (wasDefended) => {
                // 다른 특수 효과 적용
                selectedItem.ApplyItemEffects(playerCharacter, enemyCharacter, this);
                // 데미지 적용 시도는 코루틴에서 이미 처리됨
                // 데미지 또는 방어 텍스트 표시
                ShowDamageText(playerDamage, enemyCharacter.transform.position, wasDefended);
                Debug.Log("플레이어 공격! " + (wasDefended ? "방어됨!" : enemyCharacter.EnemyName + "에게 데미지: " + playerDamage));
                // 플레이어 턴 종료 - 적의 일회성 효과 리셋
                enemyCharacter.EndTurn();
                if (enemyCharacter.HP <= 0)
                {
                    Debug.Log(enemyCharacter.EnemyName + " 사망! 승리!");
                    // CEO를 처치했는지 확인 (사장 랭크)
                    if (enemyCharacter.EnemyRank == "사장")
                    {
                        var gameEndManager = FindAnyObjectByType<GameEndManager>();
                        if (gameEndManager != null)
                        {
                            gameEndManager.ShowGameClear();
                        }
                        return; // 사장 처치 시 상태 전환 및 Normal BGM 호출 방지
                    }
                    else if (stageManager != null)
                    {
                        stageManager.OnEnemyDefeated();
                        if (bgmManager != null)
                        {
                            bgmManager.PlayBGM("Battle");
                        }
                    }
                    SetState(State.ItemSelect);
                }
                else
                {
                    SetState(State.DefenseTiming);
                }
                if (!wasDefended) PlayHitSFX();
                if (wasDefended && sfxAudioSource != null && sfxTting != null) sfxAudioSource.PlayOneShot(sfxTting);
            }));
        }
    }
    
    // 아이템 날라가기 효과
    IEnumerator FlyItemToEnemy(GameObject item, float damage, System.Action<bool> onComplete)
    {
        if (gameCanvas == null)
        {
            Debug.LogError("게임 캔버스가 설정되지 않았습니다!");
            onComplete?.Invoke(false);
            yield break;
        }

        // 효과음 재생 (아이템 날아가기)
        if (sfxAudioSource != null && sfxItemFly != null)
            sfxAudioSource.PlayOneShot(sfxItemFly);

        // 아이템의 복사본 생성
        GameObject flyingItem = Instantiate(item, gameCanvas.transform);
        RectTransform flyingRect = flyingItem.GetComponent<RectTransform>();
        
        // 불필요한 컴포넌트 제거
        Destroy(flyingItem.GetComponent<Button>());
        Destroy(flyingItem.GetComponent<ItemLockState>());
        
        // 잠금 이미지 제거
        Transform lockImage = flyingItem.transform.Find("LockImage");
        if (lockImage != null)
        {
            Destroy(lockImage.gameObject);
        }
        
        // 원본 아이템의 크기와 이미지 설정 복사
        RectTransform originalRect = item.GetComponent<RectTransform>();
        flyingRect.sizeDelta = originalRect.sizeDelta;
        
        Image flyingImage = flyingItem.GetComponent<Image>();
        Image originalImage = item.GetComponent<Image>();
        if (flyingImage && originalImage)
        {
            flyingImage.sprite = originalImage.sprite;
            flyingImage.color = originalImage.color;
        }

        // 시작 위치 (아이템 패널의 현재 위치)
        Vector2 startPos = playerCharacter.transform.position;
        flyingRect.position = startPos;

        // 목표 위치 (적 캐릭터의 위치)
        Vector2 targetPos = enemyCharacter.transform.position;

        Debug.Log($"Flying item from {startPos} to {targetPos}");
        
        float elapsed = 0f;
        Vector2 originalScale = flyingRect.localScale;
        
        while (elapsed < itemFlyDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / itemFlyDuration;
            
            // 포물선 운동 계산
            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, normalizedTime);
            float height = Mathf.Sin(normalizedTime * Mathf.PI) * itemArcHeight;
            currentPos.y += height;
            
            // 위치 업데이트
            flyingRect.position = currentPos;
            
            // 크기 약간 줄이기
            float scale = Mathf.Lerp(1f, 0.5f, normalizedTime);
            flyingRect.localScale = originalScale * scale;
            
            yield return null;
        }
        
        // 효과 종료 및 정리
        Destroy(flyingItem);
        // 데미지 적용 및 방어 여부 판단
        bool wasDefended = !enemyCharacter.TryTakeDamage(damage);
        onComplete?.Invoke(wasDefended);
    }
    
    // 데미지 텍스트 표시
    void ShowDamageText(float damage, Vector3 position, bool isDefended = false)
    {
        if (damageTextPrefab == null || gameCanvas == null) return;
        
        // 데미지 텍스트 생성
        GameObject damageTextObj = Instantiate(damageTextPrefab, gameCanvas.transform);
        RectTransform rectTransform = damageTextObj.GetComponent<RectTransform>();
        
        // 월드 위치를 스크린 위치로 설정
        rectTransform.position = position;
        
        // 텍스트 설정
        Text damageText = damageTextObj.GetComponent<Text>();
        if (damageText != null)
        {
            if (isDefended)
            {
                damageText.text = "방어됨";
                damageText.color = Color.green;
            }
            else
            {
                damageText.text = $"-{damage:F0}";
                damageText.color = Color.red;
            }
            StartCoroutine(AnimateDamageText(rectTransform));
        }
    }
    
    // 데미지 텍스트 애니메이션
    IEnumerator AnimateDamageText(RectTransform textTransform)
    {
        float elapsed = 0f;
        Vector3 startPos = textTransform.position;
        Color startColor = textTransform.GetComponent<Text>().color;
        
        while (elapsed < damageTextDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / damageTextDuration;
            
            // 위로 올라가면서 페이드아웃
            Vector3 currentPos = startPos + Vector3.up * (damageTextRiseDistance * normalizedTime);
            textTransform.position = currentPos;
            
            // 크기도 약간 커지도록
            float scale = Mathf.Lerp(1f, 1.2f, normalizedTime);
            textTransform.localScale = Vector3.one * scale;
            
            Color currentColor = startColor;
            currentColor.a = 1 - normalizedTime;
            textTransform.GetComponent<Text>().color = currentColor;
            
            yield return null;
        }
        
        Destroy(textTransform.gameObject);
    }
    
    // 연속 공격 처리
    void ExecuteMultipleAttacks(int attackCount)
    {
        StartCoroutine(MultipleAttackSequence(attackCount));
    }
    
    IEnumerator MultipleAttackSequence(int attackCount)
    {
        for (int i = 0; i < attackCount; i++)
        {
            if (enemyCharacter.HP <= 0) break;
            
            // 각 공격마다 새로운 데미지 계산 (아이템 계수와 타이밍 배율 포함)
            float currentDamage = playerCharacter.CalculateAttackDamage() * currentDamageMultiplier;
            if (selectedItem != null)
            {
                currentDamage *= selectedItem.DamageCoefficient;
            }
            
            // 아이템 날라가기 효과 실행
            yield return StartCoroutine(FlyItemToEnemy(selectedItem.gameObject, currentDamage, null));
            
            // 데미지 적용 시도
            bool wasDefended = !enemyCharacter.TryTakeDamage(currentDamage);
            
            // 효과음 재생 (방어 성공/실패)
            if (wasDefended && sfxAudioSource != null && sfxTting != null) sfxAudioSource.PlayOneShot(sfxTting);
            if (!wasDefended) PlayHitSFX();
            // 데미지 또는 방어 텍스트 표시
            ShowDamageText(currentDamage, enemyCharacter.transform.position, wasDefended);
            Debug.Log($"연속 공격 {i + 1}번째! " + (wasDefended ? "방어됨!" : $"데미지: {currentDamage}"));
            
            yield return new WaitForSeconds(0.3f); // 연속 공격 간 딜레이
        }
        
        // 다른 특수 효과 적용
        if (selectedItem != null)
        {
            selectedItem.ApplyItemEffects(playerCharacter, enemyCharacter, this);
        }
        
        // 플레이어 턴 종료 - 적의 일회성 효과 리셋
        enemyCharacter.EndTurn();
        
        // 전투 상태 확인 및 변경
        if (enemyCharacter.HP <= 0)
        {
            Debug.Log(enemyCharacter.EnemyName + " 사망! 승리!");
            
            // CEO를 처치했는지 확인 (사장 랭크)
            if (enemyCharacter.EnemyRank == "사장")
            {
                var gameEndManager = FindAnyObjectByType<GameEndManager>();
                if (gameEndManager != null)
                {
                    gameEndManager.ShowGameClear();
                }
            }
            else if (stageManager != null)
            {
                stageManager.OnEnemyDefeated();
                if (bgmManager != null)
                {
                    bgmManager.PlayBGM("Battle");
                }
            }
            
            SetState(State.ItemSelect);
        }
        else
        {
            SetState(State.DefenseTiming);
        }
    }
    
    void ExecuteDefense()
    {
        if (playerCharacter == null || enemyCharacter == null)
        {
            Debug.LogError("PlayerCharacter 또는 EnemyCharacter가 없습니다!");
            return;
        }

        // 방어 타이밍 시스템의 결과를 먼저 처리
        if (defenseTimingSystem != null)
        {
            defenseTimingSystem.CheckTiming();
            // 방어 타이밍 시스템의 결과가 처리될 때까지 잠시 대기
            StartCoroutine(WaitForDefenseResult());
        }
    }

    IEnumerator WaitForDefenseResult()
    {
        // 방어 타이밍 시스템의 결과가 처리될 때까지 잠시 대기
        yield return new WaitForSeconds(0.1f);
        
        // 적의 공격 실행
        ExecuteEnemyAttack();
    }
    
    void ExecuteEnemyAttack()
    {
        if (playerCharacter == null || enemyCharacter == null)
        {
            Debug.LogError("PlayerCharacter 또는 EnemyCharacter가 없습니다!");
            return;
        }

        float enemyDamage = enemyCharacter.CalculateAttackDamage();
        
        // 플레이어가 선택한 아이템이 있다면 날라가기 효과 시작
        if (selectedItem != null)
        {
            Debug.Log("[SFX] ExecuteEnemyAttack: 아이템 날아가기 효과음 재생 시도");
            if (sfxAudioSource != null && sfxItemFly != null)
                sfxAudioSource.PlayOneShot(sfxItemFly);
            StartCoroutine(FlyItemToPlayer(selectedItem.gameObject, enemyDamage, (wasDefended) => {
                // 데미지 또는 방어 텍스트 표시
                ShowDamageText(enemyDamage, playerCharacter.transform.position, wasDefended);
                Debug.Log("적 공격! " + (wasDefended ? "방어됨!" : "플레이어에게 데미지: " + enemyDamage));
                if (!wasDefended) PlayHitSFX();
                if (wasDefended && sfxAudioSource != null && sfxTting != null) sfxAudioSource.PlayOneShot(sfxTting);
                // 적 턴 종료
                enemyCharacter.EndTurn();
                // 방어 결과 초기화
                defenseSuccess = false;
                defenseBonus = 0f;
                if (playerCharacter.HP <= 0)
                {
                    Debug.Log("플레이어 사망! 게임 오버!");
                    var gameEndManager = FindAnyObjectByType<GameEndManager>();
                    if (gameEndManager != null)
                    {
                        gameEndManager.ShowGameOver();
                    }
                }
                else
                {
                    SetState(State.ItemSelect);
                }
            }));
        }
        else
        {
            // 아이템이 없는 경우 바로 데미지 적용
            if (!defenseSuccess)
            {
                playerCharacter.TryTakeDamage(enemyDamage);
            }
            
            // 데미지 또는 방어 텍스트 표시
            ShowDamageText(enemyDamage, playerCharacter.transform.position, defenseSuccess);
            Debug.Log("적 공격! " + (defenseSuccess ? "방어됨!" : "플레이어에게 데미지: " + enemyDamage));
            if (defenseSuccess && sfxAudioSource != null && sfxTting != null) sfxAudioSource.PlayOneShot(sfxTting);
            
            // 적 턴 종료
            enemyCharacter.EndTurn();
            
            // 방어 결과 초기화
            defenseSuccess = false;
            defenseBonus = 0f;
            
            if (playerCharacter.HP <= 0)
            {
                Debug.Log("플레이어 사망! 게임 오버!");
                var gameEndManager = FindAnyObjectByType<GameEndManager>();
                if (gameEndManager != null)
                {
                    gameEndManager.ShowGameOver();
                }
            }
            else
            {
                SetState(State.ItemSelect);
            }
        }
    }

    // 아이템이 적에서 플레이어로 날라가는 효과
    IEnumerator FlyItemToPlayer(GameObject item, float damage, System.Action<bool> onComplete)
    {
        if (gameCanvas == null)
        {
            Debug.LogError("게임 캔버스가 설정되지 않았습니다!");
            onComplete?.Invoke(false);
            yield break;
        }

        Debug.Log("[SFX] FlyItemToPlayer: 아이템 날아가기 효과음 재생 시도");
        // 효과음 재생 (아이템 날아가기)
        if (sfxAudioSource != null && sfxItemFly != null)
            sfxAudioSource.PlayOneShot(sfxItemFly);

        // 아이템의 복사본 생성
        GameObject flyingItem = Instantiate(item, gameCanvas.transform);
        RectTransform flyingRect = flyingItem.GetComponent<RectTransform>();
        
        // 불필요한 컴포넌트 제거
        Destroy(flyingItem.GetComponent<Button>());
        Destroy(flyingItem.GetComponent<ItemLockState>());
        
        // 잠금 이미지 제거
        Transform lockImage = flyingItem.transform.Find("LockImage");
        if (lockImage != null)
        {
            Destroy(lockImage.gameObject);
        }
        
        // 원본 아이템의 크기와 이미지 설정 복사
        RectTransform originalRect = item.GetComponent<RectTransform>();
        flyingRect.sizeDelta = originalRect.sizeDelta;
        
        Image flyingImage = flyingItem.GetComponent<Image>();
        Image originalImage = item.GetComponent<Image>();
        if (flyingImage && originalImage)
        {
            flyingImage.sprite = originalImage.sprite;
            flyingImage.color = originalImage.color;
        }

        // 시작 위치 (적 캐릭터의 위치)
        Vector2 startPos = enemyCharacter.transform.position;
        flyingRect.position = startPos;

        // 목표 위치 (플레이어 캐릭터의 위치)
        Vector2 targetPos = playerCharacter.transform.position;

        Debug.Log($"Flying item from enemy to player: {startPos} to {targetPos}");
        
        float elapsed = 0f;
        Vector2 originalScale = flyingRect.localScale;
        
        while (elapsed < itemFlyDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = elapsed / itemFlyDuration;
            
            // 포물선 운동 계산
            Vector2 currentPos = Vector2.Lerp(startPos, targetPos, normalizedTime);
            float height = Mathf.Sin(normalizedTime * Mathf.PI) * itemArcHeight;
            currentPos.y += height;
            
            // 위치 업데이트
            flyingRect.position = currentPos;
            
            // 크기 약간 줄이기
            float scale = Mathf.Lerp(1f, 0.5f, normalizedTime);
            flyingRect.localScale = originalScale * scale;
            
            yield return null;
        }
        
        // 아이템 제거
        Destroy(flyingItem);
        // 데미지 적용 및 방어 여부 판단
        bool wasDefended = !playerCharacter.TryTakeDamage(damage);
        onComplete?.Invoke(wasDefended);
        // 맞는 소리 및 "오!" 효과음 재생
        if (!wasDefended) {
            PlayHitSFX();
            if (sfxAudioSource != null && sfxPlayerOh != null)
                sfxAudioSource.PlayOneShot(sfxPlayerOh);
        }
    }
    
    void UpdateUI()
    {
        if (playerHpText && playerCharacter)
            playerHpText.text = "김사원 HP: \n" + playerCharacter.HP.ToString("F0") + "/" + playerCharacter.MaxHP.ToString("F0");
            
        if (enemyHpText && enemyCharacter)
            enemyHpText.text = enemyCharacter.EnemyName + " HP: \n" + enemyCharacter.HP.ToString("F0") + "/" + enemyCharacter.MaxHP.ToString("F0");
            
        if (stageText && stageManager != null)
            stageText.text = "스테이지: " + (stageManager.GetCurrentStage() + 1) + "\n직급: " + stageManager.GetCurrentEnemyRank() + "\n현재 상태: " + GetStateDisplayText();
    }
    
    string GetStateDisplayText()
    {
        switch (currentState)
        {
            case State.ItemSelect: return "아이템 선택";
            case State.AttackTiming: return "공격 타이밍";
            case State.EnemyAttack: return "적 공격 중";
            case State.DefenseTiming: return "방어 타이밍";
            default: return "알 수 없음";
        }
    }
    
    // ResetCombat 메서드를 public으로 변경
    public void ResetCombat()
    {
        if (gameEndManager != null && gameEndManager.IsGameEnded())
            return;
        
        if (playerCharacter != null)
        {
            playerCharacter.HP = playerCharacter.MaxHP;
        }
        
        if (enemyCharacter != null)
        {
            enemyCharacter.HP = enemyCharacter.MaxHP;
        }
        
        selectedItem = null;
        SetState(State.ItemSelect);
        Debug.Log("전투 리셋! " + (enemyCharacter != null ? enemyCharacter.EnemyName : "대리") + "와의 전투 재시작");
    }
    
    // 공격 버튼 클릭 이벤트
    public void OnAttackButtonClick()
    {
        if (gameEndManager != null && gameEndManager.IsGameEnded())
            return;
        if (currentState == State.AttackTiming)
        {
            Debug.Log("공격 버튼 클릭!");
            if (attackTimingSystem != null)
            {
                attackTimingSystem.CheckTiming();
            }
            ExecuteAttack();
        }
    }
    
    // 방어 버튼 클릭 이벤트
    public void OnDefenseButtonClick()
    {
        if (gameEndManager != null && gameEndManager.IsGameEnded())
            return;
        if (currentState == State.DefenseTiming)
        {
            Debug.Log("방어 버튼 클릭!");
            if (defenseTimingSystem != null)
            {
                defenseTimingSystem.CheckTiming();
            }
            ExecuteDefense();
        }
    }
    
    private void OnDestroy()
    {
        // 버튼 이벤트 제거
        if (attackButton != null)
        {
            attackButton.onClick.RemoveListener(OnAttackButtonClick);
        }
        if (defenseButton != null)
        {
            defenseButton.onClick.RemoveListener(OnDefenseButtonClick);
        }
    }
    
    // 적이 데미지를 받을 때 효과음 재생
    void PlayHitSFX()
    {
        if (sfxAudioSource != null && sfxHit != null)
            sfxAudioSource.PlayOneShot(sfxHit);
    }
}