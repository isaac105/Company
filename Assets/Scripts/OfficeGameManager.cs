using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace OfficeRevenge
{
    public class OfficeGameManager : MonoBehaviour
    {
        [Header("UI References")]
        public Text stageText;
        public Text healthText;
        public Button timingButton;
        public RectTransform timingIndicator;
        public RectTransform perfectZone;
        public Image playerCharacter;
        public Image enemyCharacter;
        public GameObject resultPopup;
        public Text resultText;

        [Header("Game Settings")]
        public float timingBarSpeed = 2f;
        public int maxHealth = 100;

        [Header("Stage Data")]
        public string[] bossNames = {"김 대리", "이 과장", "박 부장", "최 이사", "사장"};

        [Header("Items")]
        public string[] itemNames = {"얇은 보고서 뭉치", "뜨거운 커피잔", "스테이플러", "대용량 USB", "결재 서류철"};
        public int[] itemDamage = { 20, 25, 30, 35, 60 };

        private int currentStage = 0;
        private int playerHealth;
        private int enemyHealth;
        private bool isPlayerTurn = true;
        private bool isTimingActive = false;
        private float timingProgress = 0f;
        private int currentItem = 0;

        void Start()
        {
            InitializeGame();
            if (timingButton != null)
                timingButton.onClick.AddListener(OnTimingButtonPressed);
        }

        void InitializeGame()
        {
            playerHealth = maxHealth;
            enemyHealth = maxHealth;
            currentStage = 0;
            UpdateUI();
            SetupStage();
            if (resultPopup != null)
                resultPopup.SetActive(false);
        }

        void SetupStage()
        {
            if (stageText != null)
                stageText.text = "스테이지 " + (currentStage + 1) + " - " + bossNames[currentStage];
            isPlayerTurn = true;
            StartPlayerTurn();
        }

        void UpdateUI()
        {
            if (healthText != null)
                healthText.text = "HP: " + playerHealth + "/" + maxHealth;
        }

        void StartPlayerTurn()
        {
            isPlayerTurn = true;
            if (timingButton != null)
            {
                timingButton.interactable = true;
                Text buttonText = timingButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                    buttonText.text = "공격!";
            }
        }

        void StartEnemyTurn()
        {
            isPlayerTurn = false;
            if (timingButton != null)
            {
                timingButton.interactable = true;
                Text buttonText = timingButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                    buttonText.text = "방어!";
            }
            StartCoroutine(EnemyAttackSequence());
        }

        IEnumerator EnemyAttackSequence()
        {
            yield return new WaitForSeconds(1f);
            StartTimingChallenge();
        }

        public void OnTimingButtonPressed()
        {
            if (!isTimingActive)
                StartTimingChallenge();
            else
                CheckTiming();
        }

        void StartTimingChallenge()
        {
            isTimingActive = true;
            timingProgress = 0f;
            StartCoroutine(TimingBarAnimation());
        }

        IEnumerator TimingBarAnimation()
        {
            while (isTimingActive)
            {
                timingProgress += Time.deltaTime * timingBarSpeed;
                if (timingIndicator != null)
                {
                    float xPosition = Mathf.PingPong(timingProgress, 1f);
                    Vector2 anchorMin = timingIndicator.anchorMin;
                    Vector2 anchorMax = timingIndicator.anchorMax;
                    anchorMin.x = xPosition;
                    anchorMax.x = xPosition + 0.05f;
                    timingIndicator.anchorMin = anchorMin;
                    timingIndicator.anchorMax = anchorMax;
                }
                yield return null;
            }
        }

        void CheckTiming()
        {
            isTimingActive = false;
            float indicatorPosition = Mathf.PingPong(timingProgress, 1f);
            float perfectStart = perfectZone.anchorMin.x;
            float perfectEnd = perfectZone.anchorMax.x;
            bool isPerfect = indicatorPosition >= perfectStart && indicatorPosition <= perfectEnd;
            bool isGood = indicatorPosition >= perfectStart - 0.1f && indicatorPosition <= perfectEnd + 0.1f;

            if (isPlayerTurn)
                PlayerAttack(isPerfect, isGood);
            else
                PlayerDefense(isPerfect, isGood);
        }

        void PlayerAttack(bool isPerfect, bool isGood)
        {
            int damage = itemDamage[currentItem];
            if (isPerfect) damage = Mathf.RoundToInt(damage * 1.5f);
            else if (isGood) damage = Mathf.RoundToInt(damage * 1.2f);
            else damage = Mathf.RoundToInt(damage * 0.7f);

            enemyHealth -= damage;
            Debug.Log(itemNames[currentItem] + "로 " + damage + " 데미지!");


            if (enemyHealth <= 0)
                StageCleared();
            else
                StartEnemyTurn();
        }

        void PlayerDefense(bool isPerfect, bool isGood)
        {
            int incomingDamage = 25;
            if (isPerfect) incomingDamage = 0;
            else if (isGood) incomingDamage = Mathf.RoundToInt(incomingDamage * 0.5f);

            playerHealth -= incomingDamage;
            UpdateUI();

            if (playerHealth <= 0)
                GameOver();
            else
                StartPlayerTurn();
        }

        void StageCleared()
        {
            if (currentStage < itemNames.Length - 1)
                currentItem = currentStage + 1;
            currentStage++;

            if (currentStage >= bossNames.Length)
                GameWin();
            else
            {
                playerHealth = maxHealth;
                enemyHealth = maxHealth;
                SetupStage();
            }
        }

        void GameWin()
        {
            ShowResult("승리!", "드디어 사장 자리에 앉았습니다!");
        }

        void GameOver()
        {
            ShowResult("패배...", "서류 더미에 깔려버렸습니다...");
        }

        void ShowResult(string title, string message)
        {
            if (resultPopup != null && resultText != null)
            {
                resultText.text = title + " " + message;
                resultPopup.SetActive(true);
                if (timingButton != null)
                    timingButton.interactable = false;
            }
        }

        public void RestartGame()
        {
            InitializeGame();
        }
    }
}