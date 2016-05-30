--- Premake5 Dev -----

solution "CurlSharp"
    configurations { "Debug", "Release" }
    location ("./build/" .. (_ACTION or ""))
    debugdir ("./bin")
    debugargs { "www.baidu.com" }
    defines { "LINUX" }
    clr "Unsafe"

configuration "Debug"
    flags { "Symbols" }
    defines { "DEBUG", "TRACE" }
configuration "Release"
    flags { "Optimize" }

------------------CurlSharp--------------------
project "CurlSharp"
language "C#"
kind "SharedLib"
framework "4.5"
targetdir "./bin"

files
{
    "./CurlSharp/**.cs",
}

links
{
    "System",
}

------------------EasyGet--------------------
project "EasyGet"
language "C#"
kind "ConsoleApp"
framework "4.5"
targetdir "./bin"

files
{
    "./Samples/EasyGet/**.cs",
}

links
{
    "System",
    "System.Core",
    "./bin/CurlSharp",
}

