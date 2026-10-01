using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO.Compression;

namespace FontCreeper
{
    internal class Packager
    {
        /// <summary>
        /// 清理用户输入的路径：去掉从网页/剪贴板粘贴时可能混入的不可见字符
        /// （双向文本控制符、零宽字符等），并去掉首尾空白。
        /// </summary>
        public static string NormalizePath(string? path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder(path.Length);
            foreach (char c in path)
            {
                if (c is '\u200B' or '\u200C' or '\u200D' or '\u200E' or '\u200F' or '\uFEFF'
                    || (c >= '\u202A' && c <= '\u202E')
                    || (c >= '\u2066' && c <= '\u2069'))
                {
                    continue;
                }
                sb.Append(c);
            }
            return sb.ToString().Trim();
        }

        string generateDefaultJson(int size=11, int oversample=4,int shift1=0,int shift2=0)
        {
            return $$"""
            {
                "providers": [
                    {
                        "type": "ttf",
                        "file": "minecraft:custom.ttf",
                        "shift": [
                            {{shift1}},
                            {{shift2}}
                        ],
                        "size": {{size}},
                        "oversample": {{oversample}}
                    }
                ]
            }
            """;
        }
        string generatePackMcmeta(int packFormat,string desc= "这是字体包描述")
        {
            return $$"""
            {
                "pack": {
                    "pack_format": {{packFormat}},
                    "supported_formats": {
                        "min_inclusive": 4,
                        "max_inclusive": 34
                    },
                    "description": "{{desc}}"
                }
            }
            """;
        }
        public void package(string pkgName,string dir,string fontFile,int packFormat,string desc="这是字体包描述")
        {
            string pkgRoot = Path.Combine(Path.GetTempPath(), "FontCreeper."+pkgName+".tmp");

            if (!Directory.Exists(pkgRoot))
            {
                Directory.CreateDirectory(pkgRoot);
            }

            // 创建文件并写入一些文本
            using (FileStream fs = File.Create(Path.Combine(pkgRoot, "pack.mcmeta")))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(generatePackMcmeta(packFormat));
                fs.Write(info, 0, info.Length);
            }

            Directory.CreateDirectory(Path.Combine(pkgRoot, "assets"));
            Directory.CreateDirectory(Path.Combine(pkgRoot, "assets", "minecraft"));
            Directory.CreateDirectory(Path.Combine(pkgRoot, "assets", "minecraft", "font"));

            System.IO.File.Copy(fontFile, Path.Combine(pkgRoot, "assets", "minecraft", "font", "custom.ttf"), true);

            // 创建文件并写入一些文本
            using (FileStream fs = File.Create(Path.Combine(pkgRoot, "assets", "minecraft", "font", "default.json")))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(generateDefaultJson());
                fs.Write(info, 0, info.Length);
            }


            string sourceDir = pkgRoot;
            string zipPath = Path.Combine(dir, pkgName + ".zip");

            ZipFile.CreateFromDirectory(sourceDir, zipPath, CompressionLevel.Optimal, false);

            Directory.Delete(pkgRoot, true);
        }
    }
}
