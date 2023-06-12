using System;
using System.Collections.Generic;
using Alpine.Renderables.Models.Skeleton.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alpine.Renderables.Models.Skeleton
{
    internal class Lod
    {
        private LodData data;
        private ModelRenderable model;
        private bool enabled;
        private LodLevelData activeLevel;
        private float activeMin, activeMax, distanceSqr;

        public Lod(LodData arg1, ModelRenderable arg2)
        {
            data = arg1;
            model = arg2;
            enabled = false;
            activeLevel = null;
            distanceSqr = 0;
        }

        public void Enable(bool arg1)
        {
            if (enabled == arg1)
            {
                return;
            }
            if (arg1)
            {
                foreach (Mesh loc2 in model.Meshes)
                {
                    if (loc2.data.hasLOD)
                    {
                        model.SetMesh(loc2.name, false, false);
                    }
                }
            }
            else
            {
                distanceSqr = 0;
            }
            enabled = arg1;
        }

        public void SetDistanceSqr(float arg1)
        {
            LodLevelData loc6;
            List<string> loc7, loc8 = new List<string>();
            int loc10;
            if (!enabled)
            {
                return;
            }
            if (distanceSqr == arg1)
            {
                return;
            }
            float loc2 = distanceSqr;
            distanceSqr = arg1;
            if ((activeMin < distanceSqr) && (distanceSqr < activeMax))
            {
                return;
            }
            LodData loc3 = data;
            List<LodLevelData> loc4 = loc3.levels;
            int loc5 = loc4.Count;
            LodLevelData loc11 = null;
            loc10 = 0;
            while(loc10 < loc5)
            {
                loc6 = loc4[loc10];
                if (loc6.thresholdSqr > loc2)
                {
                    loc11 = loc6;
                    break;
                }
                loc10++;
            }
            if ((loc11 == null) || (activeLevel == loc11))
            {
                return;
            }
            if (activeLevel != null)
            {
                loc8 = activeLevel.targets;
            }
            loc7 = loc11.targets;
            foreach (string loc9 in loc8)
            {
                if (loc7.IndexOf(loc9) == -1)
                {
                    model.SetMesh(loc9, false, false);
                }
            }
            foreach (string loc9 in loc7)
            {
                model.SetMesh(loc9, true, false);
            }
            activeLevel = loc11;
            activeMax = loc11.thresholdSqr;
            activeMin = (loc10 == 0) ? 0 : loc4[(loc10 - 1)].thresholdSqr;
        }
    }
}
