using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FontCreeper
{
    internal class Dirs
    {
        public static string GetHomeDirectory()
        {
            // 1. 优先使用 HOME 环境变量（macOS/Linux 都可靠）
            var home = Environment.GetEnvironmentVariable("HOME");

            // 2. 处理 Windows 兼容性（可选，如果你需要跨平台）
            if (string.IsNullOrEmpty(home))
            {
                home = Environment.GetEnvironmentVariable("USERPROFILE");
            }

            // 3. 回退到 .NET 内置方法（某些场景下可能返回空）
            if (string.IsNullOrEmpty(home))
            {
                home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }

            // 4. 兜底：抛出异常或返回当前目录（根据业务需求）
            if (string.IsNullOrEmpty(home))
            {
                throw new InvalidOperationException("Unable to determine user home directory.");
            }

            return home;
        }
    }
}
