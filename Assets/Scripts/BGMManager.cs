using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [System.Serializable]
    public class BGMData
    {
        public string stateName;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
    }
    
    [Header("BGM 설정")]
    public BGMData[] bgmList;
    public float fadeSpeed = 1f;
    public bool playOnAwake = true;
    
    [Header("현재 상태")]
    public string currentState = "";
    
    private AudioSource audioSource;
    private float targetVolume;
    
    void Awake()
    {
        // 씬 전환시에도 유지
        DontDestroyOnLoad(gameObject);
        
        // 이미 존재하는 BGMManager가 있다면 이 인스턴스는 제거
        var existingManagers = FindObjectsByType<BGMManager>(FindObjectsSortMode.None);
        if (existingManagers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        
        if (playOnAwake)
        {
            PlayBGM("Normal");
        }
    }
    
    void Update()
    {
        // 볼륨 페이드 처리
        if (audioSource.volume != targetVolume)
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.unscaledDeltaTime);
        }
    }
    
    public void PlayBGM(string stateName)
    {
        Debug.Log($"[BGMManager] PlayBGM 호출: {stateName}");
        if (currentState == stateName)
        {
            Debug.Log($"[BGMManager] 이미 {stateName} 상태입니다. 재생 생략");
            return;
        }
        
        BGMData bgmData = System.Array.Find(bgmList, x => x.stateName == stateName);
        if (bgmData != null)
        {
            currentState = stateName;
            audioSource.clip = bgmData.clip;
            targetVolume = bgmData.volume;
            Debug.Log($"[BGMManager] {stateName} BGMData 찾음, Clip: {audioSource.clip?.name}, Volume: {targetVolume}");
            
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                Debug.Log("[BGMManager] AudioSource.Play() 호출");
            }
        }
        else
        {
            Debug.LogWarning($"[BGMManager] {stateName} BGMData를 찾을 수 없음");
        }
    }
    
    public void StopBGM()
    {
        targetVolume = 0f;
    }
    
    public void PauseBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }
    
    public void ResumeBGM()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }
    
    public void SetVolume(float volume)
    {
        targetVolume = Mathf.Clamp01(volume);
    }
} 