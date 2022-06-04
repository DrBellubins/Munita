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
    public class ServerEngine
    {
        public static UdpServer Server = new UdpServer();

        public bool IsRunning;
        public bool IsPaused;

        public List<ServerPlayer> Players = new List<ServerPlayer>();

        public async void Initialize()
        {
            Debug.Announce("Server engine started!");

            var time = 0.0f;
            var deltaTime = 0.0f;

            var previousTimer = DateTime.Now;
            var currentTimer = DateTime.Now;

            IsRunning = true;

            // Networking
            // TODO: Rejoining crashes sockets...
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
                                Debug.Announce($"A player named '{buffer[2]}' joined!");
    
                                var player = new ServerPlayer();
                                player.Initialize();
    
                                player.EndPoint = received.Sender;
                                player.Username = buffer[2];
    
                                Players.Add(player);
    
                                Server.Reply("joined", received.Sender);
                            }

                            if (buffer[1] == "PlayerUpdate")
                            {
                                var player = GetPlayerByUserName(buffer[2]);

                                if (player != null)
                                {
                                    player.IsRunning = bool.Parse(buffer[3]);
                                    player.MoveDirection = Utils.UnpackVec2(buffer[4]);

                                    Utils.s_SendPlayerUpdate(player.Position, player.EndPoint);
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
                
                for (int i = 0; i < Players.Count; i++)
                {
                    var player = Players[i];

                    player.Update(deltaTime);
                }

                deltaTime = (currentTimer.Ticks - previousTimer.Ticks) / 10000000f;
                time += deltaTime;

                previousTimer = currentTimer;
            }
        }

        public ServerPlayer? GetPlayerByUserName(string username)
        {
            return Players.Where(x => x.Username == username).FirstOrDefault();
        }
    }
}
