using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;

namespace WebSocketServer.Net.Common
{
    internal struct DataBuffer : IDataBuffer
    {
        internal DataBuffer(WebSocketController _controller, WebSocketMessageType type, Stream _stream)
        {
            Type = type;
            controller = _controller;
            stream = _stream;
        }
        public WebSocketMessageType Type { get; private set; }

        public Byte[] Binary
        {
            get
            {
                Byte[] data = new Byte[stream.Length];
                stream.Position = 0;
                stream.Read(data,0, (Int32)stream.Length);
                return data;
            }
        }

        public String Text
        {
            get
            {
                return Encoding.GetString(this.Binary);
            }
        }


        public Int32 Count
        {
            get
            {
                return (Int32)stream.Length;
            }
        }


        public Encoding Encoding
        {
            get
            {
                return controller.Encoding;
            }
        }

        private Stream stream { get; set; }


        private WebSocketController controller { get; set; }

        public void Dispose()
        {
            if (stream != null)
            {
                stream = null;
                controller = null;
            }

        }
    }
}
