using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemLockState : MonoBehaviour
{
    private Image lockImage;        // 잠금 이미지
    private Button itemButton;      // 아이템 버튼
    private Image itemImage;        // 아이템 이미지
    private GameObject lockObj;     // 잠금 오브젝트
    
    private void Awake()
    {
        // 컴포넌트 찾기
        itemButton = GetComponent<Button>();
        itemImage = GetComponent<Image>();
        
        // LockImage 프리팹 생성
        CreateLockImage();
    }
    
    private void CreateLockImage()
    {
        // 이미 존재하는 LockImage가 있다면 제거
        Transform existingLock = transform.Find("LockImage");
        if (existingLock != null)
        {
            DestroyImmediate(existingLock.gameObject);
        }

        // 프리팹 로드 및 인스턴스화
        #if UNITY_EDITOR
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/LockImage.prefab");
        if (prefab != null)
        {
            lockObj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
        }
        #else
        GameObject prefab = Resources.Load<GameObject>("Prefabs/LockImage");
        if (prefab != null)
        {
            lockObj = Instantiate(prefab, transform);
        }
        #endif

        if (lockObj != null)
        {
            // RectTransform 설정
            RectTransform lockRect = lockObj.GetComponent<RectTransform>();
            if (lockRect != null)
            {
                lockRect.anchorMin = Vector2.zero;
                lockRect.anchorMax = Vector2.one;
                lockRect.offsetMin = Vector2.zero;
                lockRect.offsetMax = Vector2.zero;
                lockRect.localScale = Vector3.one;
            }

            // Image 컴포넌트 참조
            lockImage = lockObj.GetComponent<Image>();
            
            Debug.Log($"[{gameObject.name}] LockImage created successfully");
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] Failed to create LockImage - Prefab not found at Assets/Prefabs/LockImage.prefab");
        }
    }
    
    private void Start()
    {
        // 초기 상태 설정 (ReportBundle 제외)
        if (gameObject.name != "ReportBundle")
        {
            SetLockState(true);
            Debug.Log($"[{gameObject.name}] 초기 상태: Locked");
        }
        else
        {
            SetLockState(false);
            Debug.Log($"[{gameObject.name}] 초기 상태: Unlocked (ReportBundle)");
        }
    }
    
    public void SetLockState(bool isLocked)
    {
        if (lockObj != null)
        {
            lockObj.SetActive(isLocked);
            Debug.Log($"[{gameObject.name}] LockImage visibility set to: {isLocked}");
            
            if (itemButton != null)
            {
                itemButton.interactable = !isLocked;
                Debug.Log($"[{gameObject.name}] ItemButton interactable set to: {!isLocked}");
            }
                
            if (itemImage != null)
            {
                itemImage.color = isLocked ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white;
                Debug.Log($"[{gameObject.name}] ItemImage color set to: {(isLocked ? "Gray" : "White")}");
            }
        }
        else
        {
            Debug.LogError($"[{gameObject.name}] LockObj is null!");
        }
    }
} 