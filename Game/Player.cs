using System;
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

        public int Health { get; private set; }

        public Vector2 Position;
        public Vector2 MoveDirection = new Vector2(0f, 0f);
        public Camera2D Camera;

        public float CameraZoom;

        private Vector2 lastPosition;
        private float currentSpeed;

        public void Initialize()
        {
            Respawn();

            Camera = new Camera2D();
            Camera.target = new Vector2(Position.X, Position.Y);
            Camera.offset = new Vector2(UI.CenterPivot.X, UI.CenterPivot.Y);
            Camera.rotation = 0.0f; // Flip camera so that north is +Y
            Camera.zoom = 100.0f;

            CameraZoom = Camera.zoom;
        }

        public void Update(bool isClient, float deltaTime)
        {
            MoveDirection = Vector2.Zero;
            lastPosition = Position;
            
            if (isClient)
            {
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
            }

            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT))
                currentSpeed = RunSpeed;
            else
                currentSpeed = WalkSpeed;

            Position.X += MoveDirection.X * currentSpeed * deltaTime;

            if (Position.X < 0f || Position.X > 32f)
                Kill();

            for (int i = 0; i < World.WallCols.Count; i++)
            {
                var inBounds = Raylib.CheckCollisionCircleRec(Position, 5f, World.WallCols[i]);

                if (inBounds)
                {
                    // Not sure why this needs to be offset
                    var checkPos = Position - new Vector2(0.5f, 0.5f);
                    var isCollidingX = Raylib.CheckCollisionCircleRec(checkPos, 0.4f, World.WallCols[i]);

                    if (isCollidingX)
                        Position.X = lastPosition.X;
                }
                else
                    continue;
            }

            Position.Y += MoveDirection.Y * currentSpeed * deltaTime;

            if (Position.Y < 0f || Position.Y > 32f)
                Kill();

            for (int i = 0; i < World.WallCols.Count; i++)
            {
                var inBounds = Raylib.CheckCollisionCircleRec(Position, 5f, World.WallCols[i]);

                if (inBounds)
                {
                    var checkPos = Position - new Vector2(0.5f, 0.5f);
                    var isCollidingY = Raylib.CheckCollisionCircleRec(checkPos, 0.4f, World.WallCols[i]);

                    if (isCollidingY)
                        Position.Y = lastPosition.Y;
                }
                else
                    continue;
            }

            if (Health <= 0)
                Respawn();

            Camera.target = Vector2.Lerp(Camera.target, Position, 3.5f * deltaTime);

            CameraZoom += Raylib.GetMouseWheelMove() * ZoomSpeed;
            CameraZoom = GameMath.Clamp(CameraZoom, 20f, 100f);

            Camera.zoom = CameraZoom;
        }

        public void Draw()
        {
            Raylib.DrawCircleV(Position, 0.4f, Color.BLUE);
        }

        public void Damage(int amount)
        {
            Health = GameMath.Clamp(Health - amount, 0, 100);
        }

        public void Heal(int amount)
        {
            Health = GameMath.Clamp(Health + amount, 0, 100);
        }

        public void Kill()
        {
            Health = -999;
        }

        public void Respawn()
        {
            Health = 100;
            Position = new Vector2(16f, 16f);
        }
    }
}
