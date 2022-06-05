﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Raylib_cs;

namespace Munita
{
    public class UI
    {
        public static Vector2 CenterPivot
        {
            get { return new Vector2((float)ClientEngine.ScreenWidth / 2f, (float)ClientEngine.ScreenHeight / 2f); }
        }

        public static Vector2 TopCenterPivot
        {
            get { return new Vector2((float)ClientEngine.ScreenWidth / 2f, 0f); }
        }

        public static Vector2 BottomCenterPivot
        {
            get { return new Vector2((float)ClientEngine.ScreenWidth / 2f, (float)ClientEngine.ScreenHeight); }
        }

        public static Vector2 LeftCenterPivot
        {
            get { return new Vector2(0f, (float)ClientEngine.ScreenHeight / 2f); }
        }

        public static Vector2 RightCenterPivot
        {
            get { return new Vector2((float)ClientEngine.ScreenWidth, (float)ClientEngine.ScreenHeight / 2f); }
        }

        public static void DrawText(string text, Vector2 position)
        {
            Raylib.DrawTextEx(ClientEngine.MainFont, text, position, 28, 0.0f, Color.WHITE);
        }

        public static void DrawText(string text, float size, Vector2 position)
        {
            Raylib.DrawTextEx(ClientEngine.MainFont, text, position, size, 0.0f, Color.WHITE);
        }

        public static void DrawText(string text, Vector2 position, Color color)
        {
            Raylib.DrawTextEx(ClientEngine.MainFont, text, position, 28, 0.0f, color);
        }

        public static void DrawText(string text, float size, Vector2 position, Color color)
        {
            Raylib.DrawTextEx(ClientEngine.MainFont, text, position, size, 0.0f, color);
        }

        public static void DrawText(string text, float x, float y)
        {
            Raylib.DrawTextEx(ClientEngine.MainFont, text, new Vector2(x, y), 28, 0.0f, Color.WHITE);
        }

        public static void DrawText(string text, float size, float x, float y)
        {
            Raylib.DrawTextEx(ClientEngine.MainFont, text, new Vector2(x, y), size, 0.0f, Color.WHITE);
        }

        public static Rectangle CenterRect(Rectangle rect)
        {
            var outRect = new Rectangle(0.0f, 0.0f, rect.width, rect.height);

            outRect.x = CenterPivot.X - (rect.width / 2f);
            outRect.y = CenterPivot.Y - (rect.height / 2f);

            return outRect;
        }

        public static Rectangle AlignRect(Rectangle rect, Vector2 alignment)
        {
            var outRect = new Rectangle(0.0f, 0.0f, rect.width, rect.height);

            outRect.x = alignment.X - (rect.width / 2f);
            outRect.y = alignment.Y - (rect.height / 2f);

            return outRect;
        }
    }
}
