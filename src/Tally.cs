using System;
using System.Collections.Generic;

namespace SeawispHunter.InformationTheory {

  public class Tally<T> {

    public readonly int binCount;
    int[] counts;
    int samples = 0;
    public readonly Func<T, int> binFunc;

    public Tally(int binCount, Func<T, int> binFunc) {
      this.binCount = binCount;
      this.binFunc = binFunc;
      this.counts = new int[binCount];
    }

    /** Add a sample to its frequency count. */
    public void Add(T x) {
      int i = binFunc(x);
      if (i < 0 || i >= binCount)
        throw new ArgumentException($"Item {x} expected in bin [0, {binCount}) but placed in {i}.");
      counts[i]++;
      samples++;
    }

    /** Reset the frequency counter. */
    public void Clear() {
      Array.Clear(counts, 0, binCount);
      samples = 0;
    }

    /** Return the estimated probability of an element. */
    public float Probability(T x) {
      return (float) counts[binFunc(x)] / samples;
    }

    /** Calculate the entropy.

                                   ln(p )
                     __ 10             j
          E  =   -  \           p  ------
                    /__ j = 1    j ln(10)

      Doncieux, S., CEC, J. M. E. C., 2013. (n.d.). Behavioral diversity
      with multiple behavioral distances. Ieeexplore.Ieee.org.
      http://doi.org/10.1109/CEC.2013.6557731
  */
    public float Entropy() {
      float accum = 0f;
      for (int j = 0; j < binCount; j++) {
        if (counts[j] != 0) {
          float p_j = (float) counts[j] / samples;
          accum += p_j * (float) Math.Log(p_j);
        }
      }
      return -accum / (float) Math.Log(binCount);
    }
  }

  public class TallyFloat : Tally<float> {

    public TallyFloat(int binCount, float min, float max)
      : base(binCount, x => (int) ((Clamp(x, min, max) - min) / (max - min) * (binCount - 1))) { }

    public static float Clamp(float x, float min, float max) => Math.Min(Math.Max(x, min), max);

  }

  public class TallyAlphabet : Tally<string> {

    public TallyAlphabet(string[] alphabet)
      : base(alphabet.Length, x => Array.IndexOf(alphabet, x)) { }
  }


  public class Tally<X, Y> {

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

    public Tally(int binCount, Func<X, int> binXFunc, Func<Y, int> binYFunc) {
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
      samples = 0;
      pxSampleLock = -1;
      pySampleLock = -1;
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
      return EntropyX() + EntropyY() - EntropyXY();
    }

  }

public class TallyAlphabetPair : Tally<string, int> {

  public TallyAlphabetPair(string[] xalphabet)
    : base(xalphabet.Length,
           x => Array.IndexOf(xalphabet, x),
           y => y) { }
}
}
