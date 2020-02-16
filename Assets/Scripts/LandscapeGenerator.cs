using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace GGJ2020
{
    public class LandscapeGenerator : MonoBehaviour
    {
        public enum TileTags
        {
            Rural = 0,
            Mixed,
            Urban,
            EasyNav,
            HardNav
        }
        
        [System.Serializable]
        public struct TilePrefab
        {
            [Tooltip("Equal to the lowest possible value of sum of weirdly-weighted corner heights: 11(C(n)) + 5(C(n+1)) + 3(C(n+2)) + 1(C(n+3)), where each C is a corner going anticlockwise from start. Heights are in increments of 8 meters, starting from lowest corner of tile; C(n) should presumably always be a '0' corner.")]
            public uint m_cornerHash;
            public GameObject m_tilePrefab;
            public TileTags[] m_tags;
        }

        [SerializeField]
        TilePrefab[] m_prefabs;

        [Tooltip("Width and length.")]
        [SerializeField]
        Vector2Int m_tileGridSize;
        [Tooltip("Min and max, in units of 8 meters, where 0 is slightly below sea level.")]
        [SerializeField]
        Vector2Int m_tileHeightLimits;

        int[,] m_corners;

        // Start is called before the first frame update
        void Start()
        {
            // Ensure that it's min, then max.
            if (m_tileHeightLimits.x > m_tileHeightLimits.y) { m_tileHeightLimits = new Vector2Int(m_tileHeightLimits.y, m_tileHeightLimits.x); }

            float trigSeed = Random.Range(0, 2.0f * Mathf.PI);

            // Seed corner with lowest possible value.
            m_corners = new int[m_tileGridSize.x + 1, m_tileGridSize.y + 1];

            int min=0, max=0, i=0, j=0, layer=1, leg=0, offset= (m_tileGridSize.x / 2), x=offset, y=offset;
            
            // Absolute centre
            m_corners[x, y] = m_tileHeightLimits.y - 2;

            // One to the right.
            x++; i++; leg++;
            m_corners[x, y] = GenerateHeight(x, y, m_tileHeightLimits.y - 4, m_tileHeightLimits.y, trigSeed);

            bool stillGoing = true;
            while (stillGoing)
            {
                switch (leg)
                {
                    case 0:
                        // On bottom, going right.
                        i++;
                        x++;
                        if (x > m_tileGridSize.x) { stillGoing = false; }
                        else
                        {
                            if (i < layer)
                            {
                                // Straight
                                min = Mathf.Max(
                                    m_corners[x - 1, y + 1] - 2,
                                    m_corners[x, y + 1] - 2,
                                    m_corners[x - 1, y] - 2
                                );
                                max = Mathf.Min(
                                    m_corners[x - 1, y + 1] + 2,
                                    m_corners[x, y + 1] + 2,
                                    m_corners[x - 1, y] + 2
                                );
                            }
                            else
                            {
                                // Corner
                                min = Mathf.Max(
                                    m_corners[x - 1, y + 1] - 2,
                                    m_corners[x - 1, y] - 2
                                );
                                max = Mathf.Min(
                                    m_corners[x - 1, y + 1] + 2,
                                    m_corners[x - 1, y] + 2
                                );
                                ++leg;
                            }
                        }
                        break;
                    case 1:
                        // On right, going up.
                        j++;
                        y++;
                        if (y > m_tileGridSize.y) { stillGoing = false; }
                        else
                        {
                            if (j < layer)
                            {
                                // Straight
                                min = Mathf.Max(
                                    m_corners[x - 1, y - 1] - 2,
                                    m_corners[x, y - 1] - 2,
                                    m_corners[x - 1, y] - 2
                                );
                                max = Mathf.Min(
                                    m_corners[x - 1, y - 1] + 2,
                                    m_corners[x, y - 1] + 2,
                                    m_corners[x - 1, y] + 2
                                );
                            }
                            else
                            {
                                // Corner
                                min = Mathf.Max(
                                    m_corners[x - 1, y - 1] - 2,
                                    m_corners[x, y - 1] - 2
                                );
                                max = Mathf.Min(
                                    m_corners[x - 1, y - 1] + 2,
                                    m_corners[x, y - 1] + 2
                                );
                                leg++;
                            }
                        }
                        break;
                    case 2:
                        // On top, going left.
                        i--;
                        x--;
                        if (x < 0) { stillGoing = false; }
                        else {
                            if (-i < layer)
                            {
                                // Straight
                                min = Mathf.Max(
                                    m_corners[x + 1, y - 1] - 2,
                                    m_corners[x, y - 1] - 2,
                                    m_corners[x + 1, y] - 2
                                );
                                max = Mathf.Min(
                                    m_corners[x + 1, y - 1] + 2,
                                    m_corners[x, y - 1] + 2,
                                    m_corners[x + 1, y] + 2
                                );
                            }
                            else
                            {
                                // Corner
                                min = Mathf.Max(
                                    m_corners[x + 1, y - 1] - 2,
                                    m_corners[x + 1, y] - 2
                                );
                                max = Mathf.Min(
                                    m_corners[x + 1, y - 1] + 2,
                                    m_corners[x + 1, y] + 2
                                );
                                leg++;
                            }
                        }
                        break;
                    case 3:
                        // On left, going down.
                        j--;
                        y--;
                        if (y < 0) { stillGoing = false; }
                        else
                        {
                            if (-j < layer)
                            {
                                // Straight
                                min = Mathf.Max(
                                    m_corners[x + 1, y + 1] - 2,
                                    m_corners[x, y + 1] - 2,
                                    m_corners[x + 1, y] - 2
                                );
                                max = Mathf.Min(
                                    m_corners[x + 1, y + 1] + 2,
                                    m_corners[x, y + 1] + 2,
                                    m_corners[x + 1, y] + 2
                                );
                            }
                            else
                            {
                                // Corner
                                min = Mathf.Max(
                                    m_corners[x + 1, y + 1] - 2,
                                    m_corners[x, y + 1] - 2
                                );
                                max = Mathf.Min(
                                    m_corners[x + 1, y + 1] + 2,
                                    m_corners[x, y + 1] + 2
                                );
                                leg = 0;
                                ++layer;
                            }
                        }
                        break;
                }

                if (stillGoing)
                {
                    //Debug.Log(string.Format("x: {0}; y: {1}; i: {2}; j: {3}; leg: {4}; layer: {5}", x, y, i, j, leg, layer));
                    m_corners[x, y] = GenerateHeight(x, y, min, max, trigSeed);
                }
            }

            /*m_corners[0, 0] = m_tileHeightLimits.x;
            // Generate first row.
            for (int x = 1; x < m_tileGridSize.x + 1; x++)
            {
                int min = m_corners[x - 1, 0] - 2;
                int max = m_corners[x - 1, 0] + 2;
                m_corners[x, 0] = GenerateHeight(x, 0, min, max);
            }

            for (int y = 1; y < m_tileGridSize.y + 1; y++)
            {
                // We want the grid to be filled in a zig-zag pattern for a more even slope distribution.
                bool outNotBack = (y % 2 == 0);

                int min, max;

                // Generate first of row.
                if (outNotBack)
                {
                    min = Mathf.Max(m_corners[0, y - 1] - 2, m_corners[1, y - 1] - 2);
                    max = Mathf.Min(m_corners[0, y - 1] + 2, m_corners[1, y - 1] + 2);
                    m_corners[0, y] = GenerateHeight(0, y, min, max);
                }
                else
                {
                    min = Mathf.Max(m_corners[m_tileGridSize.x, y - 1] - 2, m_corners[m_tileGridSize.x - 1, y - 1] - 2);
                    max = Mathf.Min(m_corners[m_tileGridSize.x, y - 1] + 2, m_corners[m_tileGridSize.x - 1, y - 1] + 2);
                    m_corners[0, y] = GenerateHeight(m_tileGridSize.x, y, min, max);
                }

                // Generate Row
                if (outNotBack)
                {
                    for (int x = 1; x < m_tileGridSize.x; x++)
                    {
                        min = Mathf.Max(
                            m_corners[x - 1, y - 1] - 2,
                            m_corners[x, y - 1] - 2,
                            m_corners[x + 1, y - 1] - 2,
                            m_corners[x - 1, y] - 2
                            );
                        max = Mathf.Min(
                            m_corners[x - 1, y - 1] + 2,
                            m_corners[x, y - 1] + 2,
                            m_corners[x + 1, y - 1] + 2,
                            m_corners[x - 1, y] + 2
                            );
                        m_corners[x, y] = GenerateHeight(x, y, min, max);
                    }
                }
                else
                {
                    for (int x = m_tileGridSize.x - 1; x > 0; x--)
                    {
                        min = Mathf.Max(
                            m_corners[x + 1, y - 1] - 2,
                            m_corners[x, y - 1] - 2,
                            m_corners[x - 1, y - 1] - 2,
                            m_corners[x + 1, y] - 2
                            );
                        max = Mathf.Min(
                            m_corners[x + 1, y - 1] + 2,
                            m_corners[x, y - 1] + 2,
                            m_corners[x - 1, y - 1] + 2,
                            m_corners[x + 1, y] + 2
                            );
                        m_corners[x, y] = GenerateHeight(x, y, min, max);
                    }
                }

                // Generate last of row
                if (outNotBack)
                {
                    min = Mathf.Max(
                        m_corners[m_tileGridSize.x - 1, y - 1] - 2,
                        m_corners[m_tileGridSize.x, y - 1] - 2,
                        m_corners[m_tileGridSize.x - 1, y] - 2
                        );
                    max = Mathf.Min(
                        m_corners[m_tileGridSize.x - 1, y - 1] + 2,
                        m_corners[m_tileGridSize.x, y - 1] + 2,
                        m_corners[m_tileGridSize.x - 1, y] + 2
                        );
                    m_corners[0, y] = GenerateHeight(m_tileGridSize.x, y, min, max);
                }
                else
                {
                    min = Mathf.Max(
                        m_corners[1, y - 1] - 2,
                        m_corners[0, y - 1] - 2,
                        m_corners[1, y] - 2
                        );
                    max = Mathf.Min(
                        m_corners[1, y - 1] + 2,
                        m_corners[0, y - 1] + 2,
                        m_corners[1, y] + 2
                        );
                    m_corners[0, y] = GenerateHeight(0, y, min, max);
                }
            }*/
            string path = "CornerData.txt";
            FileStream fcreate = File.Open(path, FileMode.Create);
            StreamWriter writer = new StreamWriter(fcreate);

            for (int y2 = 0; y2 < m_tileGridSize.y + 1; y2++)
            {
                string lineOutput = "";
                List<int> l = new List<int>();
                for (int x2 = 0; x2 < m_tileGridSize.x + 1; x2++) { l.Add(m_corners[x2,y2]); }

                lineOutput += string.Join(", ",
                    l
                    .ConvertAll(a => a.ToString())
                    .ToArray());
                //cornersOutput += "\r\n";
                writer.WriteLine(lineOutput);
            }

            writer.Close();
        }

        // Update is called once per frame
        void Update()
        {

        }

        int GenerateHeight(int x, int y, int min, int max, float trigSeed)
        {
            int trueMin = Mathf.Max(min, m_tileHeightLimits.x);

            // Prevents lock situation in which even the steepest tile gradients cannot get back to sea level in time.
            int slopeX = ((2 * x) < m_tileGridSize.x) ? x : m_tileGridSize.x - x;
            int slopeY = ((2 * y) < m_tileGridSize.y) ? y : m_tileGridSize.y - y;
            int trueMax = Mathf.Min(max, m_tileHeightLimits.y, (slopeX * 2), (slopeY * 2));
            
            if (trueMin > trueMax) {
                Debug.LogWarning(string.Format("GenerateHeight(x: {0}, y: {1}, min: {2}), max: {3}) failed with trueMin: {4} and trueMax: {5}.", x, y, min, max, trueMin, trueMax));
                return 0;
            }

            if (trueMin == trueMax) { return trueMin; }

            int choices = trueMax - trueMin + 1;

            // Weird trig shit. Just... Don't worry about it.
            float xOffsetModifier = 1.5f * Mathf.Cos(3.0f + (0.05f * trigSeed) + (0.14f * (float)y));
            float xOffset = 5.0f + (0.7f * trigSeed * trigSeed) + (0.25f * (float)x) + xOffsetModifier;
            float yOffsetModifier = 1.5f * Mathf.Cos(13.0f + (0.3f * trigSeed) + (0.14f * (float)x));
            float yOffset = 3.0f + (0.13f * trigSeed * trigSeed) + (0.25f * (float)y) + yOffsetModifier;

            // Delete following:
            //float choiceOffset = ((float)i / ((float)choices - 1.0f)) * Mathf.PI;
            /*float xComp = 0.5f + (0.5f * Mathf.Sin(xOffset + 0));
            xComp = Mathf.Sqrt(xComp);
            float yComp = 0.5f + (0.5f * Mathf.Sin(yOffset + 0));
            yComp = Mathf.Sqrt(yComp);
            return Mathf.FloorToInt(5.0f * (xComp * yComp));*/

            List<float> weights = new List<float>();

            for (int i = 0; i < choices; i++)
            {
                //float weight = 1.0f;
                float weight = 0.5f;

                // Apply the weird trig shit.
                float choiceOffset = ((float)i / ((float)choices - 1.0f)) * Mathf.PI;
                float xComp = 0.5f + (0.5f * Mathf.Sin(xOffset + choiceOffset));
                xComp = Mathf.Sqrt(xComp);
                float yComp = 0.5f + (0.5f * Mathf.Sin(yOffset + choiceOffset));
                yComp = Mathf.Sqrt(yComp);
                weight += 1.0f * (xComp * yComp);

                // Less likely to pick upper extreme.
                if (i == choices - 1) { weight -= 0.75f; }
                else if (i == 0) { weight -= 0.75f; }
                else if (i == Mathf.FloorToInt((float)choices / 2)) { weight += 0.25f; }

                // Prefer to slope downwards more often, especially around high bits, as we want plenty of lowlands.
                //if (i < ((float)choices / 2) && i + trueMin > Mathf.Min(slopeX, slopeY )) { weight += 1.0f; }
                float ideal = 0.5f * Mathf.Min(slopeX, slopeY);
                if (i == 0 && trueMin + i > ideal) { weight += 1.5f; }
                else if (i > ((float)choices / 2) && trueMin + i < ideal - 4.0f) { weight += 0.25f; }

                // We want nice uneven shorelines, so add weight for extra submerged points.
                if (trueMin + i < 5 && i < ((float)choices / 2)) { weight += 1.0f; }

                weight = Mathf.Max(weight, 0.1f);

                weights.Add(weight);
            }

            string weightsStr = "";
            foreach(var w in weights) { weightsStr += w + ", "; }

            return Mathf.FloorToInt(WeightedDistributions.RandomValueFromHistogram(new Vector2(trueMin, trueMax+1), weights));
        }
    }
}