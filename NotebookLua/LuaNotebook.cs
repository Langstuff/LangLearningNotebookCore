namespace NotebookLua;

using System.ComponentModel.DataAnnotations;
using NLua;
using System.Reflection;
using System.Linq;
using NotebookDatabase;

public class LuaNotebook
{
    public uint messagesNumber;
    private string execResult = "";
    private LuaFunction tostring;
    private string name;
    public Lua lua = LuaStateMaker.MakeLuaState();
    NotebookContext context = new NotebookContext();
    Notebook notebookDbEntry;

    public LuaNotebook(string name)
    {
        var notebooksSearch = context.Notebooks
            .Where(n => n.NotebookName == name).ToList();
        if (notebooksSearch.Count > 0)
        {
            notebookDbEntry = notebooksSearch.First();
        }
        else
        {
            notebookDbEntry = new Notebook { NotebookName = name };
            context.Notebooks.Add(notebookDbEntry);
            context.SaveChanges();
        }

        this.name = name;
        tostring = lua.GetFunction("tostring");
        lua["print"] = (object)LuaPrint;
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
        else if (languageMode == "ai")
        {
            execResult = NativeLibs.Net.AI(input);
        }
        else
        {
            throw new Exception("Unknown language mode: " + languageMode);
        }
        var message = new Message
        {
            UserInput = input,
            NotebookId = notebookDbEntry.NotebookId,
            ExecutionResult = execResult,
            LanguageMode = languageMode,
        };
        context.Messages.Add(message);
        context.SaveChanges();
    }
}
