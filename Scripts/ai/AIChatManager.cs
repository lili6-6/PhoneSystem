using Halabang.Blueberry.pp;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Halabang.Blueberry.pp
{
    /// <summary>
    /// aimanager 控制收发消息
    /// </summary>
    public class AICallTrace
    {
        public string traceId;
        public int step;
        public string role;
    }

    public enum aiMode
    {
        chatTarget,//聊天对象
        myself,//自己
        StoryAI//剧情AI
    }
    public class AIChatManager : MonoBehaviour
    {
        public ChatClient _client => client;
        [SerializeField]private ChatClient client;
        [SerializeField] private PhoneDialogueController PhoneDialogueController;
        [SerializeField] private aiMode AIMode = aiMode.chatTarget;
        public string systemMessage {  get; private set; }
        public void changeSystemMessage(string message)
        {
            systemMessage = message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatMessage">聊天记录</param>
        /// <param name="text">当前消息</param>
        /// <param name="storyPrompt">剧情提示词</param>
        /// <param name="Sender">发送人</param>

        // 发送方法（公开）
        public void Send(List<ChatMessage> chatMessage, string text, string storyPrompt, string Sender)
        {
            // 每次调用生成唯一 trace
            var trace = new AICallTrace
            {
                traceId = Guid.NewGuid().ToString("N").Substring(0, 6),
                step = 1,
                role = AIMode.ToString() // chatTarget / myself / StoryAI
            };

            string finalPrompt = "";

            if (AIMode == aiMode.chatTarget)
            {
                finalPrompt = BuildTargetFinalPrompt(chatMessage, text, storyPrompt, Sender);
            }
            else if (AIMode == aiMode.myself)
            {
                finalPrompt = BuildMyFinalPrompt(chatMessage, text, storyPrompt, Sender);
            }
            else if (AIMode == aiMode.StoryAI)
            {
                finalPrompt = BuildStoryFinalPrompt(chatMessage, text, Sender);
            }

            // 调用 SendCoroutine 并传 trace
            StartCoroutine(SendCoroutine(finalPrompt,trace));
        }

        //拼提示词
        //给聊天对象的提示词
        public string BuildTargetFinalPrompt(List<ChatMessage> chatMessage, string userInput,string storyPrompt, string Sender)
        {
            int encounterDays =
                BlueberryManager.Instance
                    .CurrentPhoneManager
                    ._PhoneCalendarManager
                    .meetTime(
                        PhoneDialogueController._encounterWeek,
                        PhoneDialogueController._encounterDay
                    );

            string historyText = BuildChatHistoryText(chatMessage);

            return
        $@"{systemMessage}

【关系与时间信息】
你与玩家已经认识了大约 {encounterDays} 天。
你应当在对话中自然地体现这一相识时长带来的熟悉程度，
但不要刻意提及具体数字，除非玩家主动询问。

【故事发展】
以下是当前剧情发展的提示词，请基于这些内容继续对话，
{storyPrompt}
【近期对话记录】
以下是你与玩家的近期聊天内容，请基于这些内容及故事发展提示词继续对话，
不要重复已有信息。

{historyText}

【当前收到】
{Sender}：{userInput}";
        }

        //给自己的提示词
        public string BuildMyFinalPrompt(List<ChatMessage> chatMessage, string userInput, string storyPrompt, string Sender)
        {
            int encounterDays =
                BlueberryManager.Instance
                    .CurrentPhoneManager
                    ._PhoneCalendarManager
                    .meetTime(
                        PhoneDialogueController._encounterWeek,
                        PhoneDialogueController._encounterDay
                    );

            string historyText = BuildChatHistoryText(chatMessage);

            return
        $@"{systemMessage}

【关系与时间信息】
你与聊天对象已经认识了大约 {encounterDays} 天。
你应当在对话中自然地体现这一相识时长带来的熟悉程度，
但不要刻意提及具体数字，除非玩家主动询问。
【故事发展】
以下是当前剧情发展的大纲，请基于这些内容继续对话，
{storyPrompt}

【近期对话记录】
以下是你与聊天对象的近期聊天内容，请基于这些内容及故事发展提示词继续对话，
不要重复已有信息。

{historyText}

【当前输入】
{Sender}：{userInput}";
        }
        //给剧情AI的提示词
        public string BuildStoryFinalPrompt(List<ChatMessage> chatMessage, string userInput,string Sender)
        {
            int encounterDays =
                BlueberryManager.Instance
                    .CurrentPhoneManager
                    ._PhoneCalendarManager
                    .meetTime(
                        PhoneDialogueController._encounterWeek,
                        PhoneDialogueController._encounterDay
                    );

            string historyText = BuildChatHistoryText(chatMessage);

            return
        $@"{systemMessage}

【关系与时间信息】
玩家与聊天对象已经认识了大约 {encounterDays} 天。。

【近期对话记录】
以下是玩家与聊天对象的近期聊天内容，请基于故事剧情发展，给出接下来剧情的提示词。

{historyText}

【当前输入】
{Sender}：{userInput}";
        }
        //历史记录
        string BuildChatHistoryText(List<ChatMessage> history, int maxCount = 5)
        {
            StringBuilder sb = new StringBuilder();

            int start = Mathf.Max(0, history.Count-1 - maxCount);
            for (int i = start; i < history.Count-1; i++)
            {
                sb.AppendLine($"{history[i].nameStr}：{history[i].message}");
            }

            return sb.ToString();
        }
        // 内部协程封装
        private IEnumerator SendCoroutine(string prompt,AICallTrace trace)
        {
            Debug.Log("prompt" + prompt);
            var sw = System.Diagnostics.Stopwatch.StartNew();

            Debug.Log($"[AI START] [{trace.traceId}] Step {trace.step} | {trace.role}");
            yield return client.SendPrompt(
                trace.role,
                prompt,
                reply =>
                {
                    sw.Stop();
                    Debug.Log($"[AI END] [{trace.traceId}] Step {trace.step} | {trace.role} | {sw.ElapsedMilliseconds} ms");
                    if (AIMode == aiMode.chatTarget)
                    {
                        PhoneDialogueController.ReceiveMessage(reply);
                    }
                    else if (AIMode == aiMode.myself)
                    {
                        var data = ParseOptionJson(reply);
                        OnPlayerOptionsReady(data);
                    }
                    else if (AIMode == aiMode.StoryAI)
                    {
                        PhoneDialogueController.ReceiveStoryAIPrompt(reply);
                    }
                    Debug.Log("reply" + reply);

                },
                error =>
                {
                    Debug.LogError("请求失败: " + error);
                }
            );
        }
        private void OnPlayerOptionsReady(AIOptionResponse data)
        {
            if (data == null) return;

            PhoneDialogueController.UpdateOption(
                data.options[0].title,
                data.options[1].title,
                data.options[0].content,
                data.options[1].content
            );
        }

        private AIOptionResponse ParseOptionJson(string json)
        {
            try
            {
                var data = JsonUtility.FromJson<AIOptionResponse>(json);
                if (data == null || data.options == null || data.options.Length < 2)
                {
                    Debug.LogError("AI 返回的选项结构非法");
                    return null;
                }
                return data;
            }
            catch
            {
                Debug.LogError("AI JSON 解析失败");
                return null;
            }
        }


        [System.Serializable]
        public class AIOption
        {
            public string title;
            public string content;
        }

        [System.Serializable]
        public class AIOptionResponse
        {
            public AIOption[] options;
        }

    }
}