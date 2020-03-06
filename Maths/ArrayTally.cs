/* Original code[1] Copyright (c) 2020 Shane Celis[2]
   Licensed under the MIT License[3]

   [1]: https://github.com/shanecelis/SeawispHunter.Maths
   [2]: https://twitter.com/shanecelis
   [3]: https://opensource.org/licenses/MIT
*/

using System;
using System.Linq;
using System.Collections.Generic;

namespace SeawispHunter.Maths {

/** Keep a tally of an array of items. */
public class ArrayTally<T> {

  public readonly int binCount;
  int[,] counts;
  int samples = 0;
  public readonly Func<T, int> bin;

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

  public ArrayTally(int binCount, Func<T, int> bin) {
    this.binCount = binCount;
    this.bin = bin;
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
      int j = bin(x[i]);
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

  public float[] Probability(T[] xs)
    => xs.Select((x, i) => probability[i][bin(x)]).ToArray();

  public float[] Entropy(int? basis = null)
    => Enumerable.Range(0, counts.GetLength(0))
    .Select(i => ProbabilityDistribution.Entropy(probability[i], basis))
    .ToArray();
}

/** Keep a tally of a pair of arrays of items. */
public class ArrayTally<X, Y> {

  public readonly int binCountX;
  public readonly int binCountY;
  int[,,,] counts;
  int samples = 0;
  public readonly Func<X, int> binX;
  public readonly Func<Y, int> binY;

  private int sampleAtX = -1;
  private float[][] _probabilityX;
  public float[][] probabilityX {
    get {
      if (sampleAtX != samples) {
        for (int i = 0; i < counts.GetLength(0); i++) {
          for (int k = 0; k < counts.GetLength(2); k++) {
            int accum = 0;
            // for (int j = 0; j < counts.GetLength(1); j++)
            int j = 0;
            for (int l = 0; l < counts.GetLength(3); l++)
              accum += counts[i, j, k, l];
            _probabilityX[i][k] = (float) accum / samples;
          }
        }
        sampleAtX = samples;
      }
      return _probabilityX;
    }
  }

  private int sampleAtY = -1;
  private float[][] _probabilityY;
  public float[][] probabilityY {
    get {
      if (sampleAtY != samples) {
        for (int j = 0; j < counts.GetLength(1); j++) {
          for (int l = 0; l < counts.GetLength(3); l++) {
            int accum = 0;
            // for (int i = 0; i < counts.GetLength(0); i++)
            int i = 0;
            for (int k = 0; k < counts.GetLength(2); k++)
              accum += counts[i, j, k, l];
            _probabilityY[j][l] = (float) accum / samples;
          }
        }
        sampleAtY = samples;
      }
      return _probabilityY;
    }
  }

  private int sampleAtXY = -1;
  private float[,][,] _probabilityXY;
  /** Joint probability of an X and Y element. */
  public float[,][,] probabilityXY {
    get {
      if (sampleAtXY != samples) {
        for (int i = 0; i < counts.GetLength(0); i++)
          for (int j = 0; j < counts.GetLength(1); j++)
            for (int k = 0; k < counts.GetLength(2); k++)
              for (int l = 0; l < counts.GetLength(3); l++)
                _probabilityXY[i, j][k, l] = (float) counts[i, j, k, l] / samples;
        sampleAtXY = samples;
      }
      return _probabilityXY;
    }
  }

  public ArrayTally(int binCountX, Func<X, int> binX,
                    int binCountY, Func<Y, int> binY) {
    this.binCountX = binCountX;
    this.binX = binX;
    this.binCountY = binCountY;
    this.binY = binY;
  }

  /** Add a sample to the tally counts. */
  public void Add(X[] x, Y[] y) {
    if (counts == null) {
      counts = new int[x.Length, y.Length, binCountX, binCountY];
      _probabilityX = new float[x.Length][];
      for (int i = 0; i < x.Length; i++)
        _probabilityX[i] = new float[binCountX];

      _probabilityY = new float[y.Length][];
      for (int j = 0; j < y.Length; j++)
        _probabilityY[j] = new float[binCountY];

      _probabilityXY = new float[x.Length, y.Length][,];
      for (int i = 0; i < x.Length; i++)
        for (int j = 0; j < y.Length; j++)
          _probabilityXY[i, j] = new float[binCountX, binCountY];
    }

    if (x.Length != counts.GetLength(0))
      throw new Exception($"Expected {counts.GetLength(0)} elements; received {x.Length}.");
    if (y.Length != counts.GetLength(1))
      throw new Exception($"Expected {counts.GetLength(1)} elements; received {y.Length}.");
    for (int i = 0; i < x.Length; i++) {
      int k = binX(x[i]);
      if (k < 0 || k >= binCountX)
        throw new ArgumentException($"Item {x[i]} expected in bin [0, {binCountX}) but placed in {k}.");
      for (int j = 0; j < y.Length; j++) {

        int l = binY(y[j]);
        if (l < 0 || l >= binCountY)
          throw new ArgumentException($"Item {y[j]} expected in bin [0, {binCountY}) but placed in {l}.");
        counts[i, j, k, l]++;
      }
    }
    samples++;
  }

  /** Reset the frequency counter. */
  public void Clear() {
    Array.Clear(counts, 0, counts.Length);
    Array.Clear(_probabilityX, 0, _probabilityX.Length);
    Array.Clear(_probabilityY, 0, _probabilityY.Length);
    Array.Clear(_probabilityXY, 0, _probabilityXY.Length);
    samples = 0;
    sampleAtX = -1;
    sampleAtY = -1;
    sampleAtXY = -1;
  }

  protected static float[,] NewArray(int xLength, int yLength, Func<int, int, float> f) {
    var xys = new float[xLength, yLength];
    for (int i = 0; i < xLength; i++)
      for (int j = 0; j < yLength; j++)
        xys[i, j] = f(i, j);
    return xys;
  }

  protected static float Fold(int xLength, int yLength, Func<int, int, float, float> f, float seed) {
    var xys = new float[xLength, yLength];
    float accum = seed;
    for (int i = 0; i < xLength; i++)
      for (int j = 0; j < yLength; j++)
        accum = f(i, j, seed);
    return accum;
  }

  public float[] ProbabilityX(X[] xs)
    => xs.Select((x, i) => probabilityX[i][binX(x)]).ToArray();

  public float[] ProbabilityY(Y[] ys)
    => ys.Select((y, j) => probabilityY[j][binY(y)]).ToArray();

  public float[,] ProbabilityXY(X[] xs, Y[] ys) {
    // var xys = new float[xs.Length, ys.Length];
    // for (int i = 0; i < xs.Length; i++)
    //   for (int j = 0; j < ys.Length; j++)
    //     xys[i, j] = probabilityXY[i, j][binX(xs[i]), binY(ys[j])];
    // return xys;
    return NewArray(xs.Length, ys.Length,
                    (i, j) => probabilityXY[i, j][binX(xs[i]), binY(ys[j])]);
  }

  public float[,] ProbabilityXGivenY(X[] xs, Y[] ys)
    => NewArray(xs.Length, ys.Length,
                (i, j) => ProbabilityDistribution.ConditionalProbabilityXY(probabilityXY[i, j], probabilityY[j])[binX(xs[i]), binY(ys[j])]);

  public float[,] ProbabilityYGivenX(Y[] ys, X[] xs)
    => NewArray(xs.Length, ys.Length,
                (i, j) => ProbabilityDistribution.ConditionalProbabilityXY(probabilityXY[i, j], probabilityX[i])[binX(xs[i]), binY(ys[j])]);

  public float[,] EntropyYGivenX(int? basis = null)
    => NewArray(binCountX, binCountY,
                (i, j) => ProbabilityDistribution.ConditionalEntropyYX(probabilityXY[i, j], probabilityX[i], basis));

  public float[,] EntropyXGivenY(int? basis = null)
    => NewArray(binCountX, binCountY,
                (i, j) => ProbabilityDistribution.ConditionalEntropyXY(probabilityXY[i, j], probabilityY[j], basis));

  public float[,] EntropyXY(int? basis = null)
    => NewArray(binCountX, binCountY,
                (i, j) => ProbabilityDistribution.JointEntropy(probabilityXY[i, j], basis));

  public float[] EntropyX(int? basis = null)
    => Enumerable.Range(0, binCountX)
    .Select(i => ProbabilityDistribution.Entropy(probabilityX[i], basis))
    .ToArray();

  public float[] EntropyY(int? basis = null)
    => Enumerable.Range(0, binCountY)
    .Select(j => ProbabilityDistribution.Entropy(probabilityY[j], basis))
    .ToArray();

  public float[,] MutualInformationXY(int? basis = null)
    => NewArray(binCountX, binCountY,
                (i, j) => ProbabilityDistribution.MutualInformation(probabilityX[i], probabilityY[j], probabilityXY[i, j], basis));

}

}
