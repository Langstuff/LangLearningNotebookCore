using Microsoft.EntityFrameworkCore;

// Note: Generated with Copilot

namespace NotebookDatabase;

public class Article
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
    public ICollection<ArticleTagPair> ArticleTagPairs { get; set; }
}

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<ArticleTagPair> ArticleTagPairs { get; set; }
}

public class ArticleTagPair
{
    public int ArticleId { get; set; }
    public Article Article { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}
public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Article> Articles { get; set; }
}

public class ArticleContext : DbContext
{
    public DbSet<Article> Articles { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ArticleTagPair> ArticleTagPairs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArticleTagPair>()
            .HasKey(t => new { t.ArticleId, t.TagId });

        modelBuilder.Entity<ArticleTagPair>()
            .HasOne(pt => pt.Article)
            .WithMany(p => p.ArticleTagPairs)
            .HasForeignKey(pt => pt.ArticleId);

        modelBuilder.Entity<ArticleTagPair>()
            .HasOne(pt => pt.Tag)
            .WithMany(t => t.ArticleTagPairs)
            .HasForeignKey(pt => pt.TagId);
    }

    public void AddTagToArticle(Article article, string tagName)
    {
        var tag = Tags.SingleOrDefault(t => t.Name == tagName);
        if (tag == null)
        {
            tag = new Tag { Name = tagName };
            Tags.Add(tag);
        }
        article.ArticleTagPairs.Add(new ArticleTagPair { Article = article, Tag = tag });
    }

    public List<Article> GetArticlesWithTag(string tagName)
    {
        return Articles
            .Include(a => a.ArticleTagPairs)
            .ThenInclude(at => at.Tag)
            .Where(a => a.ArticleTagPairs.Any(at => at.Tag.Name == tagName))
            .ToList();
    }

    public void RemoveTagFromArticle(Article article, string tagName)
    {
        var articleTagPair = article.ArticleTagPairs
            .SingleOrDefault(at => at.Tag.Name == tagName);
        if (articleTagPair != null)
        {
            article.ArticleTagPairs.Remove(articleTagPair);
            ArticleTagPairs.Remove(articleTagPair);
        }
    }

    public List<string> GetTagsForArticle(Article article)
    {
        return article.ArticleTagPairs
            .Select(at => at.Tag.Name)
            .ToList();
    }
}
