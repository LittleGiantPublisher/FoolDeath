using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace F
{
    public static class SceneController
    {
        public static async Task ChangeScene(string sceneName, float transitionTime = 0f, LoadSceneMode mode = LoadSceneMode.Single)
        {
            Time.timeScale = 1;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, mode);
            asyncLoad.allowSceneActivation = false;

            // Tempo inicial para garantir o tempo mínimo de transição
            float startTime = Time.time;

            // Aguarda até que a cena seja carregada (90%)
            while (!asyncLoad.isDone && asyncLoad.progress < 0.9f)
            {
                await Task.Yield();
            }

            // Calcula o tempo restante necessário para atingir o transitionTime
            float elapsedTime = Time.time - startTime;
            float remainingTime = Mathf.Max(transitionTime - elapsedTime, 0);

            if (remainingTime > 0)
            {
                await Task.Delay((int)(remainingTime * 1000));
            }

            // Ativa a cena após o tempo mínimo de transição
            asyncLoad.allowSceneActivation = true;
        }
    }
}
