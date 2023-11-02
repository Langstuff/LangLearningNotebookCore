using NotebookLua;
using NotebookDatabase;
using Microsoft.EntityFrameworkCore;

using (var context = new NotebookContext())
{
    context.Database.Migrate();
}

var nb1 = new LuaNotebook("My Notebook");

nb1.Execute("lua", "print(123)");
nb1.Execute("text", "print(123)");
