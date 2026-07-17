using System.Reflection;
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

window.Add(pkgNameLabel);
window.Add(pkgNameText);
window.Add(fontFileLabel);
window.Add(fontFileText);

app.Run(window);