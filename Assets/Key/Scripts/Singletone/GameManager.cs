using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Key.Scripts.Singletone {
    public class GameManager : MonoBehaviour {
        public static GameManager Instance { get; private set; }

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private float gameOverFadeDuration = 0.3f;

        [Header("Scene Fade")]
        [SerializeField] private Image sceneFadeImage;
        [SerializeField] private float sceneFadeDuration = 0.5f;

        private Graphic[] _gameOverGraphics;
        private float[] _gameOverOriginalAlphas;

        private Coroutine _gameOverFadeCoroutine;
        private bool _isTransitioning;

        private void Awake() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            CacheGameOverGraphics();

            if (gameOverUI != null)
                gameOverUI.SetActive(false);

            if (sceneFadeImage != null) {
                SetGraphicAlpha(sceneFadeImage, 0f);
                sceneFadeImage.raycastTarget = false;
                sceneFadeImage.gameObject.SetActive(false);
            }
        }

        private void CacheGameOverGraphics() {
            if (gameOverUI == null)
                return;

            _gameOverGraphics = gameOverUI.GetComponentsInChildren<Graphic>(true);
            _gameOverOriginalAlphas = new float[_gameOverGraphics.Length];

            for (int i = 0; i < _gameOverGraphics.Length; i++)
                _gameOverOriginalAlphas[i] = _gameOverGraphics[i].color.a;
        }

        public void GameOver() {
            if (gameOverUI != null) {
                if (_gameOverFadeCoroutine != null) {
                    StopCoroutine(_gameOverFadeCoroutine);
                    _gameOverFadeCoroutine = null;
                }

                gameOverUI.SetActive(true);
                RestoreGameOverAlpha();
            }

            Time.timeScale = 0f;
        }

        public void Retry() {
            string currentSceneName = SceneManager.GetActiveScene().name;
            StartSceneTransition(currentSceneName);
        }

        public void Main() {
            StartSceneTransition("Main");
        }

        public void ChangeScene(string sceneName) {
            StartSceneTransition(sceneName);
        }

        private void StartSceneTransition(string sceneName) {
            if (_isTransitioning)
                return;

            StartCoroutine(SceneTransitionCoroutine(sceneName));
        }

        private IEnumerator SceneTransitionCoroutine(string sceneName) {
            _isTransitioning = true;

            if (gameOverUI != null && gameOverUI.activeSelf)
                DisappearUI();

            if (sceneFadeImage != null) {
                sceneFadeImage.gameObject.SetActive(true);
                sceneFadeImage.raycastTarget = true;

                SetGraphicAlpha(sceneFadeImage, 0f);

                yield return FadeGraphic(
                    sceneFadeImage,
                    1f,
                    sceneFadeDuration
                );
            }

            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }

        public void DisappearUI() {
            if (gameOverUI == null)
                return;

            if (_gameOverFadeCoroutine != null)
                StopCoroutine(_gameOverFadeCoroutine);

            _gameOverFadeCoroutine = StartCoroutine(
                DisappearUICoroutine()
            );
        }

        private IEnumerator DisappearUICoroutine() {
            if (_gameOverGraphics == null || _gameOverGraphics.Length == 0) {
                gameOverUI.SetActive(false);
                yield break;
            }

            float[] startAlphas = new float[_gameOverGraphics.Length];

            for (int i = 0; i < _gameOverGraphics.Length; i++) {
                if (_gameOverGraphics[i] != null)
                    startAlphas[i] = _gameOverGraphics[i].color.a;
            }

            float elapsedTime = 0f;

            while (elapsedTime < gameOverFadeDuration) {
                elapsedTime += Time.unscaledDeltaTime;

                float ratio = Mathf.Clamp01(
                    elapsedTime / gameOverFadeDuration
                );

                for (int i = 0; i < _gameOverGraphics.Length; i++) {
                    if (_gameOverGraphics[i] == null)
                        continue;

                    float alpha = Mathf.Lerp(
                        startAlphas[i],
                        0f,
                        ratio
                    );

                    SetGraphicAlpha(_gameOverGraphics[i], alpha);
                }

                yield return null;
            }

            gameOverUI.SetActive(false);
            RestoreGameOverAlpha();

            _gameOverFadeCoroutine = null;
        }

        private IEnumerator FadeGraphic(
            Graphic graphic,
            float targetAlpha,
            float duration
        ) {
            if (graphic == null)
                yield break;

            float startAlpha = graphic.color.a;

            if (duration <= 0f) {
                SetGraphicAlpha(graphic, targetAlpha);
                yield break;
            }

            float elapsedTime = 0f;

            while (elapsedTime < duration) {
                elapsedTime += Time.unscaledDeltaTime;

                float ratio = Mathf.Clamp01(
                    elapsedTime / duration
                );

                float alpha = Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    ratio
                );

                SetGraphicAlpha(graphic, alpha);

                yield return null;
            }

            SetGraphicAlpha(graphic, targetAlpha);
        }

        private void SetGraphicAlpha(Graphic graphic, float alpha) {
            if (graphic == null)
                return;

            Color color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }

        private void RestoreGameOverAlpha() {
            if (_gameOverGraphics == null || _gameOverOriginalAlphas == null)
                return;

            for (int i = 0; i < _gameOverGraphics.Length; i++) {
                if (_gameOverGraphics[i] == null)
                    continue;

                SetGraphicAlpha(
                    _gameOverGraphics[i],
                    _gameOverOriginalAlphas[i]
                );
            }
        }

        private void OnDestroy() {
            if (Instance == this)
                Instance = null;
        }
    }
}