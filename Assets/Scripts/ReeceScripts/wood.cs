using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class wood : resource
    {
        public override void Gather()
        {
            if (gatherTool == player.currentTool)
            {
                base.Gather();
                print("hitting wood");
            }
        }
    }
}
