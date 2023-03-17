using Shard;
using System;
using System.Drawing;
using System.Xml.Linq;

namespace JumpAndRun
{
    class Spring : Reward
    {

        public Spring(bool synced)
        {
            Debug.Log("synced: " + synced);
            syncedInitialize();
            this.synced = true;

        }
        public Spring()
        {
            Debug.Log("non synced");
            localInitialize();
            this.synced = false;
        }
        public override void localInitialize()
        {
            base.localInitialize();
            addTag("Spring");
            setSpriteName("spring");
        }

        public override void syncedInitialize()
        {
            base.syncedInitialize();
            addTag("Spring");
            setSpriteName("spring");
        }
    }
}
