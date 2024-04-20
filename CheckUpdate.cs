using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace ContentWarningCheat
{
    internal class CheckUpdate
    {
        private static bool hasNewUpdate = false;
        public const string Version = "v1.2";

        public static void DrawHasNewUpdate()
        {
            if (!hasNewUpdate)
                return;
            Renderer.DrawColorString(new Vector2(Screen.width / 2, 20f), "作弊模组有新的版本可用！", Color.yellow, 20f);
        }

        public static async void CheckForUpdate()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Xiaodo-APP");
                client.Timeout = TimeSpan.FromSeconds(20);
                HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/xiaodo1337/Content-Warning-Cheat/releases/latest");

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.Log($"[小豆-内容警告] 从GitHub存储库获取最新版本成功.");
                    JObject json = JObject.Parse(responseBody);
                    hasNewUpdate = (json["tag_name"].ToString() != Version);
                    if(hasNewUpdate) Win32.ShellExecuteA(IntPtr.Zero, new StringBuilder("open"), new StringBuilder($@"https://github.com/xiaodo1337/Content-Warning-Cheat/releases/tag/{json["tag_name"].ToString()}"), new StringBuilder(), new StringBuilder(), 0);
                }
                else
                {
                    Debug.Log($"[小豆-内容警告] 从GitHub存储库获取最新版本失败.错误代码{response.StatusCode}");
                    hasNewUpdate = false;
                }
            }
        }
    }
}
