using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2020
{
    public class WeightedDistributions
    {
        public struct HistogramSection
        {
            public float m_min;
            public float m_weight;
            public float m_max;

            public HistogramSection(float min, float weight, float max)
            {
                m_min = min;
                m_weight = weight;
                m_max = max;
            }
        }
        
        // Returns a random value with a weighting derived from histogram generated from points given. Points come in pairs, with a start-of-segment-domain and weight-of-segement, the segment domain stretching to immediately succeeding point. Point-count should be odd, or there's no close to final segment and it will be discarded.
        static public float RandomWeightedRange(params float[] values)
        {
            if (values.Length == 0) {
                Debug.LogError("Misuse of WeightedDistributions.RandomWeightedRange(), no valid input values.");
                return 0.0f;
            }
            if (values.Length < 3) {
                Debug.LogError("Misuse of WeightedDistributions.RandomWeightedRange(), fewer than three valid input values.");
                return values[0];
            }
            if (values.Length % 2 != 1) { Debug.LogError("Misuse of WeightedDistributions.RandomWeightedRange(), even number of input values."); }

            float totalWeight = 0.0f;
            for (int i = 0; i + 2 < values.Length; i += 2)
            {
                totalWeight += values[i + 1];
            }
            if (totalWeight <= 0.0f)
            {
                Debug.LogError("Misuse of WeightedDistributions.RandomWeightedRange(), total weight is equal to or less than zero.");
                return 0.0f;
            }

            uint segmentSelection = 0;

            float segmentSelectionVal = totalWeight;
            while (segmentSelectionVal == totalWeight) { segmentSelectionVal = Random.Range(0.0f, totalWeight); } // Ensure that it excludes the highest possible value. Important to avoid tiny chance of out-of-bounds issues.

            float runningTotal = 0.0f;
            bool found = false;
            for (uint i = 0; i + 2 < values.Length && !found; i += 2)
            {
                float nextRunningTotal = runningTotal + values[i + 2];
                if (nextRunningTotal > segmentSelectionVal)
                {
                    found = true;
                    segmentSelection = i;

                    float prop = (segmentSelectionVal - runningTotal) / values[i + 1];
                    return ((prop * (values[i + 2] - values[i])) + values[i]);
                }
                runningTotal = nextRunningTotal;
            }

            Debug.LogWarning("An instruction that should never be reached has been executed in WeightedDistributions.RandomWeightedRange().");

            return Random.Range(values[segmentSelection], values[segmentSelection + 2]);
        }

        static public float RandomWeightedRange(params HistogramSection[] slots)
        {
            if (slots.Length == 0)
            {
                Debug.LogError("Misuse of WeightedDistributions.RandomWeightedRange(), no valid input slots.");
                return 0.0f;
            }

            float totalWeight = 0.0f;
            for (int i = 0; i < slots.Length; i++)
            {
                totalWeight += slots[i].m_weight;
            }
            if (totalWeight <= 0.0f)
            {
                Debug.LogError("Misuse of WeightedDistributions.RandomWeightedRange(), total weight is equal to or less than zero.");
                return 0.0f;
            }

            uint segmentSelection = 0;

            float segmentSelectionVal = totalWeight;
            while (segmentSelectionVal == totalWeight) { segmentSelectionVal = Random.Range(0.0f, totalWeight); } // Ensure that it excludes the highest possible value. Important to avoid tiny chance of out-of-bounds issues.

            float runningTotal = 0.0f;
            bool found = false;
            for (uint i = 0; i < slots.Length && !found; i++)
            {
                float nextRunningTotal = runningTotal + slots[i].m_weight;
                if (nextRunningTotal > segmentSelectionVal)
                {
                    found = true;
                    segmentSelection = i;

                    float prop = (segmentSelectionVal - runningTotal) / slots[i].m_weight;
                    return ((prop * (slots[i].m_max - slots[i].m_min)) + slots[i].m_min);
                }
                runningTotal = nextRunningTotal;
            }

            Debug.LogWarning("An instruction that should never be reached has been executed in WeightedDistributions.RandomWeightedRange().");

            return Random.Range(slots[segmentSelection].m_min, slots[segmentSelection].m_max);
        }

        // Returns a random value with a weighting derived from histogram generated from points given. Starts with pair of upper and lower bounds (inclusive, exclusive), splits intermediate range into number of segments equal to input floats, weights each segment according to corresponsing float.
        static public float RandomValueFromHistogram(Vector2 limits, params float[] weights)
        {
            if (limits.x == limits.y) { return limits.x; }
            if (limits.x > limits.y) { limits = new Vector2(limits.y, limits.x); }

            float range = limits.y - limits.x;
            float segmentSize = range / weights.Length;
            float totalWeight = 0.0f;
            foreach (var w in weights) { totalWeight += w; }

            if (totalWeight <= 0.0f)
            {
                Debug.LogError("Misuse of WeightedDistributions.RandomValueFromHistogram(), total weight is equal to or less than zero.");
                return 0.0f;
            }

            uint segmentSelection = 0;

            float segmentSelectionVal = totalWeight;
            while (segmentSelectionVal == totalWeight) { segmentSelectionVal = Random.Range(0.0f, totalWeight); } // Ensure that it excludes the highest possible value. Important to avoid tiny chance of out-of-bounds issues.

            float runningTotal = 0.0f;
            bool found = false;
            for (uint i = 0; i < weights.Length && !found; i++)
            {
                float nextRunningTotal = runningTotal + weights[i];
                if (nextRunningTotal > segmentSelectionVal)
                {
                    found = true;
                    segmentSelection = i;

                    float prop = (segmentSelectionVal - runningTotal) / weights[i];
                    return (((float)i + prop) * segmentSize);
                }
                runningTotal = nextRunningTotal;
            }

            Debug.LogWarning("An instruction that should never be reached has been executed in WeightedDistributions.RandomValueFromHistogram().");

            return Random.Range(((float)segmentSelection * segmentSize), (((float)segmentSelection + 1.0f) * segmentSize));

        }

        // Returns a random value with a weighting derived from histogram generated from points given. Starts with pair of upper and lower bounds (inclusive, exclusive), splits intermediate range into number of segments equal to input floats, weights each segment according to corresponsing float.
        static public float RandomValueFromHistogram(Vector2 limits, List<float> weights)
        {
            if (limits.x == limits.y) { return limits.x; }
            if (limits.x > limits.y) { limits = new Vector2(limits.y, limits.x); }

            float range = limits.y - limits.x;
            float segmentSize = range / weights.Count;
            float totalWeight = 0.0f;
            foreach (var w in weights) { totalWeight += w; }

            if (totalWeight <= 0.0f)
            {
                Debug.LogError("Misuse of WeightedDistributions.RandomValueFromHistogram(), total weight is equal to or less than zero.");
                return 0.0f;
            }

            int segmentSelection = 0;

            float segmentSelectionVal = totalWeight;
            while (segmentSelectionVal == totalWeight) { segmentSelectionVal = Random.Range(0.0f, totalWeight); } // Ensure that it excludes the highest possible value. Important to avoid tiny chance of out-of-bounds issues.

            float runningTotal = 0.0f;
            bool found = false;
            for (int i = 0; i < weights.Count && !found; i++)
            {
                float nextRunningTotal = runningTotal + weights[i];
                if (nextRunningTotal > segmentSelectionVal)
                {
                    found = true;
                    segmentSelection = i;

                    float prop = (segmentSelectionVal - runningTotal) / weights[i];
                    return ((((float)i + prop) * segmentSize) + limits.x);
                }
                runningTotal = nextRunningTotal;
            }

            Debug.LogWarning("An instruction that should never be reached has been executed in WeightedDistributions.RandomValueFromHistogram().");

            return (Random.Range(((float)segmentSelection * segmentSize), (((float)segmentSelection + 1.0f) * segmentSize))+limits.x);

        }
    }
}