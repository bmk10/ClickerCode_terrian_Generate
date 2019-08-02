using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clicker
{
    public class ActionEntry
    {
        int x;
        int y;
        string text;
        int interval;
        ClickType type;
        public ActionEntry(int x, int y, string text, int interval, ClickType type)
        {
            this.x = x;
            this.y = y;
            this.text = text;
            this.interval = interval;
            this.type = type;
        }

        public int X
        {
            set { x = value; }
            get { return x; }
        }
        public int Y
        {
            set { y = value; }
            get { return y; }
        }
        public string Text
        {
            set { text = value; }
            get { return text; }
        }

        public int Interval
        {
            set { interval = value; }
            get { return interval; }
        }
        public ClickType Type
        {
            set { type = value; }
            get { return type; }
        }
    }
}
