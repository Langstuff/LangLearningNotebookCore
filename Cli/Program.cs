using LuaNotebookScripting;
using NotebookDB;
using System.Linq;
using Microsoft.EntityFrameworkCore;

var nb1 = new LuaNotebook("My Notebook");

using (var context = new NotebookContext())
{
    context.Database.Migrate();
}

nb1.Execute("lua", "print(123)");
nb1.Execute("text", "print(123)");
