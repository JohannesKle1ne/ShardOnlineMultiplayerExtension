/*
*
*   The Shard Physics Manager.   
*   
*   As with the PhysicsBody class, upon which this class depends, I make no claims as to the 
*       accuracy of the physics.  My interest in this course is showing you how an engine is 
*       architected.  It's not a course on game physics.  The task of making this work in 
*       a way that simulates real world physics is well beyond the scope of the course. 
*       
*   This class is responsible for a lot.  It handles the broad phase collision 
*       detection (via Sweep and Prune).  It handles the narrow phase collisions, making use of the 
*       collider objects and the Minkowski differences they generate.  It does some collision resolutions 
*       that are linked to the mass of colliding bodies.  And it has the management routines that 
*       let all that happen.
*       
*   @author Michael Heron
*   @version 1.0
*   
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Shard
{


    class CollidingObject
    {
        PhysicsBody a, b;

        internal PhysicsBody A { get => a; set => a = value; }
        internal PhysicsBody B { get => b; set => b = value; }

        public override String ToString()
        {
            return "[" + A.Parent.ToString() + " v " + B.Parent.ToString() + "]";
        }
    }

    class SAPEntry
    {
        PhysicsBody owner;
        float start, end;
        SAPEntry previous, next;

        public float Start { get => start; set => start = value; }
        public float End { get => end; set => end = value; }
        internal PhysicsBody Owner { get => owner; set => owner = value; }
        internal SAPEntry Previous { get => previous; set => previous = value; }
        internal SAPEntry Next { get => next; set => next = value; }
    }


    class PhysicsManager
    {
        private static PhysicsManager me;
        private List<CollidingObject> collisionsToCheck;
        List<CollidingObject> colliding;
        private bool debugging;
        private double timeInterval;
        SAPEntry sapX, sapY;
        double start;
        float gravityModifier = 0.1f;

        List<PhysicsBody> allPhysicsObjects;
        private long lastUpdate;
        private long lastDebugDraw;
        private PhysicsManager()
        {
            allPhysicsObjects = new List<PhysicsBody>();
            colliding = new List<CollidingObject>();

            lastUpdate = Bootstrap.getCurrentMillis();

            collisionsToCheck = new List<CollidingObject>();

            start = 0;
            // 60 FPS
            TimeInterval = 0.02;
        }

        public static PhysicsManager getInstance()
        {
            if (me == null)
            {
                me = new PhysicsManager();
            }

            return me;
        }


        public long LastUpdate { get => lastUpdate; set => lastUpdate = value; }
        public bool Debugging { get => debugging; set => debugging = value; }
        public double TimeInterval { get => timeInterval; set => timeInterval = value; }
        public long LastDebugDraw { get => lastDebugDraw; set => lastDebugDraw = value; }
        public float GravityModifier { get => gravityModifier; set => gravityModifier = value; }

        public void addPhysicsObject(PhysicsBody body)
        {
            if (allPhysicsObjects.Contains(body))
            {
                return;
            }

            allPhysicsObjects.Add(body);

        }

        public void removePhysicsObject(PhysicsBody body)
        {
            allPhysicsObjects.Remove(body);
        }

        public CollidingObject checkForExistingCollision(PhysicsBody A, PhysicsBody B)
        {
            foreach (CollidingObject col in colliding)
            {
                if (col.A == A && col.B == B)
                {
                    return col;
                }
            }

            return null;
        }

        public void clearList(SAPEntry node)
        {
            //Let's clear everything so the garbage collector can do its
            // work

            if (node == null)
            {
                return;
            }

            while (node != null && node.Next != null)
            {
                node = node.Next;
                node.Previous.Next = null;
                node.Previous = null;
            }

            node.Previous = null;

        }

        public SAPEntry addToList(SAPEntry node, SAPEntry entry)
        {
            SAPEntry current;

            current = node;


            // Start our list.
            if (current == null)
            {
                return entry;
            }

            // Is this our new head?
            if (entry.Start < current.Start)
            {
                current.Previous = entry;
                entry.Next = current;
                return entry;
            }

            // Look for where we get inserted.
            while (current.Next != null && entry.Start > current.Next.Start)
            {
                current = current.Next;
            }


            if (current.Next != null)
            {
                // Insert ourselves into a chain.
                entry.Previous = current;
                entry.Next = current.Next;
                current.Next = entry;
            }
            else
            {
                // We're at the end.
                current.Next = entry;
                entry.Previous = current;
            }


            return node;

        }

        public void outputList(SAPEntry node)
        {
            SAPEntry pointer = node;
            int counter = 0;
            string text = "";


            if (pointer == null)
            {
                Debug.getInstance().log("No List");
                return;
            }

            while (pointer != null)
            {
                text += "[" + counter + "]: " + pointer.Owner.Parent + ", ";
                pointer = pointer.Next;
                counter += 1;
            }

            Debug.getInstance().log("List:" + text);

        }

        public bool willTick()
        {


            if (start < TimeInterval)
            {
                return false;
            }


            return true;

        }

        public bool update()
        {
            CollisionHandler ch, ch2;
            List<CollidingObject> toRemove;


            start += Bootstrap.getDeltaTime();



            if (willTick() == false)
            {
                return false;
            }

            //            Debug.Log("Tick: " + Bootstrap.TimeElapsed);

            lastUpdate = Bootstrap.getCurrentMillis();


            toRemove = new List<CollidingObject>();


            foreach (PhysicsBody body in allPhysicsObjects)
            {
                if (body.UsesGravity)
                {
                    body.applyGravity(GravityModifier);
                }

                body.physicsTick();
                body.recalculateColliders();
            }


            // Check for old collisions that should be persisted
            foreach (CollidingObject col in colliding)
            {
                ch = (CollisionHandler)col.A.Parent;
                ch2 = (CollisionHandler)col.B.Parent;
                Vector impulse;

                impulse = checkCollisionBetweenObjects(col.A, col.B);

                //bool isRight = isRightOf(col.A, col.B);
                //bool isBelow = isTopOf(col.A, col.B);

                string direction = getCollisionDirection(col.A, col.B);

                if (impulse != null)
                {
                    ch.onCollisionStay(col.B);
                    ch2.onCollisionStay(col.A);
                    if (col.A.Parent is PositionCollisionHandler)
                    {
                        ((PositionCollisionHandler)col.A.Parent).onCollisionStay(col.B, direction);
                    }
                    if (col.B.Parent is PositionCollisionHandler)
                    {
                        ((PositionCollisionHandler)col.B.Parent).onCollisionStay(col.A, inverseDirection(direction));
                    }
                }
                else
                {
                    ch.onCollisionExit(col.B);
                    ch2.onCollisionExit(col.A);
                    if (col.A.Parent is PositionCollisionHandler)
                    {
                       
                    ((PositionCollisionHandler)col.A.Parent).onCollisionExit(col.B, direction);
                    }
                    if (col.B.Parent is PositionCollisionHandler)
                    {
                       ((PositionCollisionHandler)col.B.Parent).onCollisionExit(col.A, inverseDirection(direction));
                    }
                    toRemove.Add(col);
                }

            }

            foreach (CollidingObject col in toRemove)
            {
                colliding.Remove(col);
            }

            toRemove.Clear();
            // Check for new collisions
            checkForCollisions();

            start -= TimeInterval;

            //            Debug.Log("Time Interval is " + (Bootstrap.getCurrentMillis() - lastUpdate) + ", " + colliding.Count);


            return true;
        }

        public void drawDebugColliders()
        {
            foreach (PhysicsBody body in allPhysicsObjects)
            {
                // Debug drawing - always happens.
                body.drawMe();
            }
        }

        private Vector checkCollisionBetweenObjects(PhysicsBody a, PhysicsBody b)
        {
            Vector impulse;

            foreach (Collider col in a.getColliders())
            {
                foreach (Collider col2 in b.getColliders())
                {
                    impulse = col.checkCollision(col2);

                    if (impulse != null)
                    {
                        return impulse;
                    }
                }
            }

            return null;

        }

        // omg this won't scale omg
        private void broadPassBruteForce()
        {
            CollidingObject tmp;

            if (allPhysicsObjects.Count < 2)
            {
                // Nothing to collide.
                return;
            }

            for (int i = 0; i < allPhysicsObjects.Count; i++)
            {
                for (int j = 0; j < allPhysicsObjects.Count; j++)
                {

                    if (i == j)
                    {
                        continue;
                    }

                    if (checkForExistingCollision(allPhysicsObjects[i], allPhysicsObjects[j]) != null)
                    {
                        continue;
                    }

                    if (checkForExistingCollision(allPhysicsObjects[j], allPhysicsObjects[i]) != null)
                    {
                        continue;
                    }

                    tmp = new CollidingObject();

                    tmp.A = allPhysicsObjects[i];
                    tmp.B = allPhysicsObjects[j];

                    collisionsToCheck.Add(tmp);

                }
            }

            Debug.Log("Checking " + collisionsToCheck.Count + " collisions");

        }

        public bool findColliding(PhysicsBody a, PhysicsBody b)
        {
            foreach (CollidingObject ob in colliding)
            {
                if (ob.A == a && ob.B == b)
                {
                    return true;
                }
            }

            return false;
        }

        public bool isRightOf(PhysicsBody a, PhysicsBody b)
        {
            float aMidX = a.MinAndMaxX[0] + (a.MinAndMaxX[1] - a.MinAndMaxX[0]) / 2;
            float bMidX = b.MinAndMaxX[0] + (b.MinAndMaxX[1] - b.MinAndMaxX[0]) / 2;

            float bMaxX = b.MinAndMaxX[1];
            float aMinX = a.MinAndMaxX[0];



            return aMinX >= bMaxX;

            return (aMidX - bMidX) > 0;
        }
        public bool isTopOf(PhysicsBody a, PhysicsBody b)
        {
            //float aMidY = a.MinAndMaxY[0] + (a.MinAndMaxY[1] - a.MinAndMaxY[0]) / 2;
            //float bMidY = b.MinAndMaxY[0] + (b.MinAndMaxY[1] - b.MinAndMaxY[0]) / 2;

            //float bMinY = b.MinAndMaxY[0];
            //float aMaxY = a.MinAndMaxY[1];

            float bMinY = b.MinAndMaxY[0];
            float bMaxY = b.MinAndMaxY[1];
            float aMinY = a.MinAndMaxY[0];
            float aMaxY = a.MinAndMaxY[1];

            float distToTop = bMaxY - aMinY;

            return bMaxY - aMinY < 5;

            return aMaxY <= bMinY;

            //return (aMidY -bMidY) > 0;
        }
        public string inverseDirection(string direction)
        {
            if (direction == "Right")
            {
                return "Left";
            }
            if (direction == "Left")
            {
                return "Right";
            }
            if (direction == "Top")
            {
                return "Bottom";
            }

            return "Top";

        }
        public string getCollisionDirection(PhysicsBody a, PhysicsBody b)
        {
            float bMinY =b.MinAndMaxY[0];
            float bMaxY = b.MinAndMaxY[1];
            float aMinY = a.MinAndMaxY[0];
            float aMaxY = a.MinAndMaxY[1];

            float bMinX = b.MinAndMaxX[0];
            float bMaxX = b.MinAndMaxX[1];
            float aMinX = a.MinAndMaxX[0];
            float aMaxX = a.MinAndMaxX[1];

            float diffTop = Math.Abs(bMaxY - aMinY);
            float diffBottom = Math.Abs(bMinY - aMaxY);
            float diffLeft = Math.Abs(bMaxX - aMinX);
            float diffRight = Math.Abs(bMinX - aMaxX);

            float[] diffs = { diffTop, diffBottom, diffLeft, diffRight };
            float minValue = diffs.Min();

            if (diffTop == minValue)
            {
                return "Top";
            }

            if (diffBottom == minValue)
            {
                return "Bottom";
            }

            if (diffLeft == minValue)
            {
                return "Left";
            }


            return "Right";


        }


        private void narrowPass()
        {
            Vector impulse;
            float massTotal, massa, massb;
            float massProp = 0.0f;

            //            Debug.getInstance().log("Active objects " + collisionsToCheck.Count);

            foreach (CollidingObject ob in collisionsToCheck)
            {


                impulse = checkCollisionBetweenObjects(ob.A, ob.B);


                if (impulse != null)
                {

                    if (ob.A.PassThrough != true && ob.B.PassThrough != true)
                    {

                       

                        massTotal = ob.A.Mass + ob.B.Mass;

                        if (ob.A.Kinematic)
                        {
                            massProp = 1;
                        }
                        else
                        {
                            massProp = ob.A.Mass / massTotal;

                        }

                        if (ob.A.ImpartForce)
                        {
                            ob.A.impartForces(ob.B, massProp);
                            ob.A.reduceForces(1.0f - massProp);
                        }


                        if (ob.A.StopOnCollision)
                        {
                            ob.A.stopForces();
                        }


                        massb = massProp;

                        if (ob.B.Kinematic == false)
                        {
                            //Debug.Log ("Applying force to B " + impulse + ", " + massProp);
                            ob.B.Parent.Transform.translate(-1 * (impulse.X * massProp), -1 * (impulse.Y * massProp));
                        }


                        if (ob.B.Kinematic)
                        {
                            massProp = 1;
                        }
                        else
                        {
                            massProp = 1.0f - massProp;
                        }

                        massa = massProp;


                        if (ob.A.Kinematic == false)
                        {

                            ob.A.Parent.Transform.translate((impulse.X * massProp), (impulse.Y * massProp));
                        }

                        if (ob.B.StopOnCollision)
                        {            
                            ob.B.stopForces();
                        }


                    }



                    //bool isRight = isRightOf(ob.A, ob.B);
                    //bool isBelow = isTopOf(ob.A, ob.B);
                    string direction = getCollisionDirection(ob.A, ob.B);

                    if (findColliding(ob.A, ob.B) == false)
                    {
                        ((CollisionHandler)ob.A.Parent).onCollisionEnter(ob.B);
                        ((CollisionHandler)ob.B.Parent).onCollisionEnter(ob.A);

                        if (ob.A.Parent is PositionCollisionHandler)
                        {
                            ((PositionCollisionHandler)ob.A.Parent).onCollisionEnter(ob.B, direction);
                            //float bMinY = ob.B.MinAndMaxY[0];
                            //float aMaxY = ob.A.MinAndMaxY[1];

                            //Debug.Log("A: " + aMaxY + " B: " + bMinY);
                            //float aMidX = ob.A.MinAndMaxX[1] - ob.A.MinAndMaxX[0];
                            //float bMidX = ob.B.MinAndMaxX[1] - ob.B.MinAndMaxX[0];
                            //Debug.Log("amidx: " + aMidX);
                            //Debug.Log("bmidx: " + bMidX);
                        }
                        if (ob.B.Parent is PositionCollisionHandler)
                        {
                            ((PositionCollisionHandler)ob.B.Parent).onCollisionEnter(ob.A, inverseDirection(direction));
                            float bMinY = ob.B.MinAndMaxY[0];
                            float bMaxY = ob.B.MinAndMaxY[1];
                            float aMinY = ob.A.MinAndMaxY[0];
                            float aMaxY = ob.A.MinAndMaxY[1];

                            float bMinX = ob.B.MinAndMaxX[0];
                            float bMaxX = ob.B.MinAndMaxX[1];
                            float aMinX = ob.A.MinAndMaxX[0];
                            float aMaxX = ob.A.MinAndMaxX[1];



                           // Debug.Log("Diff Top: "+(bMaxY - aMinY));
                            float diffTop = Math.Abs(bMaxY - aMinY);
                           // Debug.Log("Diff Bottom: " + (bMinY - aMaxY));
                            float diffBottom = Math.Abs(bMinY - aMaxY);
                           // Debug.Log("Diff Left: " + (bMaxX - aMinX));
                            float diffRight = Math.Abs(bMaxX - aMinX);
                           // Debug.Log("Diff Right: " + (bMinX - aMaxX));
                            float diffLeft = Math.Abs(bMinX - aMaxX);
                            //if(bMaxY - aMinY<5)
                            //float aMidX = ob.A.MinAndMaxX[1] - ob.A.MinAndMaxX[0];
                            //float bMidX = ob.B.MinAndMaxX[1] - ob.B.MinAndMaxX[0];
                            //Debug.Log("amidx: " + aMidX);
                            //Debug.Log("bmidx: " + bMidX);
                        }

                        colliding.Add(ob);
                    }
                    else
                    {
                        ((CollisionHandler)ob.A.Parent).onCollisionStay(ob.B);
                        ((CollisionHandler)ob.B.Parent).onCollisionStay(ob.A);
                        if (ob.A.Parent is PositionCollisionHandler)
                        {
                            ((PositionCollisionHandler)ob.A.Parent).onCollisionStay(ob.B, direction);
                        }
                        if (ob.B.Parent is PositionCollisionHandler)
                        {
                            ((PositionCollisionHandler)ob.B.Parent).onCollisionStay(ob.A, inverseDirection(direction));
                        }
                    }


                    if (ob.A.ReflectOnCollision)
                    {
                        Debug.Log("Reflecting A");
                        ob.A.reflectForces(impulse);
                    }
                    if (ob.B.ReflectOnCollision)
                    {
                        ob.B.reflectForces(impulse);
                        Debug.Log("Reflecting B");
                    }


                }


            }
        }

        public void reportCollisionsInAxis(SAPEntry start)
        {
            List<SAPEntry> activeObjects;
            List<int> toRemove;
            CollidingObject col;
            activeObjects = new List<SAPEntry>();
            toRemove = new List<int>();
            col = new CollidingObject();



            while (start != null)
            {

                activeObjects.Add(start);


                for (int i = 0; i < activeObjects.Count; i++)
                {

                    if (start == activeObjects[i])
                    {
                        continue;
                    }

                    if (start.Start >= activeObjects[i].End)
                    {
                        toRemove.Add(i);
                    }
                    else
                    {
                        col = new CollidingObject();

                        if (start.Owner.Mass > activeObjects[i].Owner.Mass)
                        {
                            col.A = start.Owner;
                            col.B = activeObjects[i].Owner;
                        }
                        else
                        {
                            col.B = start.Owner;
                            col.A = activeObjects[i].Owner;
                        }


                        collisionsToCheck.Add(col);
                        //                        Debug.getInstance().log("Adding potential collision: " + col.ToString());

                    }


                }


                for (int j = toRemove.Count - 1; j >= 0; j--)
                {
                    activeObjects.RemoveAt(toRemove[j]);
                }

                toRemove.Clear();

                start = start.Next;

            }

//            Debug.Log("Checking " + collisionsToCheck.Count + " collisions");

        }


        public void broadPassSearchAndSweep()
        {
            SAPEntry sx, sy;
            float[] x, y;
            sapX = null;
            sapY = null;
            List<PhysicsBody> candidates = new List<PhysicsBody>();


            foreach (PhysicsBody body in allPhysicsObjects)
            {
                sx = new SAPEntry();

                x = body.MinAndMaxX;

                sx.Owner = body;
                sx.Start = x[0];
                sx.End = x[1];


                sapX = addToList(sapX, sx);

            }

            //            outputList (sapX);
            // What we have at this point is a sorted linked list of all
            // our objects in order.  So now we go over them all to see 
            // what are viable collision candidates.  If they don't overlap 
            // in the axis, they can't collide so don't bother checking them.

            // Now we find all the candidates that overlap in 
            // the Y axis from those that overlap in the X axis.
            // A two pass sweep and prune.

            reportCollisionsInAxis(sapX);
            clearList(sapX);

        }
        public void broadPass()
        {
            broadPassSearchAndSweep();
//            broadPassBruteForce();
        }



        private void checkForCollisions()
        {
            broadPass();
            narrowPass();

            collisionsToCheck.Clear();


        }

    }
}
