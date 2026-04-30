using UnityEngine;

namespace Economy
{
    public class CoinCounterTarget
    {
        public RectTransform Target { get; private set; }

        public void Set(RectTransform target)
        {
            Target = target;
        }
    }
}

