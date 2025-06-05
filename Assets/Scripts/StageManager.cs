using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    [System.Serializable]
    public class StageData
    {
        public string stageName;
        public string enemyRank;
        public GameObject enemyPrefab;
        public GameObject unlockedItemPrefab;
    }
    
    [Header("스테이지 설정")]
    public List<StageData> stages = new List<StageData>();
    public Transform enemySpawnPoint;
    
    [Header("현재 스테이지 정보")]
    [SerializeField] private int currentStageIndex = 0;
    [SerializeField] private bool[] stageCleared;
    
    private CombatManager combatManager;
    private GameObject currentEnemy;
    
    void Start()
    {
        combatManager = FindObjectOfType<CombatManager>();
        stageCleared = new bool[stages.Count];
        
        // PlayerPrefs에서 스테이지 클리어 정보 로드
        LoadStageProgress();
        
        // 첫 스테이지 시작
        SpawnCurrentStageEnemy();
    }
    
    void LoadStageProgress()
    {
        for (int i = 0; i < stages.Count; i++)
        {
            stageCleared[i] = PlayerPrefs.GetInt("Stage_" + i + "_Cleared", 0) == 1;
            
            // 스테이지 클리어 시 아이템 해금
            if (stageCleared[i] && stages[i].unlockedItemPrefab != null)
            {
                UnlockItem(stages[i].unlockedItemPrefab);
            }
        }
    }
    
    void SaveStageProgress()
    {
        for (int i = 0; i < stageCleared.Length; i++)
        {
            PlayerPrefs.SetInt("Stage_" + i + "_Cleared", stageCleared[i] ? 1 : 0);
        }
        PlayerPrefs.Save();
    }
    
    public void OnEnemyDefeated()
    {
        // 현재 스테이지 클리어 처리
        if (!stageCleared[currentStageIndex])
        {
            stageCleared[currentStageIndex] = true;
            SaveStageProgress();
            
            // 아이템 해금
            if (stages[currentStageIndex].unlockedItemPrefab != null)
            {
                UnlockItem(stages[currentStageIndex].unlockedItemPrefab);
            }
        }
        
        // 다음 스테이지로 진행
        if (currentStageIndex < stages.Count - 1)
        {
            currentStageIndex++;
            SpawnCurrentStageEnemy();
        }
        else
        {
            Debug.Log("모든 스테이지 클리어!");
            // TODO: 엔딩 또는 게임 클리어 처리
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
            // UI 요소로 생성
            currentEnemy = Instantiate(stages[currentStageIndex].enemyPrefab, enemySpawnPoint.transform);
            
            // RectTransform 위치 설정
            RectTransform rectTransform = currentEnemy.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // 우측 하단 기준으로 설정
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = new Vector2(-150, 180);
                rectTransform.sizeDelta = new Vector2(200, 350);
                rectTransform.localScale = Vector3.one;
            }
            
            // CombatManager에 새로운 적 등록
            if (combatManager != null)
            {
                var enemyCharacter = currentEnemy.GetComponent<EnemyCharacter>();
                if (enemyCharacter != null)
                {
                    combatManager.SetEnemy(enemyCharacter);
                }
            }
            
            Debug.Log($"스테이지 {currentStageIndex + 1} 시작: {stages[currentStageIndex].enemyRank}");
        }
    }
    
    void UnlockItem(GameObject itemPrefab)
    {
        // 아이템 프리팹을 ItemPanel에 추가
        var itemPanel = GameObject.Find("ItemPanel");
        if (itemPanel != null)
        {
            var newItem = Instantiate(itemPrefab, itemPanel.transform);
            Debug.Log($"새 아이템 해금: {itemPrefab.name}");
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