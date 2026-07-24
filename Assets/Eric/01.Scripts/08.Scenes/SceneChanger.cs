using System;
using Eric.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eric.Scenes
{
        [Serializable]
        public enum SceneType
        {
                MainMenu,
                Game,
                GameOver,
                Ending,
        }
        public class SceneChanger : MonoBehaviour
        {
                public static SceneChanger Instance{get; private set;}
                
                private SaveManager _saveManager;

                private void OnEnable()
                {
                        if (Instance == null)
                                Instance = this;
                        
                        DontDestroyOnLoad(gameObject);
                }

                private void ChangeSceneState(SceneType sceneType)
                {
                        string sceneName = sceneType switch
                        {
                                SceneType.MainMenu => "EnemyScene",
                                SceneType.Game => "GameScene",
                                SceneType.GameOver => "GameOverScene",
                                SceneType.Ending => "EndingScene",
                                _ => ""
                        };
                        if (sceneName == "") return;
                        SceneManager.LoadScene(sceneName);
                }
                
                public void GoToMainMenu()
                {
                        ChangeSceneState(SceneType.MainMenu);
                }

                public void GoToGame()
                {
                        ChangeSceneState(SceneType.Game);
                }

                public void GoToGameOver()
                {
                        ChangeSceneState(SceneType.GameOver);
                }

                public void GoToEnding()
                {
                        ChangeSceneState(SceneType.Ending);
                }

                public void EndGame()
                {
                        if (_saveManager != null)
                                _saveManager.InvokeSave();
                        Application.Quit();
                }
        }
}