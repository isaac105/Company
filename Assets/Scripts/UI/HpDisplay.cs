using UnityEngine;
using UnityEngine.UI;

public class HpDisplay : MonoBehaviour
{
    [SerializeField] private Sprite[] hpSprites; // hp1.png ~ hp7.png 이미지 배열
    [SerializeField] private Image hpImage; // HP 이미지를 표시할 UI Image

    void Awake()
    {
        if (hpImage == null)
        {
            hpImage = GetComponent<Image>();
        }
        if (hpImage == null)
        {
            Debug.LogError("HpDisplay: HP Image 컴포넌트를 찾을 수 없습니다.");
        }
    }

    public void UpdateHpImage(float currentHp, float maxHp)
    {
        if (hpSprites == null || hpSprites.Length == 0 || hpImage == null)
        {
            // Debug.LogWarning("HpDisplay: HP 스프라이트 배열 또는 HP Image가 설정되지 않았습니다.");
            return;
        }

        float hpRatio = currentHp / maxHp;
        int spriteIndex = Mathf.FloorToInt(hpRatio * (hpSprites.Length - 1));
        spriteIndex = Mathf.Clamp(spriteIndex, 0, hpSprites.Length - 1);

        hpImage.sprite = hpSprites[spriteIndex];
        hpImage.enabled = currentHp > 0; // HP가 0이면 이미지를 숨김
    }
} 