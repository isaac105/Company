using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GameEndManager : MonoBehaviour
{
    [Header("비디오 플레이어 설정")]
    public VideoPlayer videoPlayer;
    public RawImage videoScreen;
    
    [Header("UI 설정")]
    public Text restartText;
    public float textBlinkSpeed = 1f; // 텍스트 깜빡임 속도
    public int textFontSize = 60; // 텍스트 크기
    
    [Header("비디오 클립")]
    public VideoClip gameOverVideo;
    public VideoClip gameClearVideo;
    
    private bool isGameEnded = false;
    private float blinkTimer = 0f;
    private bool isGameCleared = false; // 게임 클리어 상태 추가
    
    void Start()
    {
        // 초기 설정
        if (videoScreen != null)
            videoScreen.gameObject.SetActive(false);
            
        if (restartText != null)
        {
            restartText.text = "R 버튼을 눌러 재시작";
            // restartText.fontSize = textFontSize;
            restartText.gameObject.SetActive(false);
        }
        
        // 비디오 플레이어 설정
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = false; // 반복 재생 끄기
            
            // 비디오 재생 완료 이벤트 등록
            videoPlayer.loopPointReached += OnVideoFinished;
            
            // 비디오 렌더링 대상 설정
            if (videoScreen != null)
            {
                videoPlayer.targetTexture = new RenderTexture(1080, 1920, 24);
                videoScreen.texture = videoPlayer.targetTexture;
            }
        }
    }
    
    void Update()
    {
        if (isGameEnded)
        {
            // 텍스트 깜빡임 효과
            if (restartText != null)
            {
                blinkTimer += Time.deltaTime * textBlinkSpeed;
                float alpha = Mathf.PingPong(blinkTimer, 1f);
                Color textColor = restartText.color;
                textColor.a = alpha;
                restartText.color = textColor;
            }
            
            // R 키로 재시작
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
    }
    
    public void ShowGameOver()
    {
        if (!isGameEnded)
        {
            isGameEnded = true;
            isGameCleared = false;
            PlayEndVideo(gameOverVideo);
        }
    }
    
    public void ShowGameClear()
    {
        if (!isGameEnded)
        {
            isGameEnded = true;
            isGameCleared = true;
            PlayEndVideo(gameClearVideo);
        }
    }
    
    private void PlayEndVideo(VideoClip clip)
    {
        if (videoPlayer != null && clip != null)
        {
            videoPlayer.clip = clip;
            videoPlayer.Play();
            
            if (videoScreen != null)
                videoScreen.gameObject.SetActive(true);
                
            if (restartText != null)
                restartText.gameObject.SetActive(true);
        }
    }
    
    // 비디오 재생 완료 시 호출되는 이벤트 핸들러
    private void OnVideoFinished(VideoPlayer vp)
    {
        // 마지막 프레임을 화면에 유지
        if (videoPlayer != null)
        {
            videoPlayer.frame = (long)videoPlayer.frameCount - 1;
            videoPlayer.Pause();
        }
    }
    
    private void RestartGame()
    {
        // 비디오 정지 및 UI 초기화
        if (videoPlayer != null)
            videoPlayer.Stop();
            
        if (videoScreen != null)
            videoScreen.gameObject.SetActive(false);
            
        if (restartText != null)
            restartText.gameObject.SetActive(false);
            
        isGameEnded = false;
        
        // 게임 재시작 처리
        var stageManager = FindFirstObjectByType<StageManager>();
        var combatManager = FindFirstObjectByType<CombatManager>();
        
        if (isGameCleared)
        {
            // 게임 클리어 후 재시작: 모든 진행 상황 초기화 및 첫 스테이지로
            if (stageManager != null)
            {
                stageManager.ResetAllProgress();
                stageManager.RestartFromBeginning();
            }
            isGameCleared = false;
        }
        else
        {
            // 게임 오버 후 재시작: 현재 스테이지 재시작
            if (combatManager != null)
            {
                combatManager.ResetCombat();
            }
        }
    }
    
    private void OnDestroy()
    {
        // 이벤트 핸들러 제거
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
} 