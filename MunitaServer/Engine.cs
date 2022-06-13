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
        public static UdpServer Server = new UdpServer();
        public static Dictionary<string, Player> Players = new Dictionary<string, Player>();

        public bool IsRunning;
        public bool IsPaused;

        public async void Initialize()
        {
            Debug.Announce("Server engine started!");

            var time = 0.0f;
            var deltaTime = 0.0f;

            var previousTimer = DateTime.Now;
            var currentTimer = DateTime.Now;

            IsRunning = true;

            // Initialize
            //GameMath.InitXorRNG();
            NetRandom.Initialize(777);

            var world = new World();
            world.Initialize(false);

            // TEMPORARY
            var testMob = new TestMob();
            testMob.Initialize();

            testMob.Spawn(new Vector2(17f, 14f));

            // Networking
            var task = Task.Factory.StartNew(async () =>
            {
                while (IsRunning)
                {
                    try
                    {
                        await Task.Delay(Constants.TickRate);

                        var received = await Server.Receive();
                        var buffer = received.Message.Split("#");

                        // Receive from client
                        if (buffer[0] == "munitaClient777")
                        {
                            if (buffer[1] == "joining")
                            {
                                if (!Players.ContainsKey(buffer[2]))
                                {
                                    var player = new Player();
                                    player.Initialize();

                                    player.EndPoint = received.Sender;

                                    Players.Add(buffer[2], player);

                                    Debug.Announce($"A new player named '{buffer[2]}' joined!");
                                }
                                else
                                {
                                    Debug.Announce($"'{buffer[2]}' rejoined!");
                                }

                                Server.Reply("joined", received.Sender);
                            }

                            if (buffer[1] == "PlayerUpdate")
                            {
                                var player = Players[buffer[2]];

                                if (player != null)
                                {
                                    // Current player
                                    player.IsRunning = bool.Parse(buffer[3]);
                                    player.MoveDirection = Utils.UnpackVec2(buffer[4]);

                                    Utils.SendPlayerUpdate(player.Health, player.Position, received.Sender);

                                    // Other players
                                    var otherPlayerPositions = new List<Vector2>();

                                    for (int i = 0; i < Players.Count; i++)
                                    {
                                        var username = Players.ElementAt(i).Key;

                                        if (username != buffer[2]) // Do not add current player
                                            otherPlayerPositions.Add(Players[username].Position);
                                    }

                                    Utils.SendOtherPlayerPos(otherPlayerPositions.ToArray(), received.Sender);

                                    // Mobs
                                    var mobPositions = new List<Vector2>();

                                    for (int i = 0; i < Mob.MobsList.Count; i++)
                                        mobPositions.Add(Mob.MobsList[i].Position);

                                    Utils.SendMobPos(mobPositions.ToArray(), received.Sender);
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

            while (IsRunning)
            {
                currentTimer = DateTime.Now;

                // Update
                for (int i = 0; i < Mob.MobsList.Count; i++)
                    Mob.MobsList[i].Update(time, deltaTime);

                if (Players.Count > 0)
                {
                    for (int i = 0; i < Players.Count; i++)
                    {
                        var username = Players.ElementAt(i).Key;
                        Players[username].Update(deltaTime);
                    }
                }

                deltaTime = (currentTimer.Ticks - previousTimer.Ticks) / 10000000f;
                time += deltaTime;

                previousTimer = currentTimer;
            }
        }

        public void SaveWorldData()
        {
            if (!Directory.Exists(GamePaths.ServerWorldPath))
                Directory.CreateDirectory(GamePaths.ServerWorldPath);

            using (var stream = File.Open($"{GamePaths.ServerWorldPath}\\Players.dat", FileMode.Create))
            {
                using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    writer.Write(Players.Count);

                    for (int i = 0; i < Players.Count; i++)
                    {
                        var playerPair = Players.ElementAt(i);

                        writer.Write(playerPair.Key); // Username
                        writer.Write(playerPair.Value.Health);
                        writer.Write(playerPair.Value.Position.X);
                        writer.Write(playerPair.Value.Position.Y);
                    }
                }
            }
        }

        public void LoadWorldData()
        {
            var fileName = $"{GamePaths.ServerWorldPath}\\Players.dat";

            using (var stream = File.Open(fileName, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                {
                    var playerCount = reader.ReadInt32();

                    for (int i = 0; i < playerCount; i++)
                    {
                        var player = new Player();
                        var username = reader.ReadString();

                        player.Health = reader.ReadInt32();
                        player.Position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    }
                }
            }
        }
    }
}
