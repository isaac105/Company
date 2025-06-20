using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [System.Serializable]
    public class StageData
    {
        public string stageName;
        public string enemyRank;
        public GameObject enemyPrefab;
        public GameObject unlockedItemPrefab;
        public Sprite backgroundImage; // 스테이지 배경 이미지
        
        [Header("적 캐릭터 이미지")]
        public Sprite[] poseSprites;    // 기본 포즈 (01, 02)
        public Sprite[] angrySprites;   // 화난 포즈 (01, 02)
        public Sprite dodgeSprite;      // 회피 포즈
        public Sprite[] throwSprites;   // 던지기 포즈 (01, 02)
    }
    
    [Header("스테이지 설정")]
    public List<StageData> stages = new List<StageData>();
    public Transform enemySpawnPoint;
    
    [Header("UI 참조")]
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private Image backgroundImage; // 배경 이미지 UI 참조
    
    [Header("현재 스테이지 정보")]
    [SerializeField] private int currentStageIndex = 0;
    [SerializeField] private bool[] stageCleared;
    
    private CombatManager combatManager;
    private GameObject currentEnemy;
    private bool isInitialized = false;
    
    public bool IsInitialized { get { return isInitialized; } }
    
    void Awake()
    {
        Debug.Log("StageManager Awake");
        stageCleared = new bool[stages.Count];
        ResetAllProgress();

        // // 게임 최초 실행 여부 확인
        // bool isFirstRun = PlayerPrefs.GetInt("IsFirstRun", 1) == 1;
        // if (isFirstRun)
        // {
        //     Debug.Log("First time running the game - Initializing fresh stage progress");
        //     ResetAllProgress();
        // }
        // else
        // {
        //     LoadStageProgress();
        // }
    }
    
    void Start()
    {
        Debug.Log("StageManager Start");
        combatManager = FindFirstObjectByType<CombatManager>();
        
        // 배경 이미지 컴포넌트 찾기 (없으면 생성)
        if (backgroundImage == null)
        {
            var gameArea = GameObject.Find("GameArea");
            if (gameArea != null)
            {
                var bgImageObj = gameArea.transform.Find("BackgroundImage");
                if (bgImageObj != null)
                {
                    backgroundImage = bgImageObj.GetComponent<Image>();
                    if (backgroundImage != null)
                    {
                        // 배경 이미지를 GameArea의 첫 번째 자식으로 설정
                        backgroundImage.transform.SetSiblingIndex(0);
                        // 배경 이미지가 전체 GameArea를 채우도록 설정
                        var rectTransform = backgroundImage.GetComponent<RectTransform>();
                        if (rectTransform != null)
                        {
                            rectTransform.anchorMin = Vector2.zero;
                            rectTransform.anchorMax = Vector2.one;
                            rectTransform.offsetMin = Vector2.zero;
                            rectTransform.offsetMax = Vector2.zero;
                        }
                    }
                }
            }
        }
        
        if (!isInitialized)
        {
            InitializeItemStates();
            isInitialized = true;
            Debug.Log("StageManager 초기화 완료");
        }
        
        UpdateBackground();
        SpawnCurrentStageEnemy();
    }
    
    void InitializeItemStates()
    {
        Debug.Log("=== Initializing Item States ===");
        if (itemPanel == null)
        {
            Debug.LogError("ItemPanel이 설정되지 않았습니다.");
            return;
        }

        var items = itemPanel.GetComponentsInChildren<ItemLockState>(true);
        Debug.Log($"Found {items.Length} items with ItemLockState");

        foreach (var item in items)
        {
            if (item.gameObject.name.Contains("ReportBundle"))
            {
                UnlockItem(item, "기본 아이템");
                continue;
            }

            bool isUnlocked = false;
            for (int i = 0; i < stages.Count; i++)
            {
                if (stageCleared[i] && stages[i].unlockedItemPrefab != null &&
                    item.gameObject.name.Contains(stages[i].unlockedItemPrefab.name))
                {
                    isUnlocked = true;
                    UnlockItem(item, $"스테이지 {i} 클리어");
                    break;
                }
            }

            if (!isUnlocked)
            {
                LockItem(item);
            }
        }
        Debug.Log("=== Item State Initialization Complete ===");
    }
    
    void UnlockItem(ItemLockState item, string reason = "")
    {
        Debug.Log($"Unlocking item: {item.gameObject.name} (Reason: {reason})");
        item.SetLockState(false);
    }
    
    void LockItem(ItemLockState item)
    {
        Debug.Log($"Locking item: {item.gameObject.name}");
        item.SetLockState(true);
    }
    
    void LoadStageProgress()
    {
        Debug.Log("=== Loading Stage Progress ===");
        for (int i = 0; i < stages.Count; i++)
        {
            stageCleared[i] = PlayerPrefs.GetInt($"Stage_{i}_Cleared", 0) == 1;
            Debug.Log($"Stage {i}: {(stageCleared[i] ? "Cleared" : "Not Cleared")}");
        }
    }
    
    public void ResetAllProgress()
    {
        Debug.Log("모든 스테이지 진행 상황 초기화");
        for (int i = 0; i < stageCleared.Length; i++)
        {
            stageCleared[i] = false;
        }
        // SaveStageProgress();
    }
    
    public void RestartFromBeginning()
    {
        Debug.Log("게임을 처음부터 다시 시작합니다.");
        currentStageIndex = 0;
        
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
            currentEnemy = null;
        }
        
        UpdateBackground(); // 배경 업데이트
        SpawnCurrentStageEnemy();
        
        if (itemPanel != null)
        {
            itemPanel.SetActive(true);
        }
        
        if (combatManager != null)
        {
            combatManager.SetState(CombatManager.State.ItemSelect);
        }
    }
    
    // 진행 상황을 수동으로 리셋하기 위한 public 메서드
    public void ManuallyResetProgress()
    {
        ResetAllProgress();
        InitializeItemStates();
    }
    
    void SaveStageProgress()
    {
        Debug.Log("=== Saving Stage Progress ===");
        for (int i = 0; i < stageCleared.Length; i++)
        {
            PlayerPrefs.SetInt($"Stage_{i}_Cleared", stageCleared[i] ? 1 : 0);
            Debug.Log($"Saving Stage {i}: {(stageCleared[i] ? "Cleared" : "Not Cleared")}");
        }
        PlayerPrefs.Save();
    }
    
    public void OnEnemyDefeated()
    {
        Debug.Log($"=== Enemy Defeated in Stage {currentStageIndex} ===");
        
        if (!stageCleared[currentStageIndex])
        {
            Debug.Log($"First time clearing stage {currentStageIndex}");
            stageCleared[currentStageIndex] = true;
            SaveStageProgress();
            
            UnlockStageItem(currentStageIndex);
        }
        
        if (currentStageIndex < stages.Count - 1)
        {
            currentStageIndex++;
            Debug.Log($"Advancing to stage {currentStageIndex}");
            UpdateBackground(); // 스테이지 변경 시 배경 업데이트
            SpawnCurrentStageEnemy();
        }
        else
        {
            Debug.Log("=== All Stages Cleared! ===");
        }
    }
    
    void UnlockStageItem(int stageIndex)
    {
        if (stages[stageIndex].unlockedItemPrefab == null)
        {
            Debug.Log($"No item to unlock for stage {stageIndex}");
            return;
        }

        if (itemPanel == null)
        {
            Debug.LogError("ItemPanel이 설정되지 않았습니다.");
            return;
        }

        Debug.Log($"Attempting to unlock item for stage {stageIndex}");
        var items = itemPanel.GetComponentsInChildren<ItemLockState>(true);
        string targetItemName = stages[stageIndex].unlockedItemPrefab.name;
        bool found = false;

        foreach (var item in items)
        {
            if (item.gameObject.name.Contains(targetItemName))
            {
                UnlockItem(item, $"스테이지 {stageIndex} 클리어 보상");
                found = true;
                
                // ItemSelectionManager에 아이템 해금 알림
                var itemSelectionManager = FindFirstObjectByType<ItemSelectionManager>();
                if (itemSelectionManager != null)
                {
                    itemSelectionManager.OnItemUnlocked();
                }
                
                break;
            }
        }

        if (!found)
        {
            Debug.LogWarning($"Could not find item matching {targetItemName} to unlock");
        }
    }
    
    void SpawnCurrentStageEnemy()
    {
        if (currentEnemy != null)
        {
            Destroy(currentEnemy);
        }
        
        if (currentStageIndex < stages.Count && stages[currentStageIndex].enemyPrefab != null)
        {
            Debug.Log($"Spawning enemy for stage {currentStageIndex}");
            currentEnemy = Instantiate(stages[currentStageIndex].enemyPrefab, enemySpawnPoint.transform);
            
            RectTransform rectTransform = currentEnemy.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = new Vector2(-150, 180);
                rectTransform.sizeDelta = new Vector2(200, 350);
                rectTransform.localScale = Vector3.one;
            }
            
            var enemyCharacter = currentEnemy.GetComponent<EnemyCharacter>();
            if (enemyCharacter != null)
            {
                // CharacterImageManager 설정
                var imageManager = currentEnemy.GetComponent<CharacterImageManager>();
                if (imageManager == null)
                {
                    imageManager = currentEnemy.AddComponent<CharacterImageManager>();
                }

                // 이미지 컴포넌트 설정
                var image = currentEnemy.GetComponent<Image>();
                if (image == null)
                {
                    image = currentEnemy.AddComponent<Image>();
                }

                // 스테이지 데이터에서 이미지 설정
                var stageData = stages[currentStageIndex];
                if (imageManager != null)
                {
                    var serializedObject = new UnityEditor.SerializedObject(imageManager);
                    
                    var poseSpritesProperty = serializedObject.FindProperty("poseSprites");
                    poseSpritesProperty.arraySize = stageData.poseSprites != null ? stageData.poseSprites.Length : 0;
                    for (int i = 0; i < poseSpritesProperty.arraySize; i++)
                    {
                        poseSpritesProperty.GetArrayElementAtIndex(i).objectReferenceValue = stageData.poseSprites[i];
                    }
                    
                    var angrySpritesProperty = serializedObject.FindProperty("angrySprites");
                    angrySpritesProperty.arraySize = stageData.angrySprites != null ? stageData.angrySprites.Length : 0;
                    for (int i = 0; i < angrySpritesProperty.arraySize; i++)
                    {
                        angrySpritesProperty.GetArrayElementAtIndex(i).objectReferenceValue = stageData.angrySprites[i];
                    }
                    
                    var dodgeSpriteProperty = serializedObject.FindProperty("dodgeSprite");
                    dodgeSpriteProperty.objectReferenceValue = stageData.dodgeSprite;
                    
                    var throwSpritesProperty = serializedObject.FindProperty("throwSprites");
                    throwSpritesProperty.arraySize = stageData.throwSprites != null ? stageData.throwSprites.Length : 0;
                    for (int i = 0; i < throwSpritesProperty.arraySize; i++)
                    {
                        throwSpritesProperty.GetArrayElementAtIndex(i).objectReferenceValue = stageData.throwSprites[i];
                    }
                    
                    serializedObject.ApplyModifiedProperties();
                }

                if (combatManager != null)
                {
                    combatManager.SetEnemy(enemyCharacter);
                }
            }
        }
    }
    
    public bool IsStageCleared(int stageIndex)
    {
        if (stageIndex >= 0 && stageIndex < stageCleared.Length)
        {
            return stageCleared[stageIndex];
        }
        return false;
    }
    
    public int GetCurrentStage()
    {
        return currentStageIndex;
    }
    
    public string GetCurrentEnemyRank()
    {
        if (currentStageIndex < stages.Count)
        {
            return stages[currentStageIndex].enemyRank;
        }
        return "Unknown";
    }
    
    // 배경 업데이트 함수
    private void UpdateBackground()
    {
        if (backgroundImage != null && currentStageIndex < stages.Count)
        {
            StageData currentStage = stages[currentStageIndex];
            if (currentStage.backgroundImage != null)
            {
                backgroundImage.sprite = currentStage.backgroundImage;
                backgroundImage.color = Color.white; // 이미지가 보이도록 알파값 설정
                Debug.Log($"배경 이미지 변경: {currentStage.stageName}, Sprite: {currentStage.backgroundImage.name}");
            }
            else
            {
                Debug.LogWarning($"스테이지 {currentStageIndex}의 배경 이미지가 설정되지 않았습니다!");
            }
        }
        else
        {
            Debug.LogError("배경 이미지 컴포넌트를 찾을 수 없거나 스테이지 인덱스가 범위를 벗어났습니다!");
        }
    }
} 