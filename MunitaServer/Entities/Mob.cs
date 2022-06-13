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
        public static readonly List<Mob> MobsList = new List<Mob>();

        public int Health { get; private set; }

        public float MoveSpeed { get; protected set; } = 1f;
        public bool IsEnemy { get; protected set; } = true;
        public int MaxHealth { get; protected set; } = 100;
        public Vector2 MinMaxDamage { get; protected set; } = new Vector2(1f, 5f);
        public float AttackCooldown { get; protected set; } = 0.5f;

        public Vector2 Position = Vector2.Zero;

        protected bool canAttack = false;

        private Vector2 lastPosition = Vector2.Zero;
        private float attackCooldownTimer = 0.0f;

        public Mob()
        {
            MobsList.Add(this);
        }

        public virtual void Initialize()
        {

        }

        // Update this before your override!
        public virtual void Update(float time, float deltaTime)
        {
            lastPosition = Position;

            if (Engine.Players.Count > 0)
            {
                for (int i = 0; i < Engine.Players.Count; i++)
                {
                    var username = Engine.Players.ElementAt(i).Key;
                    var distance = Vector2.Distance(Position, Engine.Players[username].Position);

                    if (distance < 4f && IsEnemy) // Chase player
                    {
                        var moveDirection = GameMath.Vector2Normalized(Position - Engine.Players[username].Position);
                        Position += -moveDirection * MoveSpeed * deltaTime;
                    }
                    else
                        continue;

                    if (distance < 2f) // Collision bounds check
                    {
                        var colliding = Raylib.CheckCollisionCircles(Position, 0.4f,
                            Engine.Players[username].Position, 0.4f);

                        if (colliding)
                            Position = lastPosition;

                        // Damage players
                        if (colliding && canAttack && IsEnemy)
                        {
                            var rnd = NetRandom.Next((int)MinMaxDamage.X, (int)MinMaxDamage.Y);

                            Debug.Log($"{username} was attacked with {rnd} damage! Health: {Engine.Players[username].Health}");
                            Engine.Players[username].Damage(rnd);
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
