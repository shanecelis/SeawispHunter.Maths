using System;
using System.Collections.Generic;

namespace SeawispHunter.InformationTheory {

public class FrequencyArray<T> {

  public readonly int binCount;
  int[,] counts;
  int samples = 0;
  public readonly Func<T, int> binFunc;

  public FrequencyArray(int binCount, Func<T, int> binFunc) {
    this.binCount = binCount;
    this.binFunc = binFunc;
  }

  /** Add a sample to the frequency count. */
  public void Add(T[] x) {
    if (counts == null)
      counts = new int[x.Length, binCount];
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
    float accum;

    for (int i = 0; i < dataCount; i++) {
      accum = 0f;
      for (int j = 0; j < binCount; j++) {
        if (counts[i, j] != 0) {
          float p_j = (float) counts[i, j] / samples;
          accum += p_j * (float) Math.Log(p_j);
        }
      }
      entropy[i] = -accum / (float) Math.Log(binCount);
    }
    return entropy;
  }
}

public class FrequencyArraySingle : FrequencyArray<float> {

  public FrequencyArraySingle(int binCount, float min, float max)
    : base(binCount, x => (int) ((Clamp(x, min, max) - min) / (max - min) * (binCount - 1))) { }

  static float Clamp(float x, float min, float max) => Math.Min(Math.Max(x, min), max);

}

public class FrequencyArrayAlphabet : FrequencyArray<string> {

  public FrequencyArrayAlphabet(string[] alphabet)
    : base(alphabet.Length, x => Array.IndexOf(alphabet, x)) { }
}
}
