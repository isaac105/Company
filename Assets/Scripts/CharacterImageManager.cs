using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CharacterImageManager : MonoBehaviour
{
    [Header("이미지 컴포넌트")]
    [SerializeField] private Image characterImage;

    [Header("캐릭터 이미지")]
    [SerializeField] private Sprite[] poseSprites;    // 기본 포즈 (01, 02)
    [SerializeField] private Sprite[] angrySprites;   // 화난 포즈 (01, 02)
    [SerializeField] private Sprite dodgeSprite;      // 회피 포즈
    [SerializeField] private Sprite[] throwSprites;   // 던지기 포즈 (01, 02)

    private bool isAngry = false;
    private bool isThrowing = false;
    private int currentPoseIndex = 0;
    private int currentAngryIndex = 0;
    private float poseChangeInterval = 1f;
    private float throwDuration = 0.2f;
    private Coroutine poseCoroutine;
    private Coroutine throwCoroutine;

    private void Start()
    {
        if (characterImage == null)
        {
            characterImage = GetComponent<Image>();
        }

        // 기본 포즈 애니메이션 시작
        StartPoseAnimation();
    }

    public void SetAngryState(bool angry)
    {
        isAngry = angry;
        if (isAngry)
        {
            if (poseCoroutine != null)
            {
                StopCoroutine(poseCoroutine);
            }
            poseCoroutine = StartCoroutine(AngryPoseAnimation());
        }
        else
        {
            if (poseCoroutine != null)
            {
                StopCoroutine(poseCoroutine);
            }
            StartPoseAnimation();
        }
    }

    public void ShowDodgeSprite()
    {
        if (characterImage != null && dodgeSprite != null)
        {
            characterImage.sprite = dodgeSprite;
        }
    }

    public void ShowThrowAnimation()
    {
        if (throwCoroutine != null)
        {
            StopCoroutine(throwCoroutine);
        }
        throwCoroutine = StartCoroutine(ThrowAnimationSequence());
    }

    private void StartPoseAnimation()
    {
        if (poseCoroutine != null)
        {
            StopCoroutine(poseCoroutine);
        }
        poseCoroutine = StartCoroutine(NormalPoseAnimation());
    }

    private IEnumerator NormalPoseAnimation()
    {
        while (true)
        {
            if (poseSprites != null && poseSprites.Length >= 2 && !isThrowing)
            {
                characterImage.sprite = poseSprites[currentPoseIndex];
                currentPoseIndex = (currentPoseIndex + 1) % 2;
            }
            yield return new WaitForSeconds(poseChangeInterval);
        }
    }

    private IEnumerator AngryPoseAnimation()
    {
        while (true)
        {
            if (angrySprites != null && angrySprites.Length >= 2 && !isThrowing)
            {
                characterImage.sprite = angrySprites[currentAngryIndex];
                currentAngryIndex = (currentAngryIndex + 1) % 2;
            }
            yield return new WaitForSeconds(poseChangeInterval);
        }
    }

    private IEnumerator ThrowAnimationSequence()
    {
        if (throwSprites != null && throwSprites.Length >= 2)
        {
            isThrowing = true;
            
            // 첫 번째 던지기 포즈
            characterImage.sprite = throwSprites[0];
            yield return new WaitForSeconds(throwDuration);
            
            // 두 번째 던지기 포즈
            characterImage.sprite = throwSprites[1];
            
            // 던지기 애니메이션이 끝나면 기본 상태로 복귀
            isThrowing = false;
            if (isAngry)
            {
                if (poseCoroutine != null)
                {
                    StopCoroutine(poseCoroutine);
                }
                poseCoroutine = StartCoroutine(AngryPoseAnimation());
            }
            else
            {
                StartPoseAnimation();
            }
        }
    }
} 