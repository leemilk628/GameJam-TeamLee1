using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eric.Scenes
{
        [Serializable]
        public enum SceneType
        {
                MainMenu,
                Lobby,
                Game
        }
        public class SceneChanger : MonoBehaviour
        {
                public static SceneChanger Instance{get; private set;}

                private void OnEnable()
                {
                        if (Instance == null)
                                Instance = this;
                        
                        DontDestroyOnLoad(gameObject);
                }

                public void ChangeSceneState(SceneType sceneType)
                {
                        string sceneName = sceneType switch
                        {
                                SceneType.MainMenu => "MainMenuScene",
                                SceneType.Lobby => "LobbyScene",
                                SceneType.Game => "GameScene",
                                _ => ""
                        };
                        if (sceneName == "") return;
                        SceneManager.LoadScene(sceneName);
                }
        }
}