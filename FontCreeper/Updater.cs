using Newtonsoft.Json;
using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Semver;

// 对应根对象
public class GiteeRelease
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("tag_name")]
    public string TagName { get; set; }

    [JsonProperty("target_commitish")]
    public string TargetCommitish { get; set; }

    [JsonProperty("prerelease")]
    public bool Prerelease { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("body")]
    public string Body { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("author")]
    public GiteeAuthor Author { get; set; }

    [JsonProperty("assets")]
    public List<GiteeAsset> Assets { get; set; }
}

// 作者信息
public class GiteeAuthor
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("login")]
    public string Login { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; }

    [JsonProperty("html_url")]
    public string HtmlUrl { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    // 其他字段按需添加...
}

// 资源文件
public class GiteeAsset
{
    [JsonProperty("browser_download_url")]
    public string DownloadUrl { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    // 提取文件扩展名
    public string Extension => Name?.Substring(Name.LastIndexOf('.') + 1);

    // 判断文件类型
    public bool IsExecutable => Name?.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) == true;
    public bool IsArchive => Name?.EndsWith(".7z", StringComparison.OrdinalIgnoreCase) == true
                          || Name?.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) == true
                          || Name?.EndsWith(".rar", StringComparison.OrdinalIgnoreCase) == true
                          || Name?.EndsWith(".tar", StringComparison.OrdinalIgnoreCase) == true
                          || Name?.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase) == true
                          || Name?.EndsWith(".gz", StringComparison.OrdinalIgnoreCase) == true
                          || Name?.EndsWith(".xz", StringComparison.OrdinalIgnoreCase) == true
        ;
}

namespace FontCreeper
{
    internal class Updater
    {
        public async Task<GiteeRelease> GetLatestRelease<GiteeRelease>()
        {
            var client = new HttpClient();
            //client.DefaultRequestHeaders.Add(@"Content-Type", "application/json;charset=UTF-8");
            var request = new HttpRequestMessage(HttpMethod.Get, "https://gitee.com/api/v5/repos/AngryMouse7891/FontCreeper/releases/latest");
            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<GiteeRelease>(responseBody);
        }
        public string isLatest(string current)
        {
            var latestRelease = SemVersion.Parse(GetLatestRelease<GiteeRelease>().Result.TagName);
            var currentRelease= SemVersion.Parse(current);

            int res = SemVersion.CompareSortOrder(currentRelease, latestRelease);

            if (res == -1)
            {
                return latestRelease.ToString();
            }
            else return currentRelease.ToString();
        }
    }
}
