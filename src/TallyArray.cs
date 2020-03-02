using System;
using System.Collections.Generic;

namespace SeawispHunter.InformationTheory {
  public class TallyArray<X, Y> {

    public readonly int binCount;
    int[,] counts;
    int samples = 0;
    public readonly Func<X, int> binXFunc;
    public readonly Func<Y, int> binYFunc;

    private int pxSampleLock = -1;
    private float[] _probabilityX;
    public float[] probabilityX {
      get {
        if (pxSampleLock != samples) {
          for (int i = 0; i < binCount; i++) {
            int accum = 0;
            for (int j = 0; j < binCount; j++)
              accum += counts[i, j];
            _probabilityX[i] = (float) accum / samples;
          }
          pxSampleLock = samples;
        }
        return _probabilityX;
      }
    }

    private int pySampleLock = -1;
    private float[] _probabilityY;
    public float[] probabilityY {
      get {
        if (pySampleLock != samples) {
          for (int j = 0; j < binCount; j++) {
            int accum = 0;
            for (int i = 0; i < binCount; i++)
              accum += counts[i, j];
            _probabilityY[j] = (float) accum / samples;
          }
          pySampleLock = samples;
        }
        return _probabilityY;
      }
    }

    private int pxySampleLock = -1;
    private float[,] _probabilityXY;
    public float[,] probabilityXY {
      get {
        if (pxySampleLock != samples) {
          for (int i = 0; i < binCount; i++)
            for (int j = 0; j < binCount; j++)
              _probabilityXY[i, j] = (float) counts[i, j] / samples;
          pxySampleLock = samples;
        }
        return _probabilityXY;
      }
    }

    public TallyArray(int binCount, Func<X, int> binXFunc, Func<Y, int> binYFunc) {
      this.binCount = binCount;
      this.binXFunc = binXFunc;
      this.binYFunc = binYFunc;
      this.counts = new int[binCount, binCount];
      this._probabilityX = new float[binCount];
      this._probabilityY = new float[binCount];
      this._probabilityXY = new float[binCount, binCount];
    }

    /** Add a sample to its frequency count. */
    public void Add(X x, Y y) {
      int i = binXFunc(x);
      int j = binYFunc(y);
      if (i < 0 || i >= binCount)
        throw new ArgumentException($"Item {x} expected in bin [0, {binCount}) but placed in {i}.");
      if (j < 0 || j >= binCount)
        throw new ArgumentException($"Item {y} expected in bin [0, {binCount}) but placed in {j}.");
      counts[i, j]++;
      samples++;
    }

    /** Reset the frequency counter. */
    public void Clear() {
      Array.Clear(counts, 0, counts.Length);
      Array.Clear(_probabilityX, 0, _probabilityX.Length);
      Array.Clear(_probabilityY, 0, _probabilityY.Length);
      Array.Clear(_probabilityXY, 0, _probabilityXY.Length);
      samples = 0;
      pxSampleLock = -1;
      pySampleLock = -1;
      pxySampleLock = -1;
    }

    /** Return the estimated probability of an element in X. */
    public float ProbabilityX(X x) {
      int i = binXFunc(x);
      return ProbabilityXByBin(i);
    }

    protected float ProbabilityXByBin(int i) {
      int accum = 0;
      for (int j = 0; j < binCount; j++)
        accum += counts[i, j];
      return (float) accum / samples;
    }

    protected float ProbabilityYByBin(int j) {
      int accum = 0;
      for (int i = 0; i < binCount; i++)
        accum += counts[i, j];
      return (float) accum / samples;
    }

    public float ProbabilityXY(X x, Y y) {
      int i = binXFunc(x);
      int j = binYFunc(y);
      return (float) counts[i, j] / samples;
    }

    public float ProbabilityXGivenY(X x, Y y) {
      return ProbabilityXY(x, y) / ProbabilityY(y);
    }

    public float ProbabilityYGivenX(Y y, X x) {
      return ProbabilityXY(x, y) / ProbabilityX(x);
    }

    /** Return the estimated probability of an element in Y. */
    public float ProbabilityY(Y y) {
      int j = binYFunc(y);
      return ProbabilityYByBin(j);
    }

    /** Calculate the conditional entropy.

                        __              p(x, y)
        H(Y|X)  =   -  \    p(x, y) log -------
                      /__               p(x)
    */
    public float EntropyYGivenX() {
      return ProbabilityDistribution.ConditionalEntropyYX(probabilityXY, probabilityX, binCount);
    }

    public float EntropyXGivenY() {
      return ProbabilityDistribution.ConditionalEntropyXY(probabilityXY, probabilityY, binCount);
    }

    public float EntropyXY() {
      return ProbabilityDistribution.JointEntropy(probabilityXY, binCount);
    }

    /** Calculate the entropy. */
    public float EntropyX() {
      return ProbabilityDistribution.Entropy(probabilityX, binCount);
    }

    public float EntropyY() {
      return ProbabilityDistribution.Entropy(probabilityY, binCount);
    }

    public float MutualInformationXY() {
      return ProbabilityDistribution.MutualInformation(probabilityX, probabilityY, probabilityXY, binCount);
    }

  }

  public class TallyArray<T> {

    public readonly int binCount;
    int[,] counts;
    int samples = 0;
    public readonly Func<T, int> binFunc;

    private int sampleAt = -1;
    private float[][] _probability;

    public float[][] probability {
      get {
        if (sampleAt != samples) {
          for (int i = 0; i < counts.GetLength(0); i++)
            for (int j = 0; j < counts.GetLength(1); j++)
              _probability[i][j] = (float) counts[i, j] / samples;

          sampleAt = samples;
        }

        return _probability;
      }
    }

    public TallyArray(int binCount, Func<T, int> binFunc) {
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

  public class FrequencyTallyArraySingle : TallyArray<float> {

    public FrequencyTallyArraySingle(int binCount, float min, float max)
      : base(binCount, x => (int) ((Clamp(x, min, max) - min) / (max - min) * (binCount - 1))) { }

    static float Clamp(float x, float min, float max) => Math.Min(Math.Max(x, min), max);

  }

  public class FrequencyTallyArrayAlphabet : TallyArray<string> {

    public FrequencyTallyArrayAlphabet(string[] alphabet)
      : base(alphabet.Length, x => Array.IndexOf(alphabet, x)) { }
  }
}