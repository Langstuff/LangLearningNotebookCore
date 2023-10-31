namespace LuaNotebookScripting;
using NLua;

public class LuaNotebook
{
    public Lua lua = LuaStateMaker.MakeLuaState();
    public LuaNotebook()
    {

    }

    public void Execute(string luaCode)
    {
        lua.DoString(luaCode);
    }
}
