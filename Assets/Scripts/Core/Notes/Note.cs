using System;
using System.Collections.Generic;

namespace Core.Notes
{
    [System.Serializable]
    public class Note
    {
        public int ID;
        public string group;
        public string dateCreated;
        public string dateOpened;


        public string content
        {
            get
            {
                string str = "";
                foreach(var l in lines)
                {
                    str += l.text + '\n';
                }
                return str;
            }

            set
            {
                string[] lines = value.Split('\n');
                this.lines ??= new List<InNoteLine>();
                this.lines.Clear();

                for (int i = 0; i < lines.Length; i++)
                {
                    var line = new InNoteLine()
                    {
                        text = lines[i]
                    };

                    this.lines.Add(line);
                }
            }
        }

        public List<InNoteLine> lines;
        public DateTime DateCreated => DateTime.Parse(dateCreated);
        public DateTime DateOpened => DateTime.Parse(dateOpened);

    }
}