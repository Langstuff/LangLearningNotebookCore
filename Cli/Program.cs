using NotebookLua;
using NotebookDatabase;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Markdig;
using Markdig.Syntax;

static void setupDatabase() {
    using (var context = new NotebookContext())
    {
        context.Database.Migrate();
    }

    using (var context = new FlashcardContext())
    {
        context.Database.Migrate();
    }

    using (var context = new ArticleContext())
    {
        context.Database.Migrate();
    }
}

setupDatabase();

var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    // .UseGridTables()
    .UsePipeTables()
    // .EnableTrackTrivia()
    .Build();

var text = @"
This is a text with some *emphasis*
test
test
And a table:

| Column 1 | Column 2 |
|----------|----------|
| Test     | Test 2   |
| Test     | Test 2   |

";

var document = Markdown.Parse(text, pipeline);

foreach (var node in document.Descendants())
{
    Console.WriteLine(node.ToString());
}


Console.WriteLine(document.ToHtml(pipeline));

// Console.WriteLine(Markdown.ToHtml(text, pipeline));



// prints: <p>This is a text with some <em>emphasis</em></p>

// var nb1 = new LuaNotebook("My Notebook");

// nb1.lua.LoadCLRPackage();
// nb1.Execute("lua", @"
// local NotebookDatabase = import('NotebookDatabase', 'NotebookDatabase')

// for k, v in pairs(NotebookDatabase) do
//     io.write(k .. ' : ' .. tostring(v) .. '\n')
// end
// ");

// nb1.Execute("lua", @"
// response = AI(
//     'Give me JSON array with pairs question-answer, questions being 50 most common words for food ingredients in Czech and answers being English translations.',
//     'Follow example response and give only JSON. Example response [{""front"": ""sůl"", ""back"": ""salt""}]. JSON should be compressed, not indented.'
// )
// io.write('\nA\n')
// print(response)
// ");

// nb1.Execute("lua", @"
// io.write('\nB\n')
// io.write(response)
// io.write('\nC\n')
// ok, response_json = pcall(json.decode, response)
// io.write('\nD\n')
// if not ok then
//   io.write(response_json)
// else
//   io.write('Okay, doing the thing\n')
//   io.write(type(response_json) .. '\n')
//   save_deck('cz-en food deck', response_json)
// end
// ");
