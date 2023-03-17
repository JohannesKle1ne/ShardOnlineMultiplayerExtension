using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebSocketSharp;
using Newtonsoft.Json;
using JumpAndRun;

namespace Shard
{
    class MyVector
    {
        internal double x, y;
    }


    internal sealed class 
        
        NetworkClient
    {
        private NetworkClient() { }

        private static NetworkClient _instance;
        private WebSocket ws;
        private GameDemoSOME game;
        private NetworkManager manager;
        private bool isSet = false;
        internal int id;
        private readonly (int x, int y)[] startPositions = new[] {
        (50, 330),
        (50, 330),
        (50, 330),
        (50, 330),
        (50, 330),
        (50, 330),
         //(60, 330),
         // (70, 330),
         //  (80, 330),
         //   (90, 330),
         //   (100, 330),

    };

        internal static NetworkClient GetInstance()
        {
            if (_instance == null)
            {
                _instance = new NetworkClient();
            }
            return _instance;
        }

        internal void Start()
        {
            // Create a scoped instance of a WS client that will be properly disposed
            //using (WebSocket ws = new WebSocket("ws://simple-websocket-server-echo.glitch.me/"))
            //ws = new WebSocket("ws://secret-island-78427.herokuapp.com");
            ws = new WebSocket("ws://localhost:3000");

            Random rd = new Random();
            id =  rd.Next(1000, 9999);

            ws.OnMessage += Ws_OnMessage;

            ws.OnError += Ws_OnError;

            ws.Connect();

            //ws.Send($"{234},{234},{23432}");
            ws.Send($"{id}");
            Debug.Log("Connected");
            Console.WriteLine("Connected 2");
            //Console.ReadKey();

        }

        internal void Send(String message)
        {
            Debug.Log(message);
            ws.Send(message);
        }

        internal void setManager(NetworkManager m, GameDemoSOME g)
        {
            game = g;
            manager = m;
            isSet = true;
        }


        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {


            MessageType type = getMessageType(e.Data);
            //Debug.Log(type.ToString());
            if (type == MessageType.Color)
            {
               
                Action a = JsonConvert.DeserializeObject<Action>(e.Data);
                if (isSet)
                {
                    (int x, int y) sPos = GetRandomStartPosition();
                    game.setPlayerStart(sPos.x, sPos.y);
                    game.setPlayerColor(a.color);
                    string message = new Position(id, MessageType.Position,game.myPlayer.ToString(), game.myPlayer.id, sPos.x, sPos.y,game.myPlayer.getFullSpriteName() ).ToJson();
                    Send(message);
                }
            }
           
            if (type == MessageType.BulletCollision)
            {
                Action action = JsonConvert.DeserializeObject<Action>(e.Data);
                if (isSet)
                {
                    if (action.bulletId == id)
                    {
                        game.myPlayer.bullet.ToBeDestroyed= true;
                    }
                    else
                    {
                        //game.removeNetworkedBullet(action.bulletId);
                    }
                    

                }
            }
            if (type == MessageType.BoxPosition)
            {
                Action action = JsonConvert.DeserializeObject<Action>(e.Data);
                if (isSet)
                {
                    game.setBoxPosition(action.position, action.index);
                }
            }
            if (type == MessageType.Position)
            {
                Position mPos = JsonConvert.DeserializeObject<Position>(e.Data);
                if (isSet)
                {
                    manager.handleObjectPosition(mPos);
                }
            }
            if (type == MessageType.Destroy)
            {
                Destroy mPos = JsonConvert.DeserializeObject<Destroy>(e.Data);
                if (isSet)
                {
                    manager.handleObjectDestroy(mPos);
                }
            }
            if (type == MessageType.DestroyRequest)
            {
                DestroyRequest mPos = JsonConvert.DeserializeObject<DestroyRequest>(e.Data);
                if (isSet)
                {
                    manager.handleObjectDestroyRequest(mPos);
                }
            }

        }




        public (int x, int y) GetRandomStartPosition()
        {
            Random rnd = new Random();
            return startPositions[rnd.Next(0, 6)];
        }


        private MessageType getMessageType(String messageString)
        {
            try
            {
                Position mPos  = JsonConvert.DeserializeObject<Position>(messageString);
                if (mPos != null)
                {
                    return mPos.type;
                }
            }
            catch(Exception e1)
            {
                try
                {
                    Action action = JsonConvert.DeserializeObject<Action>(messageString);
                    if (action != null)
                    {
                        return action.type;
                    }
                }
                catch (Exception e2)
                {
                    return MessageType.Unknown;
                }
            }
            return MessageType.Unknown;
        }

        private void Ws_OnError(object sender, ErrorEventArgs e)
        {
            Debug.Log("Received from the server: " + e.Message);
        }

        private void DrawDot(double xpos, double ypos, int width, int height, int borderWidth)
        {
            // Convert from normalized coordinates to "pixel" coordinates
            int x = (int)Math.Round(xpos * width);
            int y = (int)Math.Round(ypos * height);

            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    if (i == x && j == y)
                    {
                        Console.Write("O");
                    }
                    else if (j < borderWidth || j > height - 1 - borderWidth
                        || i < borderWidth || i > width - 1 - borderWidth)
                    {
                        Console.Write('#');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}
