using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Raylib_cs;

namespace Munita
{
    public class TestMob : Mob
    {
        public override void Initialize()
        {
            base.Initialize();

            // Mob settings
            MinMaxDamage = new Vector2(10f, 20f);
        }

        public override void Update(float time, float deltaTime)
        {
            base.Update(time, deltaTime);
        }
    }
}
