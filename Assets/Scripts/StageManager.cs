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
    [SerializeField] private GameObject itemPanel; // Inspector에서 할당할 수 있도록 추가
    
    [Header("현재 스테이지 정보")]
    [SerializeField] private int currentStageIndex = 0;
    [SerializeField] private bool[] stageCleared;
    
    private CombatManager combatManager;
    private GameObject currentEnemy;
    private bool isInitialized = false;
    
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
        combatManager = FindObjectOfType<CombatManager>();
        
        if (!isInitialized)
        {
            InitializeItemStates();
            isInitialized = true;
        }
        
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
    
    void ResetAllProgress()
    {
        Debug.Log("=== Resetting All Progress ===");
        // 모든 스테이지를 미클리어 상태로 초기화
        for (int i = 0; i < stages.Count; i++)
        {
            stageCleared[i] = false;
            PlayerPrefs.SetInt($"Stage_{i}_Cleared", 0);
        }
        
        // 최초 실행 플래그 설정
        PlayerPrefs.SetInt("IsFirstRun", 0);
        PlayerPrefs.Save();
        
        Debug.Log("All progress has been reset to initial state");
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
            
            // 해당 스테이지의 아이템 해금
            UnlockStageItem(currentStageIndex);
        }
        
        // 다음 스테이지로 진행
        if (currentStageIndex < stages.Count - 1)
        {
            currentStageIndex++;
            Debug.Log($"Advancing to stage {currentStageIndex}");
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
} 