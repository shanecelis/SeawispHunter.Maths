using System;
using System.Collections.Generic;

namespace SeawispHunter.InformationTheory {

public class ArrayTally<T> {

  public readonly int binCount;
  int[,] counts;
  int samples = 0;
  public readonly Func<T, int> binFunc;

  private int sampleAt = -1;
  private float[][] _probability;
  public float[][] probability {
    get {
      if (sampleAt != samples) {
        for (int i = 0; i < counts.GetLength(0); i++) {
          for (int j = 0; j < counts.GetLength(1); j++)
            _probability[i][j] = (float) counts[i, j] / samples;
        }
        sampleAt = samples;
      }
      return _probability;
    }
  }

  public ArrayTally(int binCount, Func<T, int> binFunc) {
    this.binCount = binCount;
    this.binFunc = binFunc;
  }

  /** Add a sample to the frequency count. */
  public void Add(T[] x) {
    if (counts == null) {
      counts = new int[x.Length, binCount];
      _probability = new float[x.Length][];
      for (int i = 0; i < x.Length; i++)
        _probability[i] = new float[binCount];
    }

    if (x.Length != counts.GetLength(0))
      throw new Exception($"Expected {counts.GetLength(0)} elements; received {x.Length}.");
    for (int i = 0; i < x.Length; i++) {
      int j = binFunc(x[i]);
      if (j < 0 || j >= binCount)
        throw new ArgumentException($"Item {x} expected in bin [0, {binCount}) but placed in {i}.");
      counts[i, j]++;
    }
    samples++;
  }

  /** Reset the frequency counter. */
  public void Clear() {
    Array.Clear(counts, 0, counts.Length);
    Array.Clear(_probability, 0, _probability.Length);
    sampleAt = -1;
    samples = 0;
  }

  /** Return the estimated probability of an element. */
  public float[] Probability(T[] x) {
    var prob = new float[counts.GetLength(0)];
    for (int i = 0; i < counts.GetLength(0); i++)
      prob[i] = (float) counts[i, binFunc(x[i])] / samples;
    return prob;
  }

  /** Calculate the entropy. */
  public float[] Entropy() {
    int dataCount = counts.GetLength(0);
    float[] entropy = new float[counts.GetLength(0)];
    for (int i = 0; i < dataCount; i++) {
      entropy[i] = ProbabilityDistribution.Entropy(probability[i]);
    }
    return entropy;
  }
}

public class FrequencyArrayTallySingle : ArrayTally<float> {

  public FrequencyArrayTallySingle(int binCount, float min, float max)
    : base(binCount, x => (int) ((Clamp(x, min, max) - min) / (max - min) * (binCount - 1))) { }

  static float Clamp(float x, float min, float max) => Math.Min(Math.Max(x, min), max);

}

public class FrequencyArrayTallyAlphabet : ArrayTally<string> {

  public FrequencyArrayTallyAlphabet(string[] alphabet)
    : base(alphabet.Length, x => Array.IndexOf(alphabet, x)) { }
}
}
