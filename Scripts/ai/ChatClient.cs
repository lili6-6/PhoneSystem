using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Diagnostics;

namespace Halabang.Blueberry.pp
{


    public class ChatClient : MonoBehaviour
    {
        [Header("百炼配置")]
         public string apiKey { get; private set; } = "sk-8feaa601bd574b8484b4a86f6218a1fd";     // sk-xxx
        public void changeApiKey(string apiK)
        {
            apiKey = apiK;
        }
        public string appId { get; private set; } = "1e5e3648a0d54dfc9e5895ff092e515e";      // 你的百炼应用ID
        public void changeAppId(string ID)
        {
            appId = ID;
        }
        private string ApiUrl =>
            $"https://dashscope.aliyuncs.com/api/v1/apps/{appId}/completion";

        public IEnumerator SendPrompt(
            string role,
    string prompt,
    System.Action<string> onReply,
    System.Action<string> onError = null
)
        {
            var sw = Stopwatch.StartNew();
            long last = 0;

            void Mark(string tag)
            {
                long now = sw.ElapsedMilliseconds;
                UnityEngine.Debug.Log(role + $"[AI Timer] {tag}: {now - last} ms");
                last = now;
            }

            // 1. 构造请求体
            var requestData = new BailianRequest
            {
                input = new InputData { prompt = prompt },
                parameters = new EmptyObject(),
                debug = new EmptyObject()
            };
            Mark("构造请求体");

            string json = JsonUtility.ToJson(requestData);
            Mark( "转json");

            // 2. 发送请求
            using var req = new UnityWebRequest(ApiUrl, "POST");
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            Mark("构建网络请求");

            yield return req.SendWebRequest();
            Mark("发送网络请求(Network + AI)");

            // 3. 错误处理
            if (req.result != UnityWebRequest.Result.Success)
            {
                onError?.Invoke(req.error);
                yield break;
            }

            // 4. 解析返回
            var response = JsonUtility.FromJson<BailianResponse>(
                req.downloadHandler.text
            );
            Mark("解析返回");

            onReply?.Invoke(response.output.text);

            sw.Stop();
            UnityEngine.Debug.Log(role + $"[AI Timer] TOTAL: {sw.ElapsedMilliseconds} ms");
        }

        // =========================
        // DTO 定义（必须在同一文件）
        // =========================

        [System.Serializable]
        private class BailianRequest
        {
            public InputData input;
            public object parameters;
            public object debug;
        }

        [System.Serializable]
        private class InputData
        {
            public string prompt;
        }

        [System.Serializable]
        private class BailianResponse
        {
            public OutputData output;
        }

        [System.Serializable]
        private class OutputData
        {
            public string text;
            public string session_id;
            public string finish_reason;
        }

        // Unity 的 JsonUtility 不喜欢匿名 object
        [System.Serializable]
        private class EmptyObject { }
    }
}