using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Net;

using Raylib_cs;

namespace Munita
{
    public class ClientEngine
    {
        public const int FPS = 60;
        public const float FrameTimestep = 1.0f / (float)FPS;

        public const int ScreenWidth = 1600;
        public const int ScreenHeight = 900;

        public static Font MainFont;
        public static List<Vector2> PlayerPositions = new List<Vector2>();

        public static UdpClient Client = UdpClient.ConnectTo("127.0.0.1", 7777);

        public bool IsRunning;
        public bool IsPaused;

        // Client stuff
        public string Username = "";
        public string ServerIP = "";

        public async void Initialize()
        {
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "Munita");
            Raylib.SetExitKey(KeyboardKey.KEY_Q);
            Raylib.SetTargetFPS(FPS);

            MainFont = Raylib.LoadFontEx("Assets/Font/VarelaRound-Regular.ttf", 64, null, 250);

            Debug.Announce("Client engine started!");

            var time = 0.0f;
            var deltaTime = 0.0f;

            var previousTimer = DateTime.Now;
            var currentTimer = DateTime.Now;     

            // Initialize
            IsRunning = true;
            LoadConfig();

            var world = new World();
            world.Initialize(true);
            
            var player = new ClientPlayer();
            player.Initialize();

            Debug.Initialize();

            // Networking
            var joined = false;

            var task = Task.Factory.StartNew(async () =>
            {
                while (IsRunning)
                {
                    try
                    {
                        await Task.Delay(1000 / 20);

                        if (!joined)
                            Client.Reply("joining", Username);

                        var received = await Client.Receive();
                        var buffer = received.Message.Split("#");

                        // Receive from server
                        if (buffer[0] == "munitaClient777")
                        {
                            if (buffer[1] == "joined")
                            {
                                Debug.Announce("We joined!");

                                // Start player updates
                                Utils.c_SendPlayerUpdate(false, Vector2.Zero, Username);

                                joined = true;
                            }

                            if (buffer[1] == "PlayerUpdate")
                            {
                                player.NetworkPosition = Utils.UnpackVec2(buffer[2]);
                                Utils.c_SendPlayerUpdate(player.IsRunning, player.MoveDirection, Username);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Error(ex.ToString());
                    }
                }
            });

            if (task.Exception != null)
            {
                Debug.Error(task.Exception.ToString());
            }

            while (IsRunning)
            {
                currentTimer = DateTime.Now;
                
                if (Raylib.WindowShouldClose())
                    IsRunning = false;
                
                // Update
                world.Update();
                player.Update(deltaTime);

                // Draw
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);

                Raylib.BeginMode2D(player.Camera);

                world.Draw();

                for (int i = 0; i < PlayerPositions.Count; i++)
                {
                    Raylib.DrawCircleV(PlayerPositions[i], 0.4f, Color.GREEN);
                }

                player.Draw();

                Raylib.EndMode2D();

                Debug.Draw(time, deltaTime, player.Position);

                Raylib.EndDrawing();
                
                if (IsPaused)
                    deltaTime = 0.0f;
                else
                    deltaTime = FrameTimestep;
                    //deltaTime = (currentTimer.Ticks - previousTimer.Ticks) / 10000000f;

                time += deltaTime;

                previousTimer = currentTimer;
            }

            Raylib.CloseWindow();
        }

        public void LoadConfig()
        {
            var configPath = $"{Environment.CurrentDirectory}\\client.conf";

            if (File.Exists(configPath))
            {
                Debug.Warning($"Client.conf already exists, loading.");

                var configLines = File.ReadAllLines(configPath);

                var usernameLineStr = configLines[0];
                var usernameLineParseStr = usernameLineStr.Split("Username: ", 2, StringSplitOptions.RemoveEmptyEntries);

                Username = usernameLineParseStr[0];

                var ipLineStr = configLines[1];
                var ipLineParseStr = ipLineStr.Split("IP: ", 2, StringSplitOptions.RemoveEmptyEntries);

                ServerIP = ipLineParseStr[0];
            }
            else
            {
                using (var fs = new FileStream(configPath, FileMode.CreateNew))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine("Username: Player");
                        sw.WriteLine("IP: 127.0.0.1");
                    }
                }
            }
        }
    }
}
