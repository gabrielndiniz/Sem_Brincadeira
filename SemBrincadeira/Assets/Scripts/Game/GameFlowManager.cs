using FPHorror.Gameplay;
using FPHorror.Widget;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        void Awake()
        {
            EventManager.AddListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventManager.AddListener<PlayerDeathEvent>(OnPlayerDeath);
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
            // unlocks the cursor before leaving the scene, to be able to click buttons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Remember that we need to load the appropriate end scene after a delay
            GameIsEnding = true;
            EndGameFadeCanvasGroup.gameObject.SetActive(true);

            if (win)
            {
                m_SceneToLoad = WinSceneName;
                m_TimeLoadEndGameScene = Time.time + EndSceneLoadDelay + DelayBeforeFadeToBlack;

                // play a sound on win
                if (VictorySound != null)
                {
                    var audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.clip = VictorySound;
                    audioSource.playOnAwake = false;
                    audioSource.PlayScheduled(AudioSettings.dspTime + DelayBeforeWinMessage);
                }

                // Display the win message
                ShowMessage(WinGameMessage, true);
            }
            else
            {
                m_SceneToLoad = LoseSceneName;
                m_TimeLoadEndGameScene = Time.time + EndSceneLoadDelay;

                // Display the lose message
                ShowMessage(LoseGameMessage, false);
            }
        }

        void ShowMessage(string message, bool win)
        {
            // Supondo que você tenha um script de widget que exiba mensagens na tela
            var widget = FindObjectOfType<MessageDisplay>();
            if (widget != null)
            {
                widget.ShowMessage(message, win);
            }
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<AllObjectivesCompletedEvent>(OnAllObjectivesCompleted);
            EventManager.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
        }
    }
}
