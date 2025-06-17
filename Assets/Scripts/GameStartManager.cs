using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    [Header("비디오 클립")]
    public VideoClip gameStartVideo;
    
    private bool isVideoFinished = false;
    private StageManager stageManager;
    private BGMManager bgmManager;
    private GameEndManager gameEndManager;
    private VideoPlayer videoPlayer;
    private RawImage videoScreen;
    
    void Start()
    {
        Debug.Log("GameStartManager 시작");
        
        // StageManager 찾기
        stageManager = FindFirstObjectByType<StageManager>();
        if (stageManager == null)
        {
            Debug.LogError("StageManager를 찾을 수 없습니다!");
        }
        
        // BGM 매니저 찾기
        bgmManager = FindFirstObjectByType<BGMManager>();
        if (bgmManager != null)
        {
            bgmManager.PlayBGM("Normal");
        }
        
        // GameEndManager 찾기
        gameEndManager = FindFirstObjectByType<GameEndManager>();
        if (gameEndManager != null)
        {
            // GameEndManager의 비디오 플레이어와 스크린 재사용
            videoPlayer = gameEndManager.videoPlayer;
            videoScreen = gameEndManager.videoScreen;
            
            if (videoScreen != null)
                videoScreen.gameObject.SetActive(true);
                
            if (videoPlayer != null)
            {
                videoPlayer.playOnAwake = true;
                videoPlayer.isLooping = false;
                
                // 비디오 재생 완료 이벤트 등록
                videoPlayer.loopPointReached += OnVideoFinished;
                
                // 시작 비디오 설정
                if (gameStartVideo != null)
                {
                    videoPlayer.clip = gameStartVideo;
                    videoPlayer.Play();
                    Debug.Log("인트로 비디오 재생 시작");
                }
                else
                {
                    Debug.LogError("GameStartVideo가 설정되지 않았습니다!");
                }
            }
        }
        else
        {
            Debug.LogError("GameEndManager를 찾을 수 없습니다!");
        }
    }
    
    void Update()
    {
        // 비디오가 끝나면 게임 시작
        if (isVideoFinished && Input.anyKeyDown)
        {
            StartGame();
        }
    }
    
    private void OnVideoFinished(VideoPlayer vp)
    {
        isVideoFinished = true;
        Debug.Log("인트로 비디오 재생 완료");
    }
    
    private void StartGame()
    {
        // 비디오 정지 및 UI 초기화
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
        }
            
        if (videoScreen != null)
        {
            videoScreen.gameObject.SetActive(false);
        }
            
        // 게임 시작
        if (stageManager != null)
        {
            // 모든 진행 상황 초기화
            stageManager.ResetAllProgress();
            // 게임을 처음부터 시작
            stageManager.RestartFromBeginning();
        }
        else
        {
            Debug.LogError("StageManager를 찾을 수 없어 게임을 시작할 수 없습니다!");
        }
        
        // GameStartManager 비활성화
        gameObject.SetActive(false);
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