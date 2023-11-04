using System.Linq;
using NLua;
using NotebookDatabase;

namespace NotebookLua.NativeLibs
{
    public static class Flashcards
    {
        public static void SaveDeck(string deckName, LuaTable flashcards)
        {
            using var db = new FlashcardContext();

            var deck = db.Decks.FirstOrDefault(d => d.Name == deckName);

            if (deck == null)
            {
                deck = new Deck { Name = deckName };
                db.Decks.Add(deck);
            }

            foreach (LuaTable flashcard in flashcards.Values)
            {
                var card = new Flashcard
                {
                    Deck = deck,
                    Front = flashcard["front"].ToString(),
                    Back = flashcard["back"].ToString(),
                };
                db.Add(card);
            }

            db.SaveChanges();
        }
    }
}
