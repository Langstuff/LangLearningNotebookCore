using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NotebookDatabase;

// Note: heavy AI usage here

// Define the Flashcard class
public class Flashcard
{
    public int Id { get; set; }
    public string? Front { get; set; }
    public string? Back { get; set; }
    public SM2Flashcard SM2Flashcard { get; set; }
    public int DeckId { get; set; }
    public Deck Deck { get; set; }
}

// Define the SM2Flashcard class
public class SM2Flashcard
{
    [Key]
    public int FlashcardId { get; set; }
    public int Interval { get; set; }
    public int Repetitions { get; set; }
    public double EFactor { get; set; }
    public Flashcard Flashcard { get; set; }
}

// Define the Deck class
public class Deck
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Flashcard> Flashcards { get; set; }
}

// Create the DbContext class to represent the database
public class FlashcardContext : DbContext
{
    public DbSet<Flashcard> Flashcards { get; set; }
    public DbSet<SM2Flashcard> SM2Flashcards { get; set; }
    public DbSet<Deck> Decks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=notebook.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Flashcard>()
            .HasOne(f => f.SM2Flashcard)
            .WithOne(sf => sf.Flashcard)
            .HasForeignKey<SM2Flashcard>(sf => sf.FlashcardId);

        modelBuilder.Entity<Flashcard>()
            .HasOne(f => f.Deck)
            .WithMany(d => d.Flashcards)
            .HasForeignKey(f => f.DeckId);
    }
}
