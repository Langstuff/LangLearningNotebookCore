namespace LuaNotebookScripting;

using System.ComponentModel.DataAnnotations;
using NLua;
using System.Reflection;
using System.Linq;
using NotebookDB;

public class LuaNotebook
{
    public uint messagesNumber;
    private string execResult;
    private LuaFunction tostring;
    private string name;
    public Lua lua = LuaStateMaker.MakeLuaState();
    NotebookContext context = new NotebookContext();
    Notebook notebookDb;

    public LuaNotebook(string name)
    {
        notebookDb = context.Notebooks
            .Where(n => n.NotebookName == name).First();

        this.name = name;
        tostring = lua.GetFunction("tostring");
        lua.RegisterFunction("print123", this, GetType().GetMethod("LuaPrint"));
    }

    protected void LuaPrint(params object[] args)
    {
        try
        {
            for (var i = 0; i < args.Length; i++)
            {
                object arg = args[i];
                try
                {
                    var callres = tostring.Call(arg);
                    execResult += (string)callres[0];
                    if (i == args.Length - 1)
                    {
                        execResult += '\n';
                    }
                    else
                    {
                        execResult += ' ';
                    }
                }
                catch (Exception e)
                {
                    execResult += '\n' + e.Message + '\n';
                }
            }
        }
        catch (Exception e)
        {
            execResult = e.Message;
        }
    }

    public void Execute(string languageMode, string input)
    {
        if (languageMode == "lua")
        {
            execResult = "";
            lua.DoString(input);
        }
        else if (languageMode == "text")
        {
            execResult = input;
        }
        var message = new Message
        {
            UserInput = input,
            NotebookId = notebookDb.NotebookId,
            ExecutionResult = execResult,
        };
        context.Messages.Add(message);
        context.SaveChanges();
    }
}
