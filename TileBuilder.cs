using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Raylib_cs;

namespace Munita
{
    public class TileBuilder
    {
        public Texture2D testTexture;

        public Rectangle tileSelection = new Rectangle(0f, 0f, 1f, 1f);
        public Rectangle TileRect = new Rectangle(0f, 0f, 1f, 1f);
        public Rectangle TileOrigRect = new Rectangle(0f, 0f, 16f, 16f);

        private bool isPlacing = false;

        private Vector2 initialMousePos = new Vector2();
        private Vector2 tileCursorPos = new Vector2();

        private static Color cursorColor = new Color(255, 255, 255, 128);

        public void Initialize()
        {
            testTexture = Raylib.LoadTexture("Assets/Textures/stone_wall.png");
        }

        public void Update(Camera2D camera)
        {
            var mousePos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), camera);
            tileCursorPos = GameMath.NearestGridCoord(mousePos);

            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                isPlacing = true;
                initialMousePos = tileCursorPos;
            }

            if (Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
            {
                isPlacing = false;
                tileSelection.width = 1f;
                tileSelection.height = 1f;
            }

            if (isPlacing)
            {
                var width = initialMousePos.X - tileCursorPos.X;
                //var height = initialMousePos.Y - tileCursorPos.Y;

                // TODO: Initial selection point moves when dragging towards upper left
                width = -width;
                //height = -height;

                if (width == -0f)
                    width = 1f;

                //if (height == -0f)
                //    height = 1f;

                if (width < 0f)
                {
                    tileSelection.x = initialMousePos.X + width;
                    tileSelection.width = MathF.Abs(width);
                }
                else
                {
                    tileSelection.width = width;
                }

                /*if (height < 0f)
                {
                    tileSelection.y = initialMousePos.Y + height;
                    tileSelection.height = MathF.Abs(height);
                }
                else
                {
                    tileSelection.height = height;
                }*/

                /*blockAmount = (int)MathF.Round(MathF.Abs(width) + MathF.Abs(height));
                Console.WriteLine($"{blockAmount}");*/
            }
            else
            {
                tileSelection.x = tileCursorPos.X;
                tileSelection.y = tileCursorPos.Y;
            }

            //Console.WriteLine($"{tileCursor.x}, {tileCursor.y}, {tileCursor.width}, {tileCursor.height}");
        }

        public void Draw()
        {
            if (isPlacing)
            {
                var width = initialMousePos.X - tileCursorPos.X;
                var height = initialMousePos.Y - tileCursorPos.Y;

                width = -width;
                height = -height;

                if (width < 0f)
                {
                    for (int x = (int)tileCursorPos.X; x < (int)initialMousePos.X; x++)
                    {
                        var tileTest = new Rectangle(x, initialMousePos.Y, 1f, 1f);
                        Raylib.DrawTexturePro(testTexture, TileOrigRect, tileTest, Vector2.Zero, 0f, Color.WHITE);
                    }
                }
                else
                {
                    for (int x = (int)initialMousePos.X; x < (int)tileCursorPos.X; x++)
                    {
                        var tileTest = new Rectangle(x, initialMousePos.Y, 1f, 1f);
                        Raylib.DrawTexturePro(testTexture, TileOrigRect, tileTest, Vector2.Zero, 0f, Color.WHITE);
                    }
                }
            }

            //Raylib.DrawTexturePro(testTexture, TileOrigRect, TileRect, Vector2.Zero, 0f, Color.WHITE);

            Raylib.DrawRectangleRec(tileSelection, cursorColor);
        }
    }
}
