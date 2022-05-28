using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Raylib_cs;
using TiledCS;

namespace Munita
{
    // TODO: Add collision
    public class World
    {
        public static List<Rectangle> WallCols = new List<Rectangle>(); // Wall colliders

        private TiledMap map = new TiledMap();
        private TiledTileset tileSet = new TiledTileset();

        private TiledLayer groundLayer = new TiledLayer();
        private TiledLayer wallLayer = new TiledLayer();

        private List<Texture2D> tileTextures = new List<Texture2D>();

        public void Initialize()
        {
            map = new TiledMap($"{GamePaths.Maps}\\Test.tmx");
            tileSet = new TiledTileset($"{GamePaths.Tilesets}\\Munita.tsx");
            groundLayer = map.Layers.First(l => l.name == "Ground");
            wallLayer = map.Layers.First(l => l.name == "Wall");

            // Load tileset textures into raylib
            for (int i = 0; i < tileSet.TileCount; i++) // could need to be 1 less
            {
                var textureName = tileSet.Tiles[i].image.source.Remove(0, 12);
                var tileName = textureName.Remove(textureName.Length - 4, 4);

                tileTextures.Add(Raylib.LoadTexture($"Assets/Textures/{textureName}"));
            }

            for (var x = 0; x < groundLayer.width; x++)
            {
                for (var y = 0; y < groundLayer.height; y++)
                {
                    var index = (y * groundLayer.height) + x;
                    var gid = wallLayer.data[index]; // The tileset tile index
                        
                    // Gid 0 is used to tell there is no tile set
                    if (gid == 0)
                        continue;
                    
                    // Add rectangle to collider list
                    var wallCol = new Rectangle(x, y, 1f, 1f);
                    WallCols.Add(wallCol);
                }
            }
        }

        public void Update()
        {
            
        }

        public void Draw()
        {
            for (int layer = 0; layer < 2; layer++)
            {
                for (var x = 0; x < groundLayer.width; x++)
                {
                    for (var y = 0; y < groundLayer.height; y++)
                    {
                        var index = (y * groundLayer.height) + x;
                        var gid = 0;

                        if (layer == 0)
                            gid = groundLayer.data[index]; // The tileset tile index
                        else if (layer == 1)
                            gid = wallLayer.data[index]; // The tileset tile index

                        // Gid 0 is used to tell there is no tile set
                        if (gid == 0)
                            continue;

                        // Render texture with raylib
                        var sourceRect = new Rectangle(0f, 0f, 32f, 32f);
                        var destRect = new Rectangle(x, y, 1f, 1f);

                        Raylib.DrawTexturePro(tileTextures[GameMath.Clamp(gid - 1, 0, int.MaxValue)],
                            sourceRect, destRect, Vector2.Zero, 0f, Color.WHITE);
                    }
                }
            }
        }
    }
}