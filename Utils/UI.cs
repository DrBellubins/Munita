using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Munita
{
    public class UI
    {
        public static Vector2 CenterPivot
        {
            get { return new Vector2((float)Engine.ScreenWidth / 2f, (float)Engine.ScreenHeight / 2f); }
        }

        public static Vector2 TopCenterPivot
        {
            get { return new Vector2((float)Engine.ScreenWidth / 2f, 0f); }
        }

        public static Vector2 BottomCenterPivot
        {
            get { return new Vector2((float)Engine.ScreenWidth / 2f, (float)Engine.ScreenHeight); }
        }

        public static Vector2 LeftCenterPivot
        {
            get { return new Vector2(0f, (float)Engine.ScreenHeight / 2f); }
        }

        public static Vector2 RightCenterPivot
        {
            get { return new Vector2((float)Engine.ScreenWidth, (float)Engine.ScreenHeight / 2f); }
        }
    }
}
