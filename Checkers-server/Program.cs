using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Checkers_server;


TcpServer server = new TcpServer("127.0.0.1", 8888);
server.Start();
