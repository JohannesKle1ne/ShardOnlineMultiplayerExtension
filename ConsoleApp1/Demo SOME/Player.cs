using Shard;
using System.Collections.Generic;
using SDL2;
using Newtonsoft.Json;
using System;

namespace JumpAndRun
{
    class Player : NetworkedObject, InputListener, PositionCollisionHandler
    {
        private string sprite;
        private bool left, right, jumpUp, jumpDown, fall, canJump, shoot;
        private int spriteCounter, spriteCounterDir;
        private string spriteName;
        private double spriteTimer, jumpCount;
        private double jumpMax = 0.3;

        private double jumpSpeed = 260;
        private double fallCounter;
        private double speed = 100;
        private bool movingStarted;
        public Bullet bullet;
        private int id;
        public string spriteColor = "red";
        public bool synced;
        public double reload;
        public double reloadTime;

        class Vector
        {

        }

        public Player(bool synced)
        {
            this.synced = true;
            syncedInitialize();
        }
        public Player()
        {
            this.synced = false;
            localInitialize();
        }



        public override bool isSynced()
        {
            return synced;
        }


        public override void localInitialize()
        {
            spriteName = "right";
            spriteCounter = 1;
            movingStarted = false;
            setPhysicsEnabled();
            MyBody.addRectCollider();
            MyBody.UsesGravity = true;
            MyBody.Mass = 0.4f;
            addTag("Player");
            spriteTimer = 0;
            jumpCount = 0;
            reloadTime = 2;
            reload = 0;
            Bootstrap.getInput().addListener(this);

            id = NetworkClient.GetInstance().id;


            Transform.translate(50, 330);
            MyBody.StopOnCollision = true;
            //MyBody.Kinematic = true;
            MyBody.PassThrough = false;

            spriteCounterDir = 1;
        }

        public override void syncedInitialize()
        {
            spriteName = "right";
            spriteCounter = 1;
            // setPhysicsEnabled();
            //MyBody.addRectCollider();
            ///addTag("NetworkedPlayer");
            spriteTimer = 0;
            jumpCount = 0;
            //MyBody.Mass = 1;
            //Bootstrap.getInput().addListener(this);


            Transform.translate(50, 480);
            //MyBody.StopOnCollision = false;
            //MyBody.Kinematic = false;


            spriteCounterDir = 1;
        }





        public void Move(double x, double y)
        {
            if (this.Transform != null)
            {
                this.Transform.Y = (float)y;
                this.Transform.X = (float)x;
            }

        }

        public int GetId()
        {
            return id;
        }


        public void handleInput(InputEvent inp, string eventType)
        {
            if (eventType == "KeyDown")
            {

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_D)
                {
                    right = true;
                    spriteName = "right";

                }

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_A)
                {
                    left = true;
                    spriteName = "left";
                }

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_W && canJump == true)
                {
                    jumpUp = true;
                    Debug.Log("Jumping up");

                }
                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_SPACE)
                {
                    if (reload <= 0)
                    {
                        shoot = true;
                        reload = reloadTime;
                        Debug.Log("Shoot with time: " + reload);
                    }

                }
            }

            else if (eventType == "KeyUp")
            {

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_D)
                {
                    right = false;

                }

                if (inp.Key == (int)SDL.SDL_Scancode.SDL_SCANCODE_A)
                {
                    left = false;
                }



            }

        }


        public override string getFullSpriteName()
        {
            return spriteColor + spriteName + spriteCounter;
        }


        private int getRandom(int max)
        {
            Random rand = new Random();
            return rand.Next(max);
        }



        public override void update()
        {

            //Debug.Log("Fallcounter is " + fallCounter);
            double oldX = Transform.X;
            double oldY = Transform.Y;

            if (left)
            {
                this.Transform.translate(-1 * speed * Bootstrap.getDeltaTime(), 0);
                spriteTimer += Bootstrap.getDeltaTime();
                //this.sendPosition();
            }

            if (right)
            {
                this.Transform.translate(1 * speed * Bootstrap.getDeltaTime(), 0);
                spriteTimer += Bootstrap.getDeltaTime();

                //this.sendPosition();
            }

            if (jumpUp)
            {

                //fall = false;
                //fallCounter = 0;
                if (jumpCount < jumpMax)
                {
                    this.Transform.translate(0, -1 * jumpSpeed * Bootstrap.getDeltaTime());
                    jumpCount += Bootstrap.getDeltaTime();
                }
                else
                {
                    jumpCount = 0;
                    jumpUp = false;
                   // fall = true;

                }
                //this.sendPosition();
            }

            if (shoot)
            {
                if (true)
                {
                    Bullet bullet = new Bullet();
                    bullet.setSpriteName(spriteColor + "bullet");
                    if (spriteName == "right")
                    {
                        bullet.setPosition(Transform.X + 40, Transform.Y);
                        bullet.setDirection(1);
                    }
                    else
                    {
                        bullet.setPosition(Transform.X - 10, Transform.Y);
                        bullet.setDirection(-1);
                    }
                }



                shoot = false;
            }



            if (spriteTimer > 0.1f)
            {
                spriteTimer -= 0.1f;
                spriteCounter += spriteCounterDir;

                if (spriteCounter >= 4)
                {
                    spriteCounterDir = -1;

                }

                if (spriteCounter <= 1)
                {
                    spriteCounterDir = 1;

                }


            }

            if (fall)
            {
                //Transform.translate(0, jumpSpeed * Bootstrap.getDeltaTime());
                //fallCounter += Bootstrap.getDeltaTime();

                //if (Transform.Y > 900)
                //{
                //    ToBeDestroyed = true;
                //}
                //this.sendPosition();

            }

            this.Transform.SpritePath = "ManicMinerSprites/" + getFullSpriteName() + ".png";

            //Debug.Log(this.Transform.SpritePath);
            if (reload > 0)
            {
                reload -= Bootstrap.getDeltaTime();
            }


            Bootstrap.getDisplay().addToDraw(this);

        }

        public bool isCenterX(PhysicsBody x)
        {
            
            float[] minAndMaxX = x.getMinAndMax(true);
            float[] minAndMaxY = x.getMinAndMax(false);

            if (Transform != null)
            {
                if (Transform.X + Transform.Wid >= minAndMaxX[0] && Transform.X <= minAndMaxX[1])
                {
                    // We're in the centre, so it's fine.
                    return true;

                    if (Transform.Y + Transform.Ht >= minAndMaxY[0])
                    {
                        return true;
                    }
                }
            }



            return false;
        }

        public void onCollisionEnter(PhysicsBody x)
        {
        }

        public void onCollisionExit(PhysicsBody x)
        {
        }

        public void onCollisionStay(PhysicsBody x)
        {
        }

        public void onCollisionEnter(PhysicsBody x, string direction)
        {

            if (MyBody == null || Transform == null) return;

            MyBody.UsesGravity = false;


            Debug.Log("direction: " + direction);
            if (x.Parent.checkTag("NetworkedBullet"))
            {
                ToBeDestroyed = true;
            }

            if (x.Parent.checkTag("Gun"))
            {
                reloadTime -= 0.5;

                Debug.Log("collistion found with gun");
            }

            if (x.Parent.checkTag("Boot"))
            {
                speed += 50;
                Debug.Log("collistion found with boot");
            }

            if (x.Parent.checkTag("Spring"))
            {
                jumpMax += 0.1;
            }



            //if (fallCounter > 2)
            //{
            //    ToBeDestroyed = true;
            //}

            //fallCounter = 0;

            //if (isCenterX(x))
            //{
            //    fall = true;
            //}
            //else
            //{
            //    fall = false;
            //}
            //canJump = true;
            if (direction == "Bottom")
            {
                canJump = true;
            }
            if (direction == "Top")
            {
                Transform.translate(0, 0.1);
            }
            if (direction == "Left")
            {
                Transform.translate(0.1, 0);
            }
            if (direction == "Right")
            {
                Transform.translate(-0.1, 0);
            }
        }

        public void onCollisionExit(PhysicsBody x, string direction)
        {
            if (MyBody == null || Transform == null) return;
            
                MyBody.UsesGravity = true;
            
            //if (x.Parent.checkTag("Collectible"))
            //{
            //    return;
            //}

            canJump = false;
           // fall = true;

            if (x.Parent.checkTag("MovingBox"))
            {
                //Debug.Log("MovingBox collide");
                //sendPosition();
            }
        }

        public void onCollisionStay(PhysicsBody x, string direction)
        {
            if (MyBody == null || Transform == null) return;

            MyBody.UsesGravity = false;


            if (x.Parent.checkTag("MovingBox"))
            {
                //Debug.Log("MovingBox collide");
                //sendPosition();aw
            }

            if (x.Parent.checkTag("Collectible"))
            {
                return;
            }

            if (direction=="Bottom")
            {
                canJump = true;
            }
            if (direction == "Top")
            {
                Transform.translate(0, 0.1);
            }
            if (direction == "Left")
            {
                Transform.translate(0.1, 0);
                Transform.translate(0, 0.1);
            }
            if (direction == "Right")
            {
                Transform.translate(-0.1, 0);
                Transform.translate(0, 0.1);
            }
            //else
            //{
            //    //canJump = true;
            //    Transform.translate(0, 1);
            //    if (isRight)
            //    {

            //    }
            //    else
            //    {
            //        Transform.translate(1, 0);
            //    }
            //}


            //if (isCenterX(x))
            //{
            //    fall = false;
            //    canJump = true;
            //    fallCounter = 0;
            //}
            //else
            //{
            //    fall = true;
            //}
        }
    }
}
