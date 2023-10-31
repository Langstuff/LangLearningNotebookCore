using NLua;
using System.Reflection;

namespace LuaNotebookScripting;

public static class LuaStateMaker
{
    public static void LoadLuaLibrary(ref Lua luaState, string name, string path)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "LuaNotebookScripting.LuaLibs." + path;

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            string result = reader.ReadToEnd();
            luaState[name] = luaState.DoString(result)[0];
        }
    }

    public static Lua MakeLuaState()
    {
        Lua luaState = new Lua();
        luaState.RegisterFunction("GetRequest", null,
            typeof(NativeLibs.Net).GetMethod("PerformGetRequest"));
        luaState.RegisterFunction("PostRequest", null,
            typeof(NativeLibs.Net).GetMethod("PerformPostRequest"));
        luaState.RegisterFunction("AI", null, typeof(NativeLibs.Net).GetMethod("AI"));
        LoadLuaLibrary(ref luaState, "json", "dkjson.lua");
        return luaState;
    }
}
