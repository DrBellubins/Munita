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
    public class Engine
    {
        public const int FPS = 60;
        public const float FrameTimestep = 1.0f / (float)FPS;

        public const int ScreenWidth = 1600;
        public const int ScreenHeight = 900;

        public static Font MainFont;

        // Network stuff
        public static List<Vector2> OtherPlayerPositions = new List<Vector2>();

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
            
            var player = new Player();
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
                        await Task.Delay(Constants.TickRate);

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
                                Utils.SendPlayerUpdate(false, Vector2.Zero, Username);

                                joined = true;
                            }

                            if (buffer[1] == "PlayerUpdate")
                            {
                                player.ServerHealth = int.Parse(buffer[2]);
                                player.ServerPosition = Utils.UnpackVec2(buffer[3]);
                                Utils.SendPlayerUpdate(player.IsRunning, player.MoveDirection, Username);
                            }

                            if (buffer[1] == "OtherPlayerUpdate")
                            {
                                var playerCount = int.Parse(buffer[2]);
                                var positionsStr = buffer[3].Split("^");

                                //OtherPlayerPositions.Clear();

                                for (int i = 0; i < playerCount; i++)
                                {
                                    if (OtherPlayerPositions.Count < playerCount)
                                        OtherPlayerPositions.Add(Utils.UnpackVec2(positionsStr[i]));
                                    else
                                        OtherPlayerPositions[i] = Utils.UnpackVec2(positionsStr[i]);
                                }
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

                // TEST ENEMY
                Raylib.DrawCircleV(new Vector2(14f, 14f), 0.4f, Color.YELLOW);

                for (int i = 0; i < OtherPlayerPositions.Count; i++)
                {
                    Raylib.DrawCircleV(OtherPlayerPositions[i], 0.4f, Color.GREEN);
                }

                player.Draw(deltaTime);

                Raylib.EndMode2D();

                player.UIDraw();

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
