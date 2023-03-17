using Shard;
using System;
using System.Drawing;
using System.Xml.Linq;

namespace JumpAndRun
{
    abstract class Reward : NetworkedObject, CollisionHandler
    {

        private int direction = 1;
        private int updateCounter = 0;
        private int speed = 500;
        private string spriteName;
        public bool synced;


        

        

        public override bool isSynced()
        {
            return synced;
        }

        public override void syncedInitialize()
        {
            setPhysicsEnabled();

            MyBody.addRectCollider((int)Transform.X, (int)Transform.Y, 10, 10);
            MyBody.PassThrough = true;
            
            
        }

        public override void localInitialize()
        {
            setPhysicsEnabled();

            MyBody.addRectCollider((int)Transform.X, (int)Transform.Y, 10, 10);

            MyBody.StopOnCollision = false;
            MyBody.Kinematic = false;
            
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
            
            Bootstrap.getDisplay().addToDraw(this);

        }

        public void onCollisionEnter(PhysicsBody x)
        {
           
            if (x.Parent.checkTag("Player"))
            {
                if (this.synced)
                {
                    this.RemoteDestroy = true;
                }
                else
                {
                    this.ToBeDestroyed = true;
                }


                Debug.Log("collistion found with player");
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
