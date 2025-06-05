using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ItemSelectionManager : MonoBehaviour
{
    [Header("아이템 관리")]
    public GameObject itemPanel;
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;
    
    private List<Item> availableItems = new List<Item>();
    private int currentItemIndex = 0;
    private CombatManager combatManager;
    private bool isInitialized = false;
    private StageManager stageManager;
    
    void Awake()
    {
        Debug.Log("ItemSelectionManager Awake");
        StartCoroutine(DelayedInitialization());
    }
    
    IEnumerator DelayedInitialization()
    {
        // StageManager가 초기화될 때까지 대기
        yield return new WaitForSeconds(0.1f);
        
        Debug.Log("ItemSelectionManager 초기화 시작");
        combatManager = FindAnyObjectByType<CombatManager>();
        stageManager = FindAnyObjectByType<StageManager>();
        
        // StageManager가 아이템 상태를 초기화할 때까지 대기
        while (stageManager == null || !stageManager.IsInitialized)
        {
            yield return null;
        }
        
        InitializeItems();
    }
    
    void InitializeItems()
    {
        if (isInitialized) return;
        
        Debug.Log("ItemSelectionManager InitializeItems 시작");
        RefreshAvailableItems();
        
        // 시작할 때 첫 번째 아이템 선택
        if (availableItems.Count > 0)
        {
            currentItemIndex = 0;
            // 모든 아이템 배경색 초기화
            foreach (Item item in availableItems)
            {
                UpdateItemBackground(item.gameObject, normalColor);
            }
            // 첫 번째 아이템 선택
            UpdateItemBackground(availableItems[currentItemIndex].gameObject, selectedColor);
            UpdateItemTitle(availableItems[currentItemIndex]);
            
            Debug.Log($"첫 번째 아이템 선택 완료: {availableItems[currentItemIndex].ItemName}");
        }
        
        isInitialized = true;
        Debug.Log($"아이템 초기화 완료: {availableItems.Count}개 아이템 사용 가능");
    }
    
    void Update()
    {
        if (!isInitialized || availableItems.Count == 0) return;
        
        // 좌우 방향키로 아이템 선택
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SelectPreviousItem();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectNextItem();
        }
        
        // 엔터키로 아이템 사용
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UseCurrentItem();
        }
    }
    
    void RefreshAvailableItems()
    {
        availableItems.Clear();
        
        // itemPanel의 모든 Item 컴포넌트 찾기
        if (itemPanel != null)
        {
            Item[] items = itemPanel.GetComponentsInChildren<Item>(true);
            foreach (Item item in items)
            {
                // 잠금 해제된 아이템만 추가
                ItemLockState lockState = item.GetComponent<ItemLockState>();
                if (lockState == null || !lockState.transform.Find("LockImage").gameObject.activeSelf)
                {
                    availableItems.Add(item);
                    // 초기 상태로 모든 아이템 배경색 설정
                    UpdateItemBackground(item.gameObject, normalColor);
                }
            }
            
            Debug.Log($"사용 가능한 아이템 새로고침: {availableItems.Count}개 발견");
        }
        else
        {
            Debug.LogError("itemPanel이 null입니다!");
        }
    }
    
    void SelectPreviousItem()
    {
        if (availableItems.Count == 0) return;
        
        // 이전 아이템의 배경색 초기화
        UpdateItemBackground(availableItems[currentItemIndex].gameObject, normalColor);
        
        currentItemIndex--;
        if (currentItemIndex < 0)
        {
            currentItemIndex = availableItems.Count - 1;
        }
        
        // 새 아이템 선택
        Item selectedItem = availableItems[currentItemIndex];
        UpdateItemBackground(selectedItem.gameObject, selectedColor);
        UpdateItemTitle(selectedItem);
        
        Debug.Log($"이전 아이템 선택: {currentItemIndex}번 아이템 - {selectedItem.ItemName}");
    }
    
    void SelectNextItem()
    {
        if (availableItems.Count == 0) return;
        
        // 이전 아이템의 배경색 초기화
        UpdateItemBackground(availableItems[currentItemIndex].gameObject, normalColor);
        
        currentItemIndex++;
        if (currentItemIndex >= availableItems.Count)
        {
            currentItemIndex = 0;
        }
        
        // 새 아이템 선택
        Item selectedItem = availableItems[currentItemIndex];
        UpdateItemBackground(selectedItem.gameObject, selectedColor);
        UpdateItemTitle(selectedItem);
        
        Debug.Log($"다음 아이템 선택: {currentItemIndex}번 아이템 - {selectedItem.ItemName}");
    }
    
    void UpdateItemTitle(Item item)
    {
        if (combatManager != null)
        {
            combatManager.OnItemSelected(item, false);  // 전투 시작하지 않고 제목만 업데이트
            Debug.Log($"아이템 제목 업데이트: {item.ItemName}");
        }
        else
        {
            Debug.LogError("CombatManager를 찾을 수 없습니다!");
        }
    }
    
    void UseCurrentItem()
    {
        if (currentItemIndex < 0 || currentItemIndex >= availableItems.Count)
        {
            Debug.LogError("사용할 아이템이 선택되지 않았습니다.");
            return;
        }

        if (combatManager != null)
        {
            Item currentItem = availableItems[currentItemIndex];
            if (currentItem != null)
            {
                Debug.Log($"아이템 사용: {currentItem.ItemName}");
                combatManager.OnItemSelected(currentItem, true);  // 전투 시작
            }
        }
        else
        {
            Debug.LogError("CombatManager를 찾을 수 없습니다!");
        }
    }
    
    void UpdateItemBackground(GameObject itemObject, Color color)
    {
        if (itemObject == null)
        {
            Debug.LogError("아이템 오브젝트가 null입니다!");
            return;
        }

        // 아이템 이름에서 Background 이름 생성
        string backgroundName = itemObject.name + "Background";
        
        // 부모 오브젝트(ItemPanel)에서 배경 찾기
        Transform backgroundTransform = itemPanel.transform.Find(backgroundName);
        if (backgroundTransform != null)
        {
            Image backgroundImage = backgroundTransform.GetComponent<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = color;
                Debug.Log($"배경색 업데이트: {itemObject.name} - {color}");
            }
            else
            {
                Debug.LogWarning($"배경 이미지 컴포넌트를 찾을 수 없음: {backgroundName}");
            }
        }
        else
        {
            Debug.LogWarning($"배경 오브젝트를 찾을 수 없음: {backgroundName}");
        }
    }
    
    public void OnItemUnlocked()
    {
        Debug.Log("아이템 잠금 해제 이벤트 발생");
        int previousIndex = currentItemIndex;
        RefreshAvailableItems();
        
        // 이전 선택 아이템 인덱스 유지
        if (previousIndex >= 0 && previousIndex < availableItems.Count)
        {
            currentItemIndex = previousIndex;
        }
        else if (availableItems.Count > 0)
        {
            currentItemIndex = 0;
        }
        
        // 모든 아이템 배경색 초기화
        foreach (Item item in availableItems)
        {
            UpdateItemBackground(item.gameObject, normalColor);
        }
        
        // 현재 선택된 아이템 하이라이트
        if (currentItemIndex >= 0 && currentItemIndex < availableItems.Count)
        {
            Item selectedItem = availableItems[currentItemIndex];
            UpdateItemBackground(selectedItem.gameObject, selectedColor);
            UpdateItemTitle(selectedItem);
        }
    }
} 