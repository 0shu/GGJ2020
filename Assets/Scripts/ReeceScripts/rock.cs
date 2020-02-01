using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class rock : resource
    {
        static int rockResource;
        int gatheredRock;

        public override void Gather()
        {
            if(gatherTool == player.currentTool)
            {
                base.Gather();
                print("hitting rock");
            }
        }
    }
}

