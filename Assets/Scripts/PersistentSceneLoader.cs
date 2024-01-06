using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ltg8
{
    /// <summary>
    /// Ensures that the persistent scene is always loaded
    /// first when the game starts. 
    /// </summary>
    public static class PersistentSceneLoader
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static async void Initialize()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (currentSceneName != "persistent")
                SceneManager.LoadScene("persistent");

            // Wait one frame so entrypoint can initialize itself
            await UniTask.Yield();
            GameInitializer initializer = Object.FindAnyObjectByType<GameInitializer>();
            await initializer.Initialize();

#if UNITY_EDITOR
            if (Game.Settings.overworldSceneNames.Contains(currentSceneName))
            {
                // do stuff like spawning items, player, ect.
                await Game.Overworld.LoadLevel(currentSceneName);
            }
            else
            {
                // vanilla unity scene loading
                await SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive);
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentSceneName));
            }
#else
            await SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
#endif
        }
    }
}
