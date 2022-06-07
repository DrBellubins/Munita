using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

using Raylib_cs;

namespace Munita
{
    public class Mob
    {
        public int Health { get; private set; }

        public int MaxHealth { get; protected set; } = 100;
        public Vector2 MinMaxDamage { get; protected set; } = new Vector2(1f, 5f);
        public float AttackCooldown { get; protected set; } = 0.5f;

        public Vector2 Position = Vector2.Zero;

        protected bool canAttack = false;

        private float attackCooldownTimer = 0.0f;

        public virtual void Initialize()
        {

        }

        // Update this before your override!
        public virtual void Update(float time, float deltaTime)
        {
            if (Engine.Players.Count > 0)
            {
                for (int i = 0; i < Engine.Players.Count; i++)
                {
                    var username = Engine.Players.ElementAt(i).Key;

                    var colliding = Raylib.CheckCollisionCircles(Position, 0.4f,
                            Engine.Players[username].Position, 0.4f);

                    if (colliding)
                    {
                        var rnd = GameMath.GetXorFloat(MinMaxDamage.X, MinMaxDamage.Y);

                        if (canAttack)
                        {
                            Debug.Log($"{username} was attacked with {(int)rnd} damage! Health: {Engine.Players[username].Health}");
                            Engine.Players[username].Damage((int)rnd);
                            canAttack = false;
                        }
                    }
                    else
                        continue;
                }
            }

            if (time > attackCooldownTimer)
            {
                attackCooldownTimer = time + AttackCooldown;

                // execute code here
                canAttack = true;
            }
        }

        public void Damage(int amount)
        {
            Health = GameMath.Clamp(Health - amount, 0, MaxHealth);
        }

        public void Heal(int amount)
        {
            Health = GameMath.Clamp(Health + amount, 0, MaxHealth);
        }

        public void Kill()
        {
            Health = -999;
        }

        public void Spawn(Vector2 position)
        {
            Health = MaxHealth;
            Position = position;
            canAttack = true;
        }
    }
}
