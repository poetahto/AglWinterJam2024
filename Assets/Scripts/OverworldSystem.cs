using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Ltg8
{
    public class OverworldSystem : MonoBehaviour
    {
        public async UniTask LoadLevel(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            Game.ItemSystem.SpawnSavedItems();
        }
    }
}
