using System;
using System.Collections.Generic;

namespace SeawispHunter.InformationTheory {

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
    samples = 0;
    Array.Clear(_probabilityX, 0, _probabilityX.Length);
    sampleAtX = -1;
    Array.Clear(_probabilityY, 0, _probabilityY.Length);
    sampleAtY = -1;
    Array.Clear(_probabilityXY, 0, _probabilityXY.Length);
    sampleAtXY = -1;
  }

}

public class ArrayTallySingle : ArrayTally<float, float> {

  public ArrayTallySingle(int binCount, float min, float max)
    : base(binCount, x => (int) ((Clamp(x, min, max) - min) / (max - min) * (binCount - 1)),
           binCount, y => (int) ((Clamp(y, min, max) - min) / (max - min) * (binCount - 1))) { }

  static float Clamp(float x, float min, float max) => Math.Min(Math.Max(x, min), max);

}

public class ArrayTallyAlphabet : ArrayTally<string, int> {

  public ArrayTallyAlphabet(string[] alphabet)
    : base(alphabet.Length, x => Array.IndexOf(alphabet, x),
           alphabet.Length, y => y) { }
}
}
