using FPHorror.Gameplay;
using FPHorror.Gameplay.Player;
using FPHorror.Widget;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace FPHorror.Game
{
    public class GameFlowManager : MonoBehaviour
    {
        [Header("Parameters")] 
        [Tooltip("Duration of the fade-to-black at the end of the game")]
        public float EndSceneLoadDelay = 3f;

        [Tooltip("The canvas group of the fade-to-black screen")]
        public CanvasGroup EndGameFadeCanvasGroup;

        [Header("Win")] 
        [Tooltip("This string has to be the name of the scene you want to load when winning")]
        public string WinSceneName = "WinScene";

        [Tooltip("Duration of delay before the fade-to-black, if winning")]
        public float DelayBeforeFadeToBlack = 4f;

        [Tooltip("Win game message")]
        public string WinGameMessage = "You are safe!";

        [Tooltip("Duration of delay before the win message")]
        public float DelayBeforeWinMessage = 2f;

        [Tooltip("Sound played on win")] 
        public AudioClip VictorySound;

        [Header("Lose")] 
        [Tooltip("This string has to be the name of the scene you want to load when losing")]
        public string LoseSceneName = "LoseScene";

        [Tooltip("Lose game message")]
        public string LoseGameMessage = "You Died!";

        [Tooltip("Duration of delay before the lose message")]
        public float DelayBeforeLoseMessage = 1f;

        public bool GameIsEnding { get; private set; }

        private ObjectiveManager objectiveManager;

        float m_TimeLoadEndGameScene;
        string m_SceneToLoad;
        
        public CanvasGroup endGameCanvasGroup;
        public TextMeshProUGUI endGameText;

        void Awake()
        {
            EventManager.AddListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventManager.AddListener<PlayerDeathEvent>(OnPlayerDeath);
        }
        
        private void Start()
        {
            // Esconde o canvas no início
            endGameCanvasGroup.alpha = 0f;
            endGameCanvasGroup.interactable = false;
            endGameCanvasGroup.blocksRaycasts = false;

            // Obter o ObjectiveManager
            objectiveManager = FindObjectOfType<ObjectiveManager>();
        }

        void Update()
        {
            if (GameIsEnding)
            {
                float timeRatio = 1 - (m_TimeLoadEndGameScene - Time.time) / EndSceneLoadDelay;
                EndGameFadeCanvasGroup.alpha = timeRatio;

                // See if it's time to load the end scene (after the delay)
                if (Time.time >= m_TimeLoadEndGameScene)
                {
                    SceneManager.LoadScene(m_SceneToLoad);
                    GameIsEnding = false;
                }
            }
        }

        void OnAllObjectivesCompleted(AllObjectivesCompletedEvent evt) => EndGame(true);
        void OnPlayerDeath(PlayerDeathEvent evt) => EndGame(false);

        void EndGame(bool win)
        {
            // Desbloqueia o cursor antes de sair da cena para poder clicar em botões
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Lembre-se de que precisamos carregar a cena final apropriada após um atraso
            GameIsEnding = true;
            EndGameFadeCanvasGroup.gameObject.SetActive(true);

            if (win)
            {
                m_SceneToLoad = WinSceneName;
                m_TimeLoadEndGameScene = Time.time + EndSceneLoadDelay + DelayBeforeFadeToBlack;

                // Toca um som de vitória
                if (VictorySound != null)
                {
                    var audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.clip = VictorySound;
                    audioSource.playOnAwake = false;
                    audioSource.PlayScheduled(AudioSettings.dspTime + DelayBeforeWinMessage);
                }

                // Exibe a mensagem de vitória e mostra o canvas
                ShowEndGameScreen(WinGameMessage, Color.white, Color.yellow);
            }
            else
            {
                m_SceneToLoad = LoseSceneName;
                m_TimeLoadEndGameScene = Time.time + EndSceneLoadDelay;

                // Exibe a mensagem de derrota e mostra o canvas
                ShowEndGameScreen(LoseGameMessage, Color.black, Color.red);
            }
        }

        void ShowEndGameScreen(string message, Color backgroundColor, Color textColor)
        {
            endGameCanvasGroup.alpha = 1f;
            endGameCanvasGroup.interactable = true;
            endGameCanvasGroup.blocksRaycasts = true;
            endGameCanvasGroup.GetComponent<Image>().color = backgroundColor;
            endGameText.text = message;
            endGameText.color = textColor;

            // Pausar o jogo ao exibir a tela de fim de jogo
            Time.timeScale = 0f;
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventManager.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
        }
    }
}
