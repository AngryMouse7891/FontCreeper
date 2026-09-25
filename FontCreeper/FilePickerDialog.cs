using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace FontCreeper
{
    /// <summary>
    /// 文件选择对话框，通过 SelectedPath 属性获取选择的文件路径
    /// </summary>
    public class FilePickerDialog : Dialog
    {
        /// <summary>
        /// 用户确认后返回的文件完整路径，取消时为空字符串
        /// </summary>
        public string SelectedPath { get; private set; } = string.Empty;

        private readonly TextField _pathField;
        private readonly ListView _fileList;
        private readonly CheckBox _showHidden;
        private readonly List<string> _items = new();
        private string _currentDir;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="startPath">初始目录，默认为用户主目录</param>
        public FilePickerDialog(string? startPath = null)
        {
            Title = "选择文件";
            Width = 70;
            Height = 24;

            _currentDir = string.IsNullOrEmpty(startPath)
                ? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
                : Path.GetFullPath(startPath);

            if (!Directory.Exists(_currentDir))
                _currentDir = Path.GetPathRoot(_currentDir) ?? _currentDir;

            // 路径输入框
            var pathLabel = new Label { Text = "路径:", X = 1, Y = 1 };
            _pathField = new TextField
            {
                Text = _currentDir,
                X = 7,
                Y = 1,
                Width = Dim.Fill(2)
            };
            _pathField.Accepted += (s, e) =>
            {
                NavigateTo(_pathField.Text ?? string.Empty);
                e.Handled = true;
            };

            // 文件列表
            _fileList = new ListView
            {
                X = 1,
                Y = 3,
                Width = Dim.Fill(2),
                Height = Dim.Fill(5)
            };
            _fileList.Accepted += (s, e) =>
            {
                OnItemActivated();
                e.Handled = true;
            };

            // 显示隐藏文件
            _showHidden = new CheckBox
            {
                Text = "显示隐藏文件",
                Value = CheckState.UnChecked,
                X = 1,
                Y = Pos.AnchorEnd(3)
            };
            _showHidden.ValueChanged += (s, e) => RefreshList();

            // 按钮
            var okBtn = new Button { Text = "确定", IsDefault = true };
            okBtn.Accepting += (s, e) =>
            {
                OnConfirm();
                e.Handled = true; // 防止事件冒泡导致对话框自动关闭
            };

            var cancelBtn = new Button { Text = "取消" };
            cancelBtn.Accepting += (s, e) =>
            {
                SelectedPath = string.Empty;
                RequestStop();
                e.Handled = true;
            };

            var parentBtn = new Button { Text = "上级目录" };
            parentBtn.Accepting += (s, e) =>
            {
                NavigateTo(Directory.GetParent(_currentDir)?.FullName ?? _currentDir);
                e.Handled = true;
            };

            var refreshBtn = new Button { Text = "刷新" };
            refreshBtn.Accepting += (s, e) =>
            {
                RefreshList();
                e.Handled = true;
            };

            AddButton(okBtn);
            AddButton(cancelBtn);
            AddButton(parentBtn);
            AddButton(refreshBtn);

            Add(pathLabel, _pathField, _fileList, _showHidden);

            RefreshList();
        }

        private void NavigateTo(string path)
        {
            try
            {
                path = Packager.NormalizePath(path);

                if (File.Exists(path))
                {
                    SelectedPath = path;
                    RequestStop();
                    return;
                }

                if (Directory.Exists(path))
                {
                    _currentDir = Path.GetFullPath(path);
                    _pathField.Text = _currentDir;
                    RefreshList();
                }
                else
                {
                    MessageBox.ErrorQuery(App!, "错误", "路径不存在", "确定");
                }
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery(App!, "错误", $"无法访问路径:\n{ex.Message}", "确定");
            }
        }

        private void RefreshList()
        {
            try
            {
                _items.Clear();

                var dirs = Directory.GetDirectories(_currentDir)
                    .Where(d => _showHidden.Value == CheckState.Checked || !new DirectoryInfo(d).Attributes.HasFlag(FileAttributes.Hidden))
                    .OrderBy(d => d);

                foreach (var dir in dirs)
                    _items.Add($"📁 {Path.GetFileName(dir)}");

                var files = Directory.GetFiles(_currentDir)
                    .Where(f => _showHidden.Value == CheckState.Checked || !new FileInfo(f).Attributes.HasFlag(FileAttributes.Hidden))
                    .OrderBy(f => f);

                foreach (var file in files)
                    _items.Add($"📄 {Path.GetFileName(file)}");

                _fileList.SetSource(new ObservableCollection<string>(_items));

                if (_items.Count > 0)
                    _fileList.SelectedItem = 0;
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery(App!, "错误", $"无法读取目录:\n{ex.Message}", "确定");
            }
        }

        private static string GetItemName(string item)
        {
            // 列表项格式为 "<图标> <名称>"，图标（📁/📄）是代理对，不能按固定长度截取
            var space = item.IndexOf(' ');
            return space >= 0 ? item[(space + 1)..] : item;
        }

        private void OnItemActivated()
        {
            var index = _fileList.SelectedItem ?? -1;
            if (index < 0 || index >= _items.Count) return;

            var selected = _items[index];
            var name = GetItemName(selected);
            var fullPath = Path.Combine(_currentDir, name);

            if (Directory.Exists(fullPath))
            {
                NavigateTo(fullPath);
            }
            else if (File.Exists(fullPath))
            {
                SelectedPath = fullPath;
                RequestStop();
            }
        }

        private void OnConfirm()
        {
            var index = _fileList.SelectedItem ?? -1;
            if (index >= 0 && index < _items.Count)
            {
                var selected = _items[index];
                var name = GetItemName(selected);
                var fullPath = Path.Combine(_currentDir, name);

                if (File.Exists(fullPath))
                {
                    SelectedPath = fullPath;
                    RequestStop();
                    return;
                }
            }

            var manualPath = Packager.NormalizePath(_pathField.Text);
            if (File.Exists(manualPath))
            {
                SelectedPath = Path.GetFullPath(manualPath);
                RequestStop();
                return;
            }

            MessageBox.Query(App!, "提示", "请选择一个文件", "确定");
        }
    }
}
