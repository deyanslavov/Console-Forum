using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Forum.App.ViewModels
{
    class ContentViewModel
    {
        private const int LINE_LENGTH = 37;

        public ContentViewModel(string content)
        {
            this.Content = this.GetLines(content);
        }

        public string[] Content { get; }

        protected string[] GetLines(string content)
        {
            char[] contentChars = content.ToCharArray();

            ICollection<string> lines = new List<string>();

            for (int i = 0; i < content.Length; i+= LINE_LENGTH)
            {
                char[] row = contentChars.Skip(1).Take(LINE_LENGTH).ToArray();
                string rowString = string.Join("", row);
                lines.Add(rowString);
            }
            return lines.ToArray();
        }
    }
}
