using System.Diagnostics;
using System.Reflection;
using FontCreeper;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using IApplication app = Application.Create();
app.Init();

const int ENTRY_DEFAULT_WIDTH = 40;

var assembly = Assembly.GetExecutingAssembly();
var version = assembly.GetName().Version;
var gitCommit=ThisAssembly.Git.Commit;
var gitCommits = ThisAssembly.Git.Commits;
var gitCommitDate = ThisAssembly.Git.CommitDate;
var gitBranch=ThisAssembly.Git.Branch;
var gitSha1 = ThisAssembly.Git.Sha;
var gitTag=ThisAssembly.Git.Tag;
var attribute = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
        .FirstOrDefault(x => x.Key == "BuildTime");

string asciiText = @$"███████╗ ██████╗ ███╗   ██╗████████╗ ██████╗██████╗ ███████╗███████╗██████╗ ███████╗██████╗ 
██╔════╝██╔═══██╗████╗  ██║╚══██╔══╝██╔════╝██╔══██╗██╔════╝██╔════╝██╔══██╗██╔════╝██╔══██╗
█████╗  ██║   ██║██╔██╗ ██║   ██║   ██║     ██████╔╝█████╗  █████╗  ██████╔╝█████╗  ██████╔╝
██╔══╝  ██║   ██║██║╚██╗██║   ██║   ██║     ██╔══██╗██╔══╝  ██╔══╝  ██╔═══╝ ██╔══╝  ██╔══██╗
██║     ╚██████╔╝██║ ╚████║   ██║   ╚██████╗██║  ██║███████╗███████╗██║     ███████╗██║  ██║
╚═╝      ╚═════╝ ╚═╝  ╚═══╝   ╚═╝    ╚═════╝╚═╝  ╚═╝╚══════╝╚══════╝╚═╝     ╚══════╝╚═╝  ╚═╝
                                                                                            {version}";

string gitMsg = $@"
Revision: {gitCommit}
Commits: {gitCommits}
Commit Date: {gitCommitDate}
Branch: {gitBranch}
SHA1: {gitSha1}
Tag: {gitTag}
";

string gitMsgShort = $"r{gitCommit} ● branch {gitBranch} ● tag {gitTag}";

Packager pak = new Packager();

using Window window = new() { Title = "FontCreeper MC字体包生成器 v"+version ,};

var menu = new MenuBar(
    new MenuBarItem[]
    {
        new MenuBarItem("_文件", new MenuItem[]
        {
            new MenuItem("_退出", "退出 FontCreeper", () => app.RequestStop())
        }),
        new MenuBarItem("_编辑", new MenuItem[]
        {
            new MenuItem("_复制", "", () => { }),
            new MenuItem("_粘贴", "", () => { })
        }),
        new MenuBarItem("_帮助", new MenuItem[]
        {
            new MenuItem("_Gitee 仓库", "打开该项目的Gitee仓库", () => openGiteeRepo()),
            new MenuItem("_Git 提交信息", "查看完整Git提交信息", () => MessageBox.Query(app, "Git 提交信息", gitMsg, "确定")),
            new MenuItem("_关于", "关于 FontCreeper", () => MessageBox.Query(app, "关于", $"{asciiText}\nFontCreeper是一个使用了.NET+Terminal.Gui的Minecraft Java版字体包制作工具。\nGit提交信息：{gitMsgShort}\n构建时间：{getBuildTime()}\nBy AngryMouse7891", "确定"))
        })
    });

void openGiteeRepo()
{
    // 指定要访问的 URL
    string url = "https://gitee.com/AngryMouse7891/mcparser";

    try
    {
        // 使用 Process 类的 Start 方法打开浏览器
        Process.Start(url);
    }
    catch (System.ComponentModel.Win32Exception noBrowser)
    {
        if (noBrowser.ErrorCode == -2147467259)
            MessageBox.ErrorQuery(app, "❌错误", "电脑上没浏览器！", "确定");
    }
    catch (Exception other)
    {
        MessageBox.ErrorQuery(app, "❌错误", $"打开网页失败！原因：{other.Message}", "确定");
    }
}

string getBuildTime()
{
    return attribute?.Value ?? "未知";
}

// 创建 OpenDialog
var ttfOpenDialog = new OpenDialog
{
    Title = "打开ttf/otf字体文件……",
    AllowedTypes = new() { new AllowedType("TrueType 字体", ".ttf"), new AllowedType("OpenType 字体", ".otf") }
};

Label pkgNameLabel = new()
{
    Text = "* 生成的资源包名",
    X=5,
    Y=5
};

TextField pkgNameText = new()
{
    X=Pos.Right(pkgNameLabel)+1,
    Y=5,
    Width=ENTRY_DEFAULT_WIDTH
};

Button BrowseDialog1 = new()
{
    X = Pos.Right(pkgNameText)+1,
    Y = 7,
    Text = "浏览……"
};

Label fontFileLabel = new()
{
    Text = "* 字体文件路径",
    X = 5,
    Y = 7
};

TextField fontFileText = new()
{
    X = Pos.Right(pkgNameLabel) + 1,
    Y = 7,
    Width = ENTRY_DEFAULT_WIDTH
};

Button BrowseDialog2 = new()
{
    X = Pos.Right(fontFileText)+1,
    Y = 9,
    Text = "浏览……"
};

Label outputDirLabel = new()
{
    Text = "* 打包输出目录",
    X = 5,
    Y = 9
};

TextField outputDirText = new()
{
    X = Pos.Right(pkgNameLabel) + 1,
    Y = 9,
    Width = ENTRY_DEFAULT_WIDTH,
    Text= Path.Combine(FontCreeper.Dirs.GetHomeDirectory(),"Documents")
};

// 创建 OpenDialog
var outputDirOpenDialog = new OpenDialog
{
    Title = "打开输出文件夹……",
    OpenMode=OpenMode.Directory
};

Label packFormatLabel = new()
{
    Text = "* 资源包版本格式",
    X = 5,
    Y = 11
};

TextField packFormatText = new()
{
    X = Pos.Right(pkgNameLabel) + 1,
    Y = 11,
    Width = ENTRY_DEFAULT_WIDTH
};

Button button = new()
{
    X = Pos.Right(packFormatLabel),
    Y = 20,
    Text="开始打包"
};

window.Add(pkgNameLabel);
window.Add(pkgNameText);
window.Add(BrowseDialog1);
window.Add(BrowseDialog2);
window.Add(fontFileLabel);
window.Add(fontFileText);
window.Add(outputDirText);
window.Add(outputDirLabel);
window.Add(packFormatText);
window.Add(packFormatLabel);
window.Add(button);
window.Add(menu);

button.Accepting += (sender, args) =>
{
    // 清理输入，去掉粘贴时可能混入的不可见字符（如 U+202A）
    string pkgName = Packager.NormalizePath(pkgNameText.Text);
    string fontFile = Packager.NormalizePath(fontFileText.Text);
    string outputDir = Packager.NormalizePath(outputDirText.Text);

    if (pkgName == "")
    {
        MessageBox.ErrorQuery(button.App!, "❌错误", "未输入生成的资源包名！", "确定");
    }
    else if (fontFile == "")
    {
        MessageBox.ErrorQuery(button.App!, "❌错误", "未输入字体文件路径！", "确定");
    }
    else if (outputDir == "")
    {
        MessageBox.ErrorQuery(button.App!, "❌错误", "未输入打包输出目录！", "确定");
    }
    else if (!int.TryParse(packFormatText.Text.Trim(), out int pf))
    {
        MessageBox.ErrorQuery(button.App!, "❌错误", "资源包版本格式必须是数字！\n如果你不知道关于资源包版本格式的信息，请看\"https://zh.minecraft.wiki/w/资源包#资源包格式版本\"。", "确定");
    }
    else if (int.Parse(packFormatText.Text)<=3)
    {
        MessageBox.ErrorQuery(button.App!, "❌错误", "不支持Minecraft 1.12.2及以下版本的字体包打包！\n如果你不知道关于资源包版本格式的信息，请看\"https://zh.minecraft.wiki/w/资源包#资源包格式版本\"。", "确定");
    }
    else if (!File.Exists(fontFile))
    {
        MessageBox.ErrorQuery(button.App!, "❌错误", $"找不到字体文件：\n{fontFile}", "确定");
    }
    else if (!Directory.Exists(outputDir))
    {
        MessageBox.ErrorQuery(button.App!, "❌错误", $"找不到打包输出目录：\n{outputDir}", "确定");
    }
    else 
    {
        try
        {
            pak.package(pkgName, outputDir, fontFile, pf);
            MessageBox.Query(button.App!, "打包完成", $"打包文件已放在{Path.Combine(outputDir, pkgName + ".zip")}", "确定");
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery(button.App!, "❌错误", $"打包失败：\n{ex.Message}", "确定");
        }
    }
    args.Handled = true; // 防止事件冒泡
};

BrowseDialog1.Accepting += (sender, args) =>
{
    // 模态运行
    app.Run(ttfOpenDialog);
    // 获取结果
    if (ttfOpenDialog.FilePaths.Count > 0)
    {
        string selectedFile = ttfOpenDialog.FilePaths[0];
        fontFileText.Text=selectedFile;
    }
};

BrowseDialog2.Accepting += (sender, args) =>
{
    // 模态运行
    app.Run(outputDirOpenDialog);
    // 获取结果
    if (outputDirOpenDialog.FilePaths.Count > 0)
    {
        string selectedDir = outputDirOpenDialog.FilePaths[0];
        outputDirText.Text = selectedDir;
    }
};

app.Run(window);