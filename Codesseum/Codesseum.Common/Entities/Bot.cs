using System;
using System.Linq;
using Codesseum.Common.Types;

namespace Codesseum.Common.Entities
{
    public abstract class Bot
    {
        protected Bot()
        {
            Position = new Coordinate();
            IsDead = false;
            Ammunition = 3;

            Health = 5;
            Power = 5;
            Defense = 5;
            Speed = 5;
            Range = 5;
        }

        // Properties

        public Guid Id { get; internal set; }

        public string TeamName { get; set; }

        public int Health { get; internal set; }
        public int Power { get; internal set; }
        public int Defense { get; internal set; }
        public int Speed { get; internal set; }
        public int Range { get; internal set; }

        public Coordinate Position { get; set; }
        public int Ammunition { get; set; }

        internal bool IsDead { get; set; }

        // Methods

        /// <summary>
        /// Set bot attributes at spawn
        /// </summary>
        /// <returns>Array of attribute values in order: Health, Power, Defense, Speed</returns>
        public abstract int[] GetAttributes();
        public abstract BotAction NextAction();

        internal void SetAttributes(int[] values)
        {
            if (values.Length != 5 || values.Sum() != 25 || values.Any(v => v <= 0))
            {
                throw new ArgumentException("Bot attributes are invalid");
            }

            AttributeValues = values;

            Health = values[0];
            Power = values[1];
            Defense = values[2];
            Speed = values[3];
            Range = values[4];
        }

        internal int[] AttributeValues;
    }
}
