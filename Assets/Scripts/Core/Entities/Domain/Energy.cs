using AiAlgorithmsResearch.Core.Entities.Api;
using System;

namespace AiAlgorithmsResearch.Core.Entities.Domain
{
    internal class Energy : IEnergyView
    {
        public int Current { get; private set; }
        public int Max { get; }
        public int RegenerationPerTurn { get; }

        public Energy(int max, int regenerationPerTurn)
        {
            if (max < 0)
                throw new ArgumentOutOfRangeException(nameof(max));

            if (regenerationPerTurn < 0)
                throw new ArgumentOutOfRangeException(nameof(regenerationPerTurn));

            Max = max;
            Current = max;
            RegenerationPerTurn = regenerationPerTurn;
        }

        public bool CanSpend(int amount)
        {
            return amount >= 0 && Current >= amount;
        }

        public bool TrySpend(int amount)
        {
            if (!CanSpend(amount))
                return false;

            Current -= amount;
            return true;
        }

        public void Regenerate()
        {
            Current = Math.Min(Max, Current + RegenerationPerTurn);
        }
    }
}
