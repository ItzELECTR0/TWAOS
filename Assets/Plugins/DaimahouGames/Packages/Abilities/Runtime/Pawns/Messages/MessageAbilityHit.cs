using DaimahouGames.Runtime.Pawns;
using UnityEngine;

namespace DaimahouGames.Runtime.Abilities
{
    public struct MessageAbilityHit : IPawnMessage
    {
        public GameObject CasterGameObject { get; }

        public MessageAbilityHit(GameObject casterGameObject)
        {
            CasterGameObject = casterGameObject;
        }
    }
}