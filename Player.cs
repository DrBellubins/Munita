﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Raylib_cs;

namespace Munita
{
    public class Player
    {
        // Movement
        public const float WalkSpeed = 1.45f;
        public const float RunSpeed = 3.5f;

        public const float Dampening = 17.0f;

        // Input
        public const float ZoomSpeed = 2.5f;

        public Vector2 Position;
        public Camera2D Camera;

        public float CameraZoom;

        private Vector2 moveDirection = new Vector2(0f, 0f);
        private float currentSpeed;

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
            if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
            {
                moveDirection.Y -= 1f;
            }

            if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
            {
                moveDirection.Y += 1f;
            }

            if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
            {
                moveDirection.X -= 1f;
            }

            if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
            {
                moveDirection.X += 1f;
            }

            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
                currentSpeed = RunSpeed;
            else
                currentSpeed = WalkSpeed;

            //moveDirection = GameMath.Vector2Normalized(moveDirection);
            //moveDirection = Vector2.Normalize(moveDirection);

            Position += moveDirection * currentSpeed * deltaTime;

            Camera.target = Vector2.Lerp(Camera.target, Position, 3.5f * deltaTime);

            CameraZoom += Raylib.GetMouseWheelMove() * ZoomSpeed;
            CameraZoom = GameMath.Clamp(CameraZoom, 20f, 100f);

            Camera.zoom = CameraZoom;

            //Console.WriteLine($"{moveDirection.X} {moveDirection.Y}");

            moveDirection = Vector2.Zero;
        }

        public void Draw()
        {
            Raylib.DrawCircleV(Position, 0.4f, Color.BLUE);
        }
    }
}