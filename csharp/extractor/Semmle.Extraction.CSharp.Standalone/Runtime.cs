using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using Semmle.Util;

namespace Semmle.Extraction.CSharp.Standalone
{
    /// <summary>
    /// Locates .NET Runtimes.
    /// </summary>
    static class Runtime
    {
        static string ExecutingRuntime => RuntimeEnvironment.GetRuntimeDirectory();

        /// <summary>
        /// Locates .NET Core Runtimes.
        /// </summary>
        public static IEnumerable<string> CoreRuntimes
        {
            get
            {
                var dotnetPath = FileUtils.FindProgramOnPath(Win32.IsWindows() ? "dotnet.exe" : "dotnet");
                var dotnetDirs = dotnetPath != null
                    ? new[] { dotnetPath }
                    : new[] { "/usr/share/dotnet", @"C:\Program Files\dotnet" };
                var coreDirs = dotnetDirs.Select(d => Path.Combine(d, "shared", "Microsoft.NETCore.App"));

                foreach (var dir in coreDirs.Where(Directory.Exists))
                    return Directory.EnumerateDirectories(dir).OrderByDescending(Path.GetFileName);
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// Locates .NET Desktop Runtimes.
        /// This includes Mono and Microsoft.NET.
        /// </summary>
        public static IEnumerable<string> DesktopRuntimes
        {
            get
            {
                var monoPath = FileUtils.FindProgramOnPath(Win32.IsWindows() ? "mono.exe" : "mono");
                var monoDirs = monoPath != null
                    ? new[] { monoPath }
                    : new[] { "/usr/lib/mono", @"C:\Program Files\Mono\lib\mono" };

                if (Directory.Exists(@"C:\Windows\Microsoft.NET\Framework64"))
                {
                    return Directory.EnumerateDirectories(@"C:\Windows\Microsoft.NET\Framework64", "v*").
                        OrderByDescending(Path.GetFileName);
                }

                foreach (var dir in monoDirs.Where(Directory.Exists))
                {
                    return Directory.EnumerateDirectories(dir).
                        Where(d => Char.IsDigit(Path.GetFileName(d)[0])).
                        OrderByDescending(Path.GetFileName);
                }

                return Enumerable.Empty<string>();
            }
        }

        public static IEnumerable<string> Runtimes
        {
            get
            {
                foreach (var r in CoreRuntimes)
                    yield return r;

                foreach (var r in DesktopRuntimes)
                    yield return r;

                // A bad choice if it's the self-contained runtime distributed in odasa dist.
                yield return ExecutingRuntime;
            }
        }
    }
}
