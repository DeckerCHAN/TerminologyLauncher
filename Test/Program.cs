using System;
using System.Collections.Generic;
using System.IO;
using TerminologyLauncher.Entities.InstanceManagement;
using TerminologyLauncher.Entities.SerializeUtils;
using TerminologyLauncher.GUI;
using TerminologyLauncher.GUI.ToolkitWindows.SingleSelect;

namespace TerminologyLauncher.Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var startupAugs = new InstanceStartupArgumentsEntity
            {
                JvmArguments = new List<string>{ "-XX:+UseConcMarkSweepGC", "-XX:+CMSIncrementalMode", "-XX:-UseAdaptiveSizePolicy" },
                MiniumMemoryMegaSize = 1600,
                Nativespath = "natives",
                LibraryPaths = new List<string>{ "libraries/net/minecraftforge/forge/1.7.10-10.13.2.1291/forge-1.7.10-10.13.2.1291.jar",
                    "libraries/net/minecraft/launchwrapper/1.11/launchwrapper-1.11.jar",
                    "libraries/org/ow2/asm/asm-all/5.0.3/asm-all-5.0.3.jar",
                    "libraries/com/typesafe/akka/akka-actor_2.11/2.3.3/akka-actor_2.11-2.3.3.jar",
                    "libraries/com/typesafe/config/1.2.1/config-1.2.1.jar",
                    "libraries/org/scala-lang/scala-actors-migration_2.11/1.1.0/scala-actors-migration_2.11-1.1.0.jar",
                    "libraries/org/scala-lang/scala-compiler/2.11.1/scala-compiler-2.11.1.jar",
                    "libraries/org/scala-lang/plugins/scala-continuations-library_2.11/1.0.2/scala-continuations-library_2.11-1.0.2.jar",
                    "libraries/org/scala-lang/plugins/scala-continuations-plugin_2.11.1/1.0.2/scala-continuations-plugin_2.11.1-1.0.2.jar",
                    "libraries/org/scala-lang/scala-library/2.11.1/scala-library-2.11.1.jar",
                    "libraries/org/scala-lang/scala-parser-combinators_2.11/1.0.1/scala-parser-combinators_2.11-1.0.1.jar",
                    "libraries/org/scala-lang/scala-reflect/2.11.1/scala-reflect-2.11.1.jar",
                    "libraries/org/scala-lang/scala-swing_2.11/1.0.1/scala-swing_2.11-1.0.1.jar",
                    "libraries/org/scala-lang/scala-xml_2.11/1.0.2/scala-xml_2.11-1.0.2.jar",
                    "libraries/net/sf/jopt-simple/jopt-simple/4.5/jopt-simple-4.5.jar",
                    "libraries/lzma/lzma/0.0.1/lzma-0.0.1.jar",
                    "libraries/com/mojang/realms/1.3.5/realms-1.3.5.jar",
                    "libraries/org/apache/commons/commons-compress/1.8.1/commons-compress-1.8.1.jar",
                    "libraries/org/apache/httpcomponents/httpclient/4.3.3/httpclient-4.3.3.jar",
                    "libraries/commons-logging/commons-logging/1.1.3/commons-logging-1.1.3.jar",
                    "libraries/org/apache/httpcomponents/httpcore/4.3.2/httpcore-4.3.2.jar",
                    "libraries/java3d/vecmath/1.3.1/vecmath-1.3.1.jar",
                    "libraries/net/sf/trove4j/trove4j/3.0.3/trove4j-3.0.3.jar",
                    "libraries/com/ibm/icu/icu4j-core-mojang/51.2/icu4j-core-mojang-51.2.jar",
                    "libraries/com/paulscode/codecjorbis/20101023/codecjorbis-20101023.jar",
                    "libraries/com/paulscode/codecwav/20101023/codecwav-20101023.jar",
                    "libraries/com/paulscode/libraryjavasound/20101123/libraryjavasound-20101123.jar",
                    "libraries/com/paulscode/librarylwjglopenal/20100824/librarylwjglopenal-20100824.jar",
                    "libraries/com/paulscode/soundsystem/20120107/soundsystem-20120107.jar",
                    "libraries/io/netty/netty-all/4.0.10.Final/netty-all-4.0.10.Final.jar",
                    "libraries/com/google/guava/guava/16.0/guava-16.0.jar",
                    "libraries/org/apache/commons/commons-lang3/3.2.1/commons-lang3-3.2.1.jar",
                    "libraries/commons-io/commons-io/2.4/commons-io-2.4.jar",
                    "libraries/commons-codec/commons-codec/1.9/commons-codec-1.9.jar",
                    "libraries/net/java/jinput/jinput/2.0.5/jinput-2.0.5.jar",
                    "libraries/net/java/jutils/jutils/1.0.0/jutils-1.0.0.jar",
                    "libraries/com/google/code/gson/gson/2.2.4/gson-2.2.4.jar",
                    "libraries/com/mojang/authlib/1.5.16/authlib-1.5.16.jar",
                    "libraries/org/apache/logging/log4j/log4j-api/2.0-beta9/log4j-api-2.0-beta9.jar",
                    "libraries/org/apache/logging/log4j/log4j-core/2.0-beta9/log4j-core-2.0-beta9.jar",
                    "libraries/org/lwjgl/lwjgl/lwjgl/2.9.1/lwjgl-2.9.1.jar",
                    "libraries/org/lwjgl/lwjgl/lwjgl_util/2.9.1/lwjgl_util-2.9.1.jar",
                    "libraries/org/lwjgl/lwjgl/lwjgl-platform/2.9.1/lwjgl-platform-2.9.1-natives-windows.jar;jinput-platform-2.0.5-natives-windows.jar",
                    "libraries/tv/twitch/twitch/5.16/twitch-5.16.jar",
                    "libraries/tv/twitch/twitch-platform/5.16/twitch-platform-5.16-natives-windows-32.jar",
                    "libraries/tv/twitch/twitch-external-platform/4.5/twitch-external-platform-4.5-natives-windows-32.jar"},
                MainJarPath = "1.7.10-Forge10.13.2.1291.jar",
                MainClass = "net.minecraft.launchwrapper.Launch",
                Version = "1.7.10",
                AssetsDir = "assets",
                AssetIndex = "1.7.10",
                UserType = "mojang",
                TweakClasses = new List<string> { "cpw.mods.fml.common.launcher.FMLTweaker" }
            };
            File.WriteAllText("arg", JsonConverter.ConvertToJson(startupAugs));
            Console.ReadKey();
        }
    }
}
