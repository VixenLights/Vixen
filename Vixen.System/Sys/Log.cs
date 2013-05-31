using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys
{
    public abstract class Log
    {
        public event EventHandler<LogItemEventArgs> ItemLogged;

        protected Log(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public virtual void Write(string qualifyingMessage, Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}: {1}\n", qualifyingMessage, ex.Message);

            while (ex.InnerException != null)
            {
                sb.AppendLine(ex.InnerException.Message);
                ex = ex.InnerException;
            }
            Write(sb.ToString());
        }

        public virtual void Write(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(ex.Message);

            while (ex.InnerException != null)
            {
                sb.AppendLine(ex.InnerException.Message);
                ex = ex.InnerException;
            }
            Write(sb.ToString());

        }

        public virtual void Write(string text)
        {
            text = string.Format("[{0}]: {1}\n", DateTime.Now, text);
            OnItemLogged(new LogItemEventArgs(text));
        }

        protected virtual void OnItemLogged(LogItemEventArgs e)
        {
            if (ItemLogged != null)
            {
                ItemLogged(this, e);
            }
        }
    }
}
