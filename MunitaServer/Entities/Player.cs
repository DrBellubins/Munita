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

        public int Health { get; set; }

        public Vector2 Position;

        // Clientside data
        public System.Net.IPEndPoint EndPoint;
        public bool IsRunning;
        public Vector2 MoveDirection;

        private Vector2 lastPosition;
        private float currentSpeed;

        public void Initialize()
        {
            Respawn();
        }

        public void Update(float deltaTime)
        {
            lastPosition = Position;

            if (IsRunning)
                currentSpeed = RunSpeed;
            else
                currentSpeed = WalkSpeed;

            // speed hack prevention
            MoveDirection.X = GameMath.Clamp(MoveDirection.X, -1f, 1f);

            Position.X += MoveDirection.X * currentSpeed * deltaTime;

            if (Position.X < 0f || Position.X > 32f)
                Kill();

            for (int i = 0; i < World.WallCols.Count; i++)
            {
                var wallPos = new Vector2(World.WallCols[i].x, World.WallCols[i].y);
                var distance = Vector2.Distance(Position, wallPos);

                if (distance < 4f)
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

            // speed hack prevention
            MoveDirection.Y = GameMath.Clamp(MoveDirection.Y, -1f, 1f);
            
            Position.Y += MoveDirection.Y * currentSpeed * deltaTime;

            if (Position.Y < 0f || Position.Y > 32f)
                Kill();

            for (int i = 0; i < World.WallCols.Count; i++)
            {
                var wallPos = new Vector2(World.WallCols[i].x, World.WallCols[i].y);
                var distance = Vector2.Distance(Position, wallPos);

                if (distance < 4f)
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
