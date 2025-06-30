using System;
using UnityEngine;

namespace PowerUps
{
    [CreateAssetMenu(menuName = "PowerUps/ModifyArc")]
    public class ModifyArc : PowerUp
    {
        public float modifyValue = 50f;
        public override void Apply(PlayerController player)
        {
            player.ownArc.AddExtraAngleDegrees(modifyValue,0.5f);
        }

        public override void Remove(PlayerController player)
        {
            Debug.Log("Remove");
            player.ownArc.RemoveExtraAngleDegrees(modifyValue,0.5f);
        }

        public override bool CanApply(PlayerController player)
        {
            if (player.ownArc.maxAngleDegrees < modifyValue + player.ownArc.angleDegrees || 
                player.ownArc.minAngleDegrees > modifyValue + player.ownArc.angleDegrees )
            {
                return false;
            }

            return true;
        }

        public override bool CanRemove(PlayerController player)
        {
            if (player.ownArc.maxAngleDegrees < player.ownArc.angleDegrees - modifyValue || 
                player.ownArc.minAngleDegrees > player.ownArc.angleDegrees - modifyValue )
            {
                return false;
            }

            return true;
        }

        // public override bool IsValid(PlayerController player)
        // {
        //     if (player.ownArc.maxAngleDegrees < modifyValue + player.ownArc.angleDegrees ||
        //         player.ownArc.minAngleDegrees > modifyValue + player.ownArc.angleDegrees )
        //     {
        //         return false;
        //     }
        //     return true;
        //
        // }
    }
}