namespace Core.Notes
{

    [System.Serializable]
    public struct NoteLineFormat
    {
        public SizeType size;
        public ListType list;
        public int lineNumber;
        public bool bold;
        public bool italic;
        public bool underline;
        public int indentSize;
        public int color;
        public bool mark;


        public const float LIST_INDENT = 75f;
        public const char BULLET_POINT = '•';

        public enum SizeType
        {
            N, H1, H2, H3
        }

        public enum ListType
        {
            None, Numbered, Dotted
        }
    }
}