using Eric.Scenes;
using UnityEngine;

namespace Key.Scripts.Singletone {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverUI;

        [Header("Stage And Waves")]
        [SerializeField] private int currentStage;
        [SerializeField] private int currentWave;
        [SerializeField] private int maxStage;
        [SerializeField] private int maxWaves;

        private bool _isGameOver;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            Time.timeScale = 1f;

            if (gameOverUI != null)
                gameOverUI.SetActive(false);
        }

        public void GameOver() {
            if (_isGameOver)
                return;

            _isGameOver = true;

            if (gameOverUI != null)
                gameOverUI.SetActive(true);

            Time.timeScale = 0f;
        }

        public void Retry() {
            ChangeScene(SceneType.Game);
        }

        public void Main() {
            ChangeScene(SceneType.MainMenu);
        }

        public void Lobby() {
            ChangeScene(SceneType.Lobby);
        }

        public void ChangeScene(SceneType sceneType) {
            if (SceneChanger.Instance == null) {
                return;
            }

            Time.timeScale = 1f;
            SceneChanger.Instance.ChangeSceneState(sceneType);
        }

        private void OnDestroy() {
            if (Instance == this)
                Instance = null;
        }
    }
}