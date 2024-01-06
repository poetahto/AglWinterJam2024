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
            Game.Save = await Game.Serializer.ReadFromDisk("dev_test");

            await SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentSceneName));
            Game.ItemSystem.SpawnSavedItems();
        }
    }
}
