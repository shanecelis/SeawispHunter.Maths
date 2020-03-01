using System;
using System.Collections.Generic;

namespace SeawispHunter.InformationTheory {
  public class FrequencyCount<T> {

    public readonly int binCount;
    int[] counts;
    int samples = 0;
    public readonly Func<T, int> binFunc;

    public FrequencyCount(int binCount, Func<T, int> binFunc) {
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

  public class FrequencyCountSingle : FrequencyCount<float> {

    public FrequencyCountSingle(int binCount, float min, float max)
      : base(binCount, x => (int) ((Clamp(x, min, max) - min) / (max - min) * (binCount - 1))) { }

    public static float Clamp(float x, float min, float max) => Math.Min(Math.Max(x, min), max);

  }

  public class FrequencyCountAlphabet : FrequencyCount<string> {

    public FrequencyCountAlphabet(string[] alphabet)
      : base(alphabet.Length, x => Array.IndexOf(alphabet, x)) { }
  }
}
