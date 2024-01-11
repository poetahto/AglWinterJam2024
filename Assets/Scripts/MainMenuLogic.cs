using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Ltg8
{
    public class MainMenuLogic : MonoBehaviour
    {
        public GameObject loadGameButton;
        
        private void Start()
        {
            if (!Game.Serializer.SaveExists(Game.PersistentSave.CurrentSaveId))
                loadGameButton.SetActive(false);
        }

        public void HandleNewGame()
        {
            NewGameTask().Forget();
        }

        public void HandleLoadGame()
        {
            LoadGameTask().Forget();
        }

        private async UniTask NewGameTask()
        {
            await SceneManager.UnloadSceneAsync(gameObject.scene);
            Game.Save = new OverworldSaveData();
            await Game.Overworld.LoadFromSave();
        }

        private async UniTask LoadGameTask()
        {
            await SceneManager.UnloadSceneAsync(gameObject.scene);
            await GameUtil.Load();
            await Game.Overworld.LoadFromSave();
        }

        public void HandleQuit()
        {
            Application.Quit();
        }
    }
}
