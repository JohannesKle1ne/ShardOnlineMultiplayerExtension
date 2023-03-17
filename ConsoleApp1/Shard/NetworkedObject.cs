/*
*
*   Any game object that is going to react to collisions will need to implement this interface.
*   @author Michael Heron
*   @version 1.0
*   
*/

using System;

namespace Shard
{
    abstract class NetworkedObject : GameObject
    {

        public int id;

        public bool RemoteDestroy;

        public abstract string getFullSpriteName();

        public abstract bool isSynced();
        public abstract void syncedInitialize();

        public abstract void localInitialize();

        public void setId(int id)
        {
            this.id = id;
        }


        public NetworkedObject()
        {
            Random rd = new Random();
            id = rd.Next(0, 10000);

            RemoteDestroy = false;

        }
    }
}
