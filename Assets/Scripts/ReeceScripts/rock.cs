using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class rock : resource
    {
        public override void Gather()
        {
            if(gatherTool == player.currentTool)
            {
                base.Gather();
                base.Material(1);
                print("hitting rock");
            }
        }
    }
}

