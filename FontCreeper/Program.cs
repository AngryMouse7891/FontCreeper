using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

using IApplication app = Application.Create();
app.Init();

using Window window = new() { Title = "Hello World (Esc to quit)" };
Label label = new()
{
    Text = "Hello, Terminal.Gui v2!",
    X = Pos.Center(),
    Y = Pos.Center()
};
window.Add(label);

app.Run(window);