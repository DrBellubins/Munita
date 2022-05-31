﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Raylib_cs;

namespace Munita
{
    public class ClientPlayer
    {
        // Input
        public const float ZoomSpeed = 2.5f;

        // Movement
        public bool IsRunning;
        public Vector2 Position;
        public Vector2 MoveDirection = Vector2.Zero;
        public Camera2D Camera;

        public float CameraZoom;

        // Temporary
        private static Vector2 networkPosition;

        public void Initialize()
        {
            Camera = new Camera2D();
            Camera.target = new Vector2(Position.X, Position.Y);
            Camera.offset = new Vector2(UI.CenterPivot.X, UI.CenterPivot.Y);
            Camera.rotation = 0.0f; // Flip camera so that north is +Y
            Camera.zoom = 100.0f;

            CameraZoom = Camera.zoom;
        }

        public void Update(float deltaTime)
        {
            MoveDirection = Vector2.Zero;

            // Input
            if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
            {
                MoveDirection.Y -= 1f;
            }

            if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
            {
                MoveDirection.Y += 1f;
            }

            if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
            {
                MoveDirection.X -= 1f;
            }
            
            if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
            {
                MoveDirection.X += 1f;
            }

            IsRunning = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);

            Position = networkPosition;

            Camera.target = Vector2.Lerp(Camera.target, Position, 3.5f * deltaTime);

            CameraZoom += Raylib.GetMouseWheelMove() * ZoomSpeed;
            CameraZoom = GameMath.Clamp(CameraZoom, 20f, 100f);

            Camera.zoom = CameraZoom;
        }

        public void Draw()
        {
            Raylib.DrawCircleV(Position, 0.4f, Color.BLUE);
        }

        public static void NetUpdate(Vector2 pos)
        {
            networkPosition = pos;
        }
    }
}
