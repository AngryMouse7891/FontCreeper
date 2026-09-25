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
    if (pkgNameText.Text == "")
    {
        MessageBox.ErrorQuery(button.App!, "❌错误", "未输入生成的资源包名！", "确定");
    }
    else if (fontFileText.Text == "")
    {
        MessageBox.ErrorQuery(button.App!, "❌错误", "未输入字体文件路径！", "确定");
    }
    else if (outputDirText.Text == "")
    {
        MessageBox.ErrorQuery(button.App!, "❌错误", "未输入打包输出目录！", "确定");
    }
    else if (packFormatText.Text == "")
    {
        MessageBox.ErrorQuery(button.App!, "❌错误", "未输入资源包版本格式！如果你不知道关于资源包版本格式的信息，请看\"https://zh.minecraft.wiki/w/资源包#资源包格式版本\"。", "确定");
    }
    else 
    {
        MessageBox.Query(button.App!, "打包完成", $"打包文件已放在D:/111.zip", "确定");
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