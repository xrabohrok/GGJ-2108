using UnityEngine;
using System.Collections;
using System;

namespace BabyMap
{
    public class IntVector2 : System.Object
    {

        /*
         * PRIVATE VARIABLES
         */
        private int _x, _y;


        /*
         * GETTERS/SETTERS
         */

        // Because I hate getting the wrong capitalization on Vector2. -mw
        public int X
        {
            get { return this._x; }
            set { _x = (int)value; }
        }

        public int x
        {
            get { return this._x; }
            set { _x = (int)value; }
        }

        public int Y
        {
            get { return this._y; }
            set { _y = (int)value; }
        }

        public int y
        {
            get { return this._y; }
            set { _y = (int)value; }
        }

        /*
         * OPERATOR OVERRIDES
         */
        public static bool operator ==(IntVector2 a, IntVector2 b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
                return false;
            return ((a.X == b.X) && (a.Y == b.Y));
        }

        public static bool operator !=(IntVector2 a, IntVector2 b)
        {
            return !((a.X == b.X) && (a.Y == b.Y));
        }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            Debug.Log("a:" + a + ", b:" + b);
            if (!object.ReferenceEquals(a, null) && !object.ReferenceEquals(b, null))
                return new IntVector2(a.X + b.X, a.Y + b.Y);
            else
                throw new NullReferenceException("Don't add things that don't exist.");
        }

        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.X - b.X, a.Y - b.Y);
        }

        public override bool Equals(System.Object b)
        {
            if (b == null)
                return false;
            IntVector2 p = b as IntVector2;
            if ((System.Object)p == null)
                return false;

            return this == p;
        }

        public bool Equals(IntVector2 b)
        {
            if ((object)b == null)
                return false;
            return this == b;
        }

        public override int GetHashCode()
        {
            return (this._x * 10000) + this._y;
        }

        public override string ToString()
        {
            return "(" + this._x + ", " + this._y + ")";
        }


        /*
         * STATIC FIELDS
         */
        public static IntVector2 down = new IntVector2(0, -1);
        public static IntVector2 Down = new IntVector2(0, -1);
        public static IntVector2 up = new IntVector2(0, 1);
        public static IntVector2 Up = new IntVector2(0, 1);
        public static IntVector2 left = new IntVector2(-1, 0);
        public static IntVector2 Left = new IntVector2(-1, 0);
        public static IntVector2 right = new IntVector2(1, 0);
        public static IntVector2 Right = new IntVector2(1, 0);
        public static IntVector2 zero = new IntVector2(0, 0);
        public static IntVector2 Zero = new IntVector2(0, 0);


        /*
         * CONSTRUCTORS
         */

        public IntVector2()
        {
            this._x = 0;
            this._y = 0;
        }

        public IntVector2(int x, int y)
        {
            this._x = x;
            this._y = y;
        }


        /*
         * PUBLIC FUNCTIONS
         */
        public double Magnitude()
        {
            return IntVector2.Distance(this, new IntVector2(0, 0));
        }

        public static double Distance(IntVector2 from, IntVector2 to)
        {
            return Math.Sqrt(
                  Math.Pow((to.x - from.x), 2)
                + Math.Pow((to.y - from.y), 2)
            );
        }
    }
}