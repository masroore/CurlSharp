--- Premake5 Dev -----

local name_suffix = ""
if _ACTION == 'gmake' then
    name_suffix = "-" .. _ACTION
end

local curlsharp_corelib_name = "CurlSharp" .. name_suffix

solution ("CurlSharp-" .. (_ACTION or ""))
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
project (curlsharp_corelib_name)
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
project ("EasyGet" .. name_suffix)
language "C#"
kind "ConsoleApp"
framework "4.5"
targetdir "./bin"

dependson { curlsharp_corelib_name }

files
{
    "./Samples/EasyGet/**.cs",
}

links
{
    "System",
    "System.Core",
    "./bin/" .. curlsharp_corelib_name,
}

