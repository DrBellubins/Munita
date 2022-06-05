using System;
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

        // Networking
        public Vector2 NetworkPosition = Vector2.Zero;
        public List<Vector2> PrevOtherPlayerPositions = new List<Vector2>();

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

            Position = Vector2.Lerp(lastPosition, NetworkPosition, 10f * deltaTime);
            //Position = NetworkPosition;

            Camera.target = Vector2.Lerp(Camera.target, Position, 3.5f * deltaTime);

            CameraZoom += Raylib.GetMouseWheelMove() * ZoomSpeed;
            CameraZoom = GameMath.Clamp(CameraZoom, 20f, 100f);

            Camera.zoom = CameraZoom;
        }

        public void Draw(float deltaTime)
        {
            // Other players
            for (int i = 0; i < ClientEngine.OtherPlayerPositions.Count; i++)
            {
                var currentPos = ClientEngine.OtherPlayerPositions[i];        

                if (ClientEngine.OtherPlayerPositions.Count == PrevOtherPlayerPositions.Count)
                {
                    var previousPos = PrevOtherPlayerPositions[i];

                    // TODO: Does not lerp at all, most likely currentPos & previousPos
                    // are identical somehow
                    var otherPlayerPos = Vector2.Lerp(currentPos, previousPos, 0.5f * deltaTime);

                    Debug.DrawText($"({currentPos.X}, {currentPos}) - ({previousPos.X}, {previousPos})");

                    Raylib.DrawCircleV(otherPlayerPos, 0.4f, Color.GREEN);
                }
            }

            //Raylib.DrawCircleV(networkPosition, 0.4f, Color.RED);
            Raylib.DrawCircleV(Position, 0.4f, Color.BLUE);

            PrevOtherPlayerPositions.Clear();

            for (int i = 0; i < ClientEngine.OtherPlayerPositions.Count; i++)
                PrevOtherPlayerPositions.Add(ClientEngine.OtherPlayerPositions[i]);
        }
    }
}
