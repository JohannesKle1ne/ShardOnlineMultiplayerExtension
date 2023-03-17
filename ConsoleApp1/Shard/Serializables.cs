using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shard
{
    class Position
    {
        public int clientId;
        public double x;
        public double y;
        public string sprite;
        public MessageType type;
        public string objectType;
        public int objectId;
        public Position(int clientId, MessageType type, string objectType, int objectId, double x, double y, string sprite)
        {
            this.clientId = clientId;
            this.x = x;
            this.y = y;
            this.type = type;
            this.sprite = sprite;
            this.objectType = objectType;
            this.objectId = objectId;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    class Destroy
    {
        public int clientId;
        public MessageType type;
        public int objectId;
        public Destroy(int clientId, MessageType type, int objectId)
        {
            this.clientId = clientId;
            this.type = type;
            this.objectId = objectId;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    class DestroyRequest
    {
        public int clientId;
        public MessageType type;
        public int targetObjectId;
        public DestroyRequest(int clientId, int targetObjectId)
        {
            this.clientId = clientId;
            this.type = MessageType.DestroyRequest;
            this.targetObjectId = targetObjectId;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    class Action
    {
        public int clientId;
        public MessageType type;
        public string color;
        public int bulletId;
        public int position;
        public int index;
        public Action(int id, MessageType type)
        {
            clientId = id;
            this.type = type;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }




    enum MessageType
    {
        Unknown,
        PlayerPosition,
        PlayerStartPosition,
        BulletPosition,
        PlayerDestroy,
        BulletDestroy,
        BulletCollision,
        Color,
        BoxPosition,
        Position,
        Destroy,
        DestroyRequest

    }
}
