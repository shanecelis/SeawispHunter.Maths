using System;
using System.Collections.Generic;

namespace SeawispHunter.InformationTheory {

public class FrequencyPairArray<T> {

  public readonly int binCount;
  int[,,,] counts;
  int samples = 0;
  public readonly Func<T, int> binFunc;

  public FrequencyPairArray(int binCount, Func<T, int> binFunc) {
    this.binCount = binCount;
    this.binFunc = binFunc;
  }

  /** Add a sample to the frequency count. */
  public void Add(T[] x, T[] y) {
    if (counts == null)
      counts = new int[x.Length, y.Length, binCount, binCount];
    if (x.Length != counts.GetLength(0))
      throw new Exception($"Expected {counts.GetLength(0)} elements; received {x.Length}.");
    if (y.Length != counts.GetLength(1))
      throw new Exception($"Expected {counts.GetLength(1)} elements; received {y.Length}.");
    for (int i = 0; i < x.Length; i++) {
      int k = binFunc(x[i]);
      if (k < 0 || k >= binCount)
        throw new ArgumentException($"Item {x[i]} expected in bin [0, {binCount}) but placed in {k}.");
      for (int j = 0; j < y.Length; j++) {

        int l = binFunc(y[j]);
        if (l < 0 || l >= binCount)
          throw new ArgumentException($"Item {y[j]} expected in bin [0, {binCount}) but placed in {l}.");
        counts[i, j, k, l]++;
      }
    }
    samples++;
  }

  /** Reset the frequency counter. */
  public void Clear() {
    Array.Clear(counts, 0, counts.Length);
    samples = 0;
  }

  /** Return the estimated probability. */
  public float[] ProbabilityX(T[] x) {
    var prob = new float[counts.GetLength(0)];
    for (int i = 0; i < counts.GetLength(0); i++) {
      int k = binFunc(x[i]);
      for (int j = 0; j < counts.GetLength(1); j++)
        for (int l = 0; l < binCount; l++)
          prob[i] += (float) counts[i, j, k, l];
      prob[i] /= samples;
    }
    return prob;
  }

  /**

                      __              p(x, y)
      H(Y|X)  =   -  \    p(x, y) log -------
                    /__               p(x)


   */
  public float[,] EntropyYGivenX() {
    float[,] condEntropy = new float[counts.GetLength(0), counts.GetLength(1)];

    for (int i = 0; i < counts.GetLength(0); i++) {

      for (int j = 0; j < counts.GetLength(1); j++) {
      }

    }
    return condEntropy;

  }

  /** Calculate the entropy. */
  public float[] Entropy() {

    int dataCount = counts.GetLength(0);
    float[] entropy = new float[counts.GetLength(0)];
    // float accum;

    // for (int i = 0; i < dataCount; i++) {
    //   accum = 0f;
    //   for (int j = 0; j < binCount; j++) {
    //     if (counts[i, j] != 0) {
    //       float p_j = (float) counts[i, j] / samples;
    //       accum += p_j * (float) Math.Log(p_j);
    //     }
    //   }
    //   entropy[i] = -accum / (float) Math.Log(binCount);
    // }
    return entropy;
  }
}

public class FrequencyPairArraySingle : FrequencyPairArray<float> {

  public FrequencyPairArraySingle(int binCount, float min, float max)
    : base(binCount, x => (int) ((Clamp(x, min, max) - min) / (max - min) * (binCount - 1))) { }

  static float Clamp(float x, float min, float max) => Math.Min(Math.Max(x, min), max);

}

public class FrequencyPairArrayAlphabet : FrequencyPairArray<string> {

  public FrequencyPairArrayAlphabet(string[] alphabet)
    : base(alphabet.Length, x => Array.IndexOf(alphabet, x)) { }
}
}
