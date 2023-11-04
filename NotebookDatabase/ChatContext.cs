using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NotebookDatabase;

// Note: heavy AI usage here

public class NotebookContext : DbContext
{
    public DbSet<Notebook> Notebooks { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=notebook.db");
    }
}

public class Notebook
{
    public int NotebookId { get; set; }
    public string NotebookName { get; set; }
    public List<Message> Messages { get; set; }
}

public class Message
{
    public int MessageId { get; set; }
    public string? UserInput { get; set; }
    public string? ExecutionResult { get; set; }
    public string? LanguageMode { get; set; }
    public DateTime Timestamp { get; set; }

    public int NotebookId { get; set; }
    public Notebook Notebook { get; set; }
}
