using System.Reflection;
using FontCreeper;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using IApplication app = Application.Create();
app.Init();

var assembly = Assembly.GetExecutingAssembly();
var version = assembly.GetName().Version;

using Window window = new() { Title = "FontCreeper MC字体包生成器 v"+version };

Label pkgNameLabel = new()
{
    Text = "生成的资源包名",
    X=5,
    Y=5
};

TextField pkgNameText = new()
{
    X=Pos.Right(pkgNameLabel)+1,
    Y=5,
    Width=20
};

Button BrowseDialog1 = new()
{
    X = Pos.Right(pkgNameText)+1,
    Y = 7,
    Text = "浏览……"
};

Label fontFileLabel = new()
{
    Text = "字体文件路径",
    X = 5,
    Y = 7
};

TextField fontFileText = new()
{
    X = Pos.Right(pkgNameLabel) + 1,
    Y = 7,
    Width = 20
};

Button BrowseDialog2 = new()
{
    X = Pos.Right(fontFileText)+1,
    Y = 9,
    Text = "浏览……"
};

Label outputDirLabel = new()
{
    Text = "打包输出目录",
    X = 5,
    Y = 9
};

TextField outputDirText = new()
{
    X = Pos.Right(pkgNameLabel) + 1,
    Y = 9,
    Width = 20
};

Label packFormatLabel = new()
{
    Text = "资源包版本格式",
    X = 5,
    Y = 11
};

TextField packFormatText = new()
{
    X = Pos.Right(pkgNameLabel) + 1,
    Y = 11,
    Width = 20
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
        MessageBox.ErrorQuery(button.App!, "❌错误", "资源包版本格式必须是数字！如果你不知道关于资源包版本格式的信息，请看\"https://zh.minecraft.wiki/w/资源包#资源包格式版本\"。", "确定");
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
            Packager pak = new Packager();
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
    var dialog = new FilePickerDialog();
    app.Run(dialog);
    if (!string.IsNullOrEmpty(dialog.SelectedPath))
    {
        fontFileText.Text = dialog.SelectedPath;
    }
};

app.Run(window);