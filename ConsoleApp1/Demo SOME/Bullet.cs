using Shard;
using System;
using System.Drawing;

namespace JumpAndRun
{
    class Bullet : NetworkedObject, CollisionHandler
    {

        private int direction = 1;
        private int updateCounter = 0;
        private int speed = 500;
        private string spriteName;
        public bool synced;


        public Bullet(bool synced)
        {
            Debug.Log("synced: " + synced);
            syncedInitialize();
            this.synced = true;

        }
        public Bullet()
        {
            Debug.Log("non synced");
            localInitialize();
            this.synced = false;
        }

        public override bool isSynced()
        {
            return synced;
        }

        public override void syncedInitialize()
        {
            setPhysicsEnabled();

            addTag("NetworkedBullet");
            MyBody.addRectCollider((int)Transform.X, (int)Transform.Y, 10, 10);
            MyBody.PassThrough = true;
        }

        public override void localInitialize()
        {
            setPhysicsEnabled();

            addTag("Bullet");
            MyBody.addRectCollider((int)Transform.X, (int)Transform.Y, 10, 10);

            MyBody.StopOnCollision = false;
            MyBody.Kinematic = false;
            this.Transform.SpritePath = "ManicMinerSprites/" + "bullet" + ".png";
        }




        public void setSpriteName(string name)
        {
            this.spriteName = name;
            this.Transform.SpritePath = "ManicMinerSprites/" + name + ".png";
        }

        public override string getFullSpriteName()
        {
            return spriteName;
        }

        public void setPosition(double x, double y)
        {
            Transform.translate(x, y);
        }

        
        

        public override void update()
        {
            if (!this.ToBeDestroyed && !this.synced)
            {
                if (Transform.X > 1200 || Transform.X < 0 || Transform.Y > 600 || Transform.Y < 0)
                {
                    this.ToBeDestroyed = true;
                    Debug.Log("set to be destroyd");
                }
                else
                {
                    Transform.translate(direction * speed * Bootstrap.getDeltaTime(), 0);
                    //sendPosition();
                    updateCounter++;
                }
            }
            Bootstrap.getDisplay().addToDraw(this);

        }

        public void onCollisionEnter(PhysicsBody x)
        {
            if (x.Parent.checkTag("Box"))
            {
                this.ToBeDestroyed = true;
            
           
        }
            if (x.Parent.checkTag("Player"))
            {

                this.RemoteDestroy = true;

                Debug.Log("collistion found with Networked player");
            }

        }

        public void onCollisionExit(PhysicsBody x)
        {
        }

        public void onCollisionStay(PhysicsBody x)
        {
        }

        public override string ToString()
        {
            return "City: [" + Transform.X + ", " + Transform.Y + "]";
        }

        internal void setDirection(int direction)
        {
            this.direction = direction;
        }
    }
}
