using NotebookLua;
using NotebookDatabase;
using Microsoft.EntityFrameworkCore;
using System.Text;

static void setupDatabase() {
    using (var context = new NotebookContext())
    {
        context.Database.Migrate();
    }

    using (var context = new FlashcardContext())
    {
        context.Database.Migrate();
    }
}

setupDatabase();

var nb1 = new LuaNotebook("My Notebook");

nb1.Execute("lua", @"
response = AI(
    'Give me JSON array with pairs question-answer, questions being 5 most common words for food ingredients in Japanese and answers being English translations.',
    'Example response [{""front"": ""肉"", ""back"": ""meat""}]'
)
io.write('A\n')
print(response)
io.write('B\n')
io.write(response)
io.write('C\n')
ok, response_json = pcall(json.decode, response)
io.write('D\n')
if not ok then
  io.write(response_json)
else
  io.write('Okay, doing the thing\n')
  io.write(type(response_json) .. '\n')
  save_deck('jp-en food deck', response_json)
end
");
// nb1.Execute("lua", "print(response)");
// nb1.Execute("lua", "io.write(response)");
// nb1.Execute("lua", "response_json = json.decode(encode)");
// nb1.Execute("lua", "save_deck('deck1', response_json)");

// nb1.Execute("lua", "response_json = json.decode(response)");
// nb1.Execute("lua", @"
// for i = 1, #response_json do
//   print(response_json[i].question .. ' | ' .. response_json[i].answer)
// end
// ");
// nb1.Execute("ai", @"Give me JSON array with pairs question-answer, questions
// being 50 most common words for food ingredients in Czech and answers being English translations.
// Response should be only JSON array and nothing else.");
