using Shard;
using System;
using System.Drawing;
using System.Xml.Linq;

namespace JumpAndRun
{
    class Boot : Reward
    {

        public Boot(bool synced)
        {
            Debug.Log("synced: " + synced);
            syncedInitialize();
            this.synced = true;

        }
        public Boot()
        {
            Debug.Log("non synced");
            localInitialize();
            this.synced = false;
        }
        public override void localInitialize()
        {
            base.localInitialize();
            addTag("Boot");
            setSpriteName("boot");
        }

        public override void syncedInitialize()
        {
            base.syncedInitialize();
            addTag("Boot");
            setSpriteName("boot");
        }
    }
}
