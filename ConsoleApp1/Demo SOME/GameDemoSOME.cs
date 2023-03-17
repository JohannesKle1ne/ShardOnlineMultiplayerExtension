// Sprites from https://www.spriters-resource.com/fullview/113060/

using JumpAndRun;
using System;
using System.Collections.Generic;
using System.Drawing;
using static System.Formats.Asn1.AsnWriter;
using System.Xml.Linq;
using System.Numerics;

namespace Shard
{
    class GameDemoSOME : NetworkedGame, InputListener
    {
        Random rand;
        public Player myPlayer;
        private double respawnTime = 0;
        private MovingBox movingBox1;

        private MovingBox movingBox2;
        private MovingBox movingBox3;
        private MovingBox movingBox4;
        private MovingBox movingBox5;
        private MovingBox movingBox6;

        private MovingBox movingBox7;


        public override bool isRunning()
        {

            //if (myPlayer != null && myPlayer.ToBeDestroyed)
            //{
            //    return false;
            //}

            return true;
        }



        public override void update()
        {
            //Debug.Log(Math.Round(Bootstrap.getDeltaTime()*1000).ToString());
            if (myPlayer != null && myPlayer.ToBeDestroyed && respawnTime <= 0)
            {
                respawnTime = 5;
                if (myPlayer.Transform!=null)
                {
                    Random rd = new Random();
                    int type  = rd.Next(0,3);
                    Reward r;
                    if (type == 0)
                    {
                        r = new Boot();
                        r.Transform.X = myPlayer.Transform.X;
                        r.Transform.Y = myPlayer.Transform.Y;
                    }
                    if (type == 1)
                    {
                        r = new Gun();
                        r.Transform.X = myPlayer.Transform.X;
                        r.Transform.Y = myPlayer.Transform.Y;
                    }
                    if (type == 2)
                    {
                        r = new Spring();
                        r.Transform.X = myPlayer.Transform.X;
                        r.Transform.Y = myPlayer.Transform.Y;
                    }

                }
                
            }


            if (respawnTime > 0)
            {
                //Debug.Log(respawnTime.ToString());
                Color col = Color.White;
                Bootstrap.getDisplay().showText("You died!", 30, 30, 40, col);
                Bootstrap.getDisplay().showText("Respawn in: " + (((int)respawnTime) + 1), 30, 80, 20, col);




                respawnTime = respawnTime - Bootstrap.getDeltaTime();
                //Debug.Log(respawnTime.ToString());

                if (respawnTime <= 0)
                {
                    NetworkClient client = NetworkClient.GetInstance();
                    (int x, int y) sPos = client.GetRandomStartPosition();
                    string oldColor = myPlayer.spriteColor;
                    Debug.Log("resetting the player");
                    setPlayerStart(sPos.x, sPos.y);
                    setPlayerColor(oldColor);
                    //string message = new Position(client.id, MessageType.Position, myPlayer.ToString(), myPlayer.id, sPos.x, sPos.y, myPlayer.getFullSpriteName()).ToJson();
                    //client.Send(message);
                }

            }


        }




        public void setBoxPosition(int position, int index)
        {
            
            if (index == 0)
            {
                if (movingBox1 == null)
                {
                    movingBox1 = new MovingBox();
                    movingBox1.setPosition(0, 600, 300, 100);
                    movingBox1.MoveDirY = 1;
                }
                    movingBox1.Transform.Y = position;
            }
            if (index == 1)
            {
                if (movingBox2 == null)
                {
                   

                    movingBox2 = new MovingBox();
                    movingBox2.setPosition(500, 550, 500, 100);
                    movingBox2.MoveDirY = 1;

                    movingBox3 = new MovingBox();
                    movingBox3.setPosition(550, 525, 500, 100);
                    movingBox3.MoveDirY = 1;

                    movingBox4 = new MovingBox();
                    movingBox4.setPosition(550, 575, 500, 100);
                    movingBox4.MoveDirY = 1;

                    movingBox5 = new MovingBox();
                    movingBox5.setPosition(600, 550, 500, 100);
                    movingBox5.MoveDirY = 1;

                    movingBox6 = new MovingBox();
                    movingBox6.setPosition(650, 575, 500, 100);
                    movingBox6.MoveDirY = 1;


                   
                }
                movingBox2.Transform.Y = position;
                movingBox3.Transform.Y = position-25;
                movingBox4.Transform.Y = position+25;
                movingBox5.Transform.Y = position;
                movingBox6.Transform.Y = position+25;
            }
            if (index == 2)
            {
                if (movingBox7 == null)
                {
                    movingBox7 = new MovingBox();
                    movingBox7.setPosition(1150, 425, 300, 100);
                    movingBox7.MoveDirY = 1;
                }
                movingBox7.Transform.Y = position;
            }

        }

        public void setPlayerStart(double x, double y)
        {
            myPlayer = new Player();
            myPlayer.Move(x, y);
        }

        public void setPlayerColor(string color)
        {
            myPlayer.spriteColor = color;
        }

       

        public override void initialize()
        {
            Box b;
            MovingBox mb;
            Bootstrap.getInput().addListener(this);
            Bootstrap.getDisplay().setSize(1200, 600);
            Bootstrap.getDisplay().setBackgroundColor(Color.LightSkyBlue);
            rand = new Random();




            //movingBox1 = new MovingBox();
            //movingBox1.setPosition(0, 600, 300, 100);
            //movingBox1.MoveDirY = 1;

            b = new Box();
            b.setPosition(0, 360, 0, 0);

            b = new Box();
            b.setPosition(50, 350, 0, 0);

            b = new Box();
            b.setPosition(100, 375, 0, 0);

            b = new Box();
            b.setPosition(150, 450, 0, 0);

            b = new Box();
            b.setPosition(100, 560, 0, 0);

            b = new Box();
            b.setPosition(150, 590, 0, 0);

            b = new Box();
            b.setPosition(200, 575, 0, 0);

            b = new Box();
            b.setPosition(250, 560, 0, 0);



            b = new Box();
            b.setPosition(350, 590, 0, 0);

            b = new Box();
            b.setPosition(400, 575, 0, 0);

            b = new Box();
            b.setPosition(450, 525, 0, 0);


            //movingBox2 = new MovingBox();
            //movingBox2.setPosition(500, 550, 500, 100);
            //movingBox2.MoveDirY = 1;

            //movingBox3 = new MovingBox();
            //movingBox3.setPosition(550, 525, 500, 100);
            //movingBox3.MoveDirY = 1;

            //movingBox4 = new MovingBox();
            //movingBox4.setPosition(550, 575, 500, 100);
            //movingBox4.MoveDirY = 1;

            //movingBox5 = new MovingBox();
            //movingBox5.setPosition(600, 550, 500, 100);
            //movingBox5.MoveDirY = 1;

            //movingBox6 = new MovingBox();
            //movingBox6.setPosition(650, 575, 500, 100);
            //movingBox6.MoveDirY = 1;

            b = new Box();
            b.setPosition(700, 525, 0, 0);

            b = new Box();
            b.setPosition(750, 500, 0, 0);

            b = new Box();
            b.setPosition(800, 475, 0, 0);

            b = new Box();
            b.setPosition(900, 425, 0, 0);

            b = new Box();
            b.setPosition(950, 400, 0, 0);

            b = new Box();
            b.setPosition(1000, 375, 0, 0);

            b = new Box();
            b.setPosition(1050, 400, 0, 0);

            b = new Box();
            b.setPosition(1100, 425, 0, 0);


            //movingBox7 = new MovingBox();
            //movingBox7.setPosition(1150, 425, 300, 100);
            //movingBox7.MoveDirY = 1;


            b = new Box();
            b.setPosition(770, 90, 0, 0);

            b = new Box();
            b.setPosition(750, 140, 0, 0);

            b = new Box();
            b.setPosition(750, 190, 0, 0);

            b = new Box();
            b.setPosition(750, 240, 0, 0);

            b = new Box();
            b.setPosition(800, 260, 0, 0);

            b = new Box();
            b.setPosition(850, 240, 0, 0);

            b = new Box();
            b.setPosition(900, 220, 0, 0);

            b = new Box();
            b.setPosition(950, 240, 0, 0);

            b = new Box();
            b.setPosition(1000, 220, 0, 0);

            b = new Box();
            b.setPosition(1050, 240, 0, 0);

            b = new Box();
            b.setPosition(0, 100, 0, 0);

            b = new Box();
            b.setPosition(50, 120, 0, 0);

            b = new Box();
            b.setPosition(100, 100, 0, 0);

            b = new Box();
            b.setPosition(150, 120, 0, 0);

            b = new Box();
            b.setPosition(200, 100, 0, 0);

            b = new Box();
            b.setPosition(300, 150, 0, 0);

            b = new Box();
            b.setPosition(350, 175, 0, 0);

            b = new Box();
            b.setPosition(400, 200, 0, 0);




        }


        public void handleInput(InputEvent inp, string eventType)
        {
        }

       


    }
}
