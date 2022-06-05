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
    // TODO: Server closes when second player (even with diff username) joins
    public class ServerEngine
    {
        public static UdpServer Server = new UdpServer();

        public bool IsRunning;
        public bool IsPaused;

        public Dictionary <string, ServerPlayer> Players = new Dictionary<string, ServerPlayer>();

        public async void Initialize()
        {
            Debug.Announce("Server engine started!");

            var time = 0.0f;
            var deltaTime = 0.0f;

            var previousTimer = DateTime.Now;
            var currentTimer = DateTime.Now;

            IsRunning = true;

            // Networking
            var task = Task.Factory.StartNew(async () =>
            {
                while (IsRunning)
                {
                    try
                    {
                        await Task.Delay(Utils.TickRate);

                        var received = await Server.Receive();
                        var buffer = received.Message.Split("#");
    
                        // Receive from client
                        if (buffer[0] == "munitaClient777")
                        {
                            if (buffer[1] == "joining")
                            {
                                if (!Players.ContainsKey(buffer[2]))
                                {
                                    var player = new ServerPlayer();
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
                                    player.IsRunning = bool.Parse(buffer[3]);
                                    player.MoveDirection = Utils.UnpackVec2(buffer[4]);

                                    Utils.SendPlayerPosition(player.Position, received.Sender);

                                    var otherPlayerPositions = new List<Vector2>();

                                    for (int i = 1; i < Players.Count; i++)
                                    {
                                        var username = Players.ElementAt(i).Key;

                                        otherPlayerPositions.Add(Players[username].Position);
                                    }

                                    Utils.SendOtherPlayerPos(otherPlayerPositions.ToArray(), received.Sender);
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

            // Initialize
            var world = new World();
            world.Initialize(false);

            while (IsRunning)
            {
                currentTimer = DateTime.Now;
                
                // Update
                //world.Update();

                if (Players.Count > 0)
                {
                    for (int i = 0; i < Players.Count; i++)
                    {
                        var username = Players.ElementAt(i).Key;
                        Players[username].Update(deltaTime);
                    }

                    /*foreach (var username in Players.Keys.ToList())
                    {
                        Players[username].Update(deltaTime);
                    }*/
                }

                deltaTime = (currentTimer.Ticks - previousTimer.Ticks) / 10000000f;
                time += deltaTime;

                previousTimer = currentTimer;
            }
        }
    }
}
