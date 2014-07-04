using System;
using System.IO;
using System.Text;

namespace Codesseum.Common.Log
{
    public class Logger
    {
        public Logger(Stream output)
        {
            _output = output;
        }

        public void Log(string message)
        {
            if (_output == null) { return; }

            var bytes = Encoding.UTF8.GetBytes(message + Environment.NewLine);
            _output.Write(bytes, 0, bytes.Length);
            _output.Flush();
        }

        public void Close()
        {
            if (_output == null)
            {
                return;
            }

            _output.Flush();
            _output.Close();
        }

        private readonly Stream _output;
    }
}
