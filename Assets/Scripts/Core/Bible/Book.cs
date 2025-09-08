using System.Collections.Generic;

namespace Core
{
    public class Book
    {
        public string Name;
        public string[] names;

        public int book;
        private List<Chapter> chapters;

        public int Count
        {
            get { return chapters.Count; }
        }

        public Chapter this[int chapter]
        {
            get
            {
                return chapters[chapter - 1];
            }
        }

        public Book(int book, string name)
        {
            this.book = book;
            this.Name = name;
            this.chapters = new List<Chapter>();
        }


        /// <summary>
        /// Add chapter to book
        /// </summary>
        /// <param name="chapter"></param>
        /// <returns>0=success, 1=repeat, 2=next</returns>
        public int Push(Chapter chapter)
        {
            if(chapter.book != book)
            {
                // pushing wrong book's chapter
                return -2;
            }

            if(chapter.chapter <= Count)
            {
                // pushing pre-existing chapters
                return -1;
            }

            chapters.Add(chapter);
            return 0;
        }

        /// <summary>
        /// Names And Abbreviations
        /// </summary>
        public static (string FullName, string Abbrev)[] ABBREVIATIONS = new (string, string)[]
        {
            // Old Testament
            ("Genesis", "Gen"), ("Exodus", "Exo"), ("Leviticus", "Lev"), ("Numbers", "Num"), ("Deuteronomy", "Deu"),
            ("Joshua", "Jos"), ("Judges", "Jdg"), ("Ruth", "Rut"), ("1 Samuel", "1Sa"), ("2 Samuel", "2Sa"), ("1 Kings", "1Ki"),

            ("2 Kings", "2Ki"), ("1 Chronicles", "1Ch"), ("2 Chronicles", "2Ch"), ("Ezra", "Ezr"), ("Nehemiah", "Neh"),
            ("Esther", "Est"), ("Job", "Job"), ("Psalms", "Psa"), ("Proverbs", "Pro"), ("Ecclesiastes", "Ecc"), ("Song of Solomon", "Son"),

            ("Isaiah", "Isa"), ("Jeremiah", "Jer"), ("Lamentations", "Lam"), ("Ezekiel", "Eze"), ("Daniel", "Dan"),
            ("Hosea", "Hos"), ("Joel", "Joe"), ("Amos", "Amo"), ("Obadiah", "Oba"), ("Jonah", "Jon"), ("Micah", "Mic"),

            ("Nahum", "Nah"), ("Habakkuk", "Hab"), ("Zephaniah", "Zep"), ("Haggai", "Hag"), ("Zechariah", "Zec"),
            ("Malachi", "Mal"),

            // New Testament
            ("Matthew", "Mat"), ("Mark", "Mar"), ("Luke", "Luk"), ("John", "Joh"), ("Acts", "Act"),
            ("Romans", "Rom"), ("1 Corinthians", "1Co"), ("2 Corinthians", "2Co"), ("Galatians", "Gal"), ("Ephesians", "Eph"), ("Philippians", "Phi"),

            ("Colossians", "Col"), ("1 Thessalonians", "1Th"), ("2 Thessalonians", "2Th"), ("1 Timothy", "1Ti"), ("2 Timothy", "2Ti"),
            ("Titus", "Tit"), ("Philemon", "Phm"), ("Hebrews", "Heb"), ("James", "Jam"), ("1 Peter", "1Pe"), ("2 Peter", "2Pe"),

            ("1 John", "1Jo"), ("2 John", "2Jo"), ("3 John", "3Jo"), ("Jude", "Jud"), ("Revelation", "Rev")
        };

    }
}