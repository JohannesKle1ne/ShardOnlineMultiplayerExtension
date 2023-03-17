using Shard;
using System;
using System.Drawing;
using System.Xml.Linq;

namespace JumpAndRun
{
    class Gun : Reward
    {

        public Gun(bool synced)
        {
            Debug.Log("synced: " + synced);
            syncedInitialize();
            this.synced = true;

        }
        public Gun()
        {
            Debug.Log("non synced");
            localInitialize();
            this.synced = false;
        }
        public override void localInitialize()
        {
            base.localInitialize();
            addTag("Gun");
            setSpriteName("gun");
        }

        public override void syncedInitialize()
        {
            base.syncedInitialize();
            addTag("Gun");
            setSpriteName("gun");
        }
    }
}
