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
        // Input
        public const float ZoomSpeed = 2.5f;

        // Networking
        public int ServerHealth = 0;
        public Vector2 ServerPosition = Vector2.Zero;

        // Movement
        public bool IsRunning;
        public Vector2 Position;
        public Vector2 MoveDirection = Vector2.Zero;
        public Camera2D Camera;

        public float CameraZoom;

        private Vector2 lastPosition;

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
            lastPosition = Position;

            MoveDirection = Vector2.Zero;

            // Input
            if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
                MoveDirection.Y -= 1f;

            if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
                MoveDirection.Y += 1f;

            if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
                MoveDirection.X -= 1f;
            
            if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
                MoveDirection.X += 1f;

            IsRunning = Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT);

            Position = Vector2.Lerp(lastPosition, ServerPosition, 20f * deltaTime);
            //Position = NetworkPosition;

            Camera.target = Vector2.Lerp(Camera.target, Position, 3.5f * deltaTime);

            CameraZoom += Raylib.GetMouseWheelMove() * ZoomSpeed;
            CameraZoom = GameMath.Clamp(CameraZoom, 20f, 100f);

            Camera.zoom = CameraZoom;
        }

        public void Draw(float deltaTime)
        {
            // Other players
            for (int i = 0; i < Engine.OtherPlayerPositions.Count; i++)
                Raylib.DrawCircleV(Engine.OtherPlayerPositions[i], 0.4f, Color.GREEN);

            Raylib.DrawCircleV(Position, 0.4f, Color.BLUE);

            // Health bar
            //UI.DrawTextWorld(, new Vector2(Position.X, Position.Y), 0.5f);
        }

        public void UIDraw()
        {
            UI.DrawText($"Health: {ServerHealth}", new Vector2(UI.TopRight.X - 150f, UI.TopRight.Y));
        }
    }
}
