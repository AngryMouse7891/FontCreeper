using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.IO.Compression;

namespace FontCreeper
{
    internal class Packager
    {
        string generateDefaultJson(int size=11, int oversample=4,int shift1=0,int shift2=0)
        {
            return string.Format("""
            {
                "providers": [
                    {
                        "type": "ttf",
                        "file": "minecraft:custom.ttf",
                        "shift": [
                            {0},
                            {1}
                        ],
                        "size": {2},
                        "oversample": {3}
                    }
                ]
            }
            """, shift1, shift2,size,oversample);
        }
        string generatePackMcmeta(int packFormat,string desc= "这是字体包描述")
        {
            return string.Format("""
            {
                "pack": {
                    "pack_format": {0},
                    "supported_formats": {
                        "min_inclusive": 4,
                        "max_inclusive": 34
                    },
                    "description": "{1}"
                }
            }
            """,packFormat,desc);
        }
        public void package(string pkgName,string dir,string fontFile,int packFormat,string desc="这是字体包描述")
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                                               .IsOSPlatform(OSPlatform.Windows);
            string tempDir;
            if (isWindows)
            {
                tempDir = Environment.GetEnvironmentVariable("Temp");
            }
            else
            {
                tempDir = "/tmp";
            }

            string pkgRoot;

            pkgRoot = tempDir + pkgName;

            if (!Directory.Exists(pkgRoot))
            {
                Directory.CreateDirectory(pkgRoot);
            }

            // 创建文件并写入一些文本
            using (FileStream fs = File.Create(pkgRoot + "/pack.mcmeta"))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(generatePackMcmeta(packFormat));
                fs.Write(info, 0, info.Length);
            }

            Directory.CreateDirectory(pkgRoot + "/assets");
            Directory.CreateDirectory(pkgRoot + "/assets/minecraft");
            Directory.CreateDirectory(pkgRoot + "/assets/minecraft/font");

            System.IO.File.Copy(fontFile, pkgRoot + "/assets/minecraft/font/custom.ttf", true);

            // 创建文件并写入一些文本
            using (FileStream fs = File.Create(pkgRoot + "/assets/minecraft/font/default.json"))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(generateDefaultJson());
                fs.Write(info, 0, info.Length);
            }


            string sourceDir = pkgRoot;
            string zipPath = string.Format(@"{}/{}.zip",dir,pkgName);

            ZipFile.CreateFromDirectory(sourceDir, zipPath, CompressionLevel.Optimal, false);

            Directory.Delete(pkgRoot, true);
        }
    }
}
