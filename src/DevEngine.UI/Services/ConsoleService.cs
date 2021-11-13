using DevEngine.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DevEngine.UI.Services
{
    public class ConsoleService : IConsoleLogger
    {

        private readonly ConcurrentQueue<string> Lines = new ConcurrentQueue<string>();
        private TextWriter? PreviousOut;

        public delegate void OnConsoleLinesChangedHandler();
        public event OnConsoleLinesChangedHandler? OnConsoleLinesChanged;

        private void Add(string content)
        {
            Lines.Enqueue(content);

            if (Lines.Count > 1000)
                Lines.TryDequeue(out var _);

            OnConsoleLinesChanged?.Invoke();
        }

        public IEnumerable<string> GetLines() => Lines;

        public void Clear()
        {
            Lines.Clear();
        }

        public void StartLogging()
        {
            PreviousOut = Console.Out;
            var newWriter = new ConsoleWriter(line =>
            {
                Add(line);
                OnConsoleLinesChanged?.Invoke();
            });
            Console.SetOut(newWriter);
        }

        public void StopLogging()
        {
            if (PreviousOut == null)
                return;

            var o = Console.Out;
            Console.SetOut(PreviousOut);
            o.Dispose();

            PreviousOut = null;
        }

        private class ConsoleWriterEventArgs : EventArgs
        {
            public string Value { get; private set; }
            public ConsoleWriterEventArgs(string value)
            {
                Value = value;
            }
        }

        private class ConsoleWriter : TextWriter
        {
            public override Encoding Encoding { get { return Encoding.UTF8; } }

            private Action<string> OnWriteLine;

            public ConsoleWriter(Action<string> onWriteLine)
            {
                OnWriteLine = onWriteLine;
            }

            public override void Write(string? value)
            {
                if (value == null)
                {
                    base.Write(value);
                    return;
                }

                OnWriteLine(value);

                base.Write(value);
            }

            public override void WriteLine(string? value)
            {
                if (value == null)
                {
                    base.Write(value);
                    return;
                }

                OnWriteLine(value);

                base.WriteLine(value);
            }
        }
    }
}
