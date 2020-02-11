using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            
            // Seed corner with lowest possible value.
            m_corners = new int[m_tileGridSize.x + 1, m_tileGridSize.y + 1];

            m_corners[0, 0] = m_tileHeightLimits.x;
            // Generate first row.
            for (int x = 1; x < m_tileGridSize.x + 1; x++)
            {
                int min = m_corners[x - 1, 0] - 2;
                int max = m_corners[x - 1, 0] + 2;
                m_corners[x, 0] = GenerateHeight(x, 0, min, max);
            }

            for (int y = 1; y < m_tileGridSize.y + 1; y++)
            {
                // Generate first of row.
                int min = Mathf.Max(m_corners[0, y - 1] - 2, m_corners[1, y - 1] - 2);
                int max = Mathf.Min(m_corners[0, y - 1] + 2, m_corners[1, y - 1] + 2);
                m_corners[0, y] = GenerateHeight(0, y, min, max);

                // Generate Row
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

                // Generate last of row
                int endX = m_tileGridSize.x;
                min = Mathf.Max(
                    m_corners[endX - 1, y - 1] - 2,
                    m_corners[endX, y - 1] - 2,
                    m_corners[endX - 1, y] - 2
                    );
                max = Mathf.Min(
                    m_corners[endX - 1, y - 1] + 2,
                    m_corners[endX, y - 1] + 2,
                    m_corners[endX - 1, y] + 2
                    );
                m_corners[0, y] = GenerateHeight(endX, y, min, max);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        int GenerateHeight(int x, int y, int min, int max)
        {
            int trueMin = Mathf.Max(min, m_tileHeightLimits.x);

            // Prevents lock situation in which even the steepest tile gradients cannot get back to sea level in time.
            int slopeX = ((2 * x) < m_tileGridSize.x) ? x : m_tileGridSize.x - x;
            int slopeY = ((2 * y) < m_tileGridSize.y) ? y : m_tileGridSize.y - y;
            int trueMax = Mathf.Min(max, m_tileHeightLimits.y, (slopeX * 2), (slopeY * 2));
            
            if (trueMin < trueMax) {
                Debug.LogWarning(string.Format("GenerateHeight(x: {0}, y: {1}, min: {2}), max: {3}) failed with trueMin: {4} and trueMax: {5}.", x, y, min, max, trueMin, trueMax));
                return 0;
            }


            //WeightedDistributions.RandomWeightedRange()

            int output = 0;

            return output;
        }
    }
}