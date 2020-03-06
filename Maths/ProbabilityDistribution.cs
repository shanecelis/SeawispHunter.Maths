/* Original code[1] Copyright (c) 2020 Shane Celis[2]
   Licensed under the MIT License[3]

   [1]: https://github.com/shanecelis/SeawispHunter.Maths
   [2]: https://twitter.com/shanecelis
   [3]: https://opensource.org/licenses/MIT
*/

using System;

namespace SeawispHunter.Maths {

/** Probability distribution

    Defined as an array of floats that sum to 1 and each item is greater than or
    equal to 0 and less than or equal to 1.
*/
public static class ProbabilityDistribution {

  /**
      __
     \     p(x)  =  1
     /__ x

   */
  public static bool IsValid(float[] p) {
    float sum = 0f;
    for (int i = 0; i < p.Length; i++)
      sum += p[i];
    // XXX: Should have an epsilon check here, not a bare equals for floats.
    return sum == 1f;
  }

  /**
     ^           p(x)
     p(x)  =  ----------
               __
              \     p(x)
              /__ x

   */
  public static void Normalize(float[] p) {
    float sum = 0f;
    for (int i = 0; i < p.Length; i++)
      sum += p[i];
    for (int i = 0; i < p.Length; i++)
      p[i] /= sum;
  }

  /**
      __    __
     \     \     p(x, y)  ==  1
     /__ x /__ y

   */
  public static bool IsValid(float[,] p) {
    float sum = 0f;
    for (int i = 0; i < p.GetLength(0); i++)
      for (int j = 0; j < p.GetLength(1); j++)
        sum += p[i, j];
    // XXX: Should have an epsilon check here, not a bare equals for floats.
    return sum == 1f;
  }


  /**
     ^                p(x,y)
     p(x,y)  =  -------------------
                 __    __
                \     \     p(x, y)
                /__ x /__ y

   */
  public static void Normalize(float[,] p) {
    float sum = 0f;
    for (int i = 0; i < p.GetLength(0); i++)
      for (int j = 0; j < p.GetLength(1); j++)
        sum += p[i, j];

    for (int i = 0; i < p.GetLength(0); i++)
      for (int j = 0; j < p.GetLength(1); j++)
        p[i, j] /= sum;
  }

  /**
                __             1
      H(X)  =  \    p(x) log ----
               /__           p(x)

   */
  public static float Entropy(float[] p, int? basis = null) {
    float accum = 0f;
    for (int i = 0; i < p.Length; i++)
      if (p[i] != 0f)
        accum += p[i] * (float) Math.Log(p[i]);
    if (basis.HasValue)
      return -accum / (float) Math.Log(basis.Value);
    else
      return -accum;
  }

  /**
                P(X,Y)
     P(Y|X)  =  ------
                 P(X)
   */
  public static float[,] ConditionalProbabilityYX(float[,] p_xy, float[] p_x) {
    float[,] result = new float[p_xy.GetLength(0), p_xy.GetLength(1)];
    for (int i = 0; i < p_xy.GetLength(0); i++)
      for (int j = 0; j < p_xy.GetLength(1); j++)
        if (p_x[i] != 0f)
          result[i, j] = p_xy[i, j] / p_x[i];
    return result;
  }

  /**
                P(X,Y)
     P(X|Y)  =  ------
                 P(Y)
   */
  public static float[,] ConditionalProbabilityXY(float[,] p_xy, float[] p_y) {
    float[,] result = new float[p_xy.GetLength(0), p_xy.GetLength(1)];
    for (int i = 0; i < p_xy.GetLength(0); i++)
      for (int j = 0; j < p_xy.GetLength(1); j++)
        if (p_y[i] != 0f)
          result[i, j] = p_xy[i, j] / p_y[j];
    return result;
  }

  /**
                 __               p(x)
     H(Y|X)  =  \    p(x, y) log -------
                /__              p(x, y)

   */
  public static float ConditionalEntropyYX(float[,] p_xy, float[] p_x, int? basis = null) {
    float accum = 0f;
    if (p_xy.GetLength(0) != p_x.Length)
      throw new
        ArgumentException($"Expected p_xy.GetLength(0) ({p_xy.GetLength(0)}) to equal p_x.Length ({p_x.Length}).");
    for (int i = 0; i < p_xy.GetLength(0); i++)
      for (int j = 0; j < p_xy.GetLength(1); j++)
        if (p_xy[i, j] != 0f && p_x[i] != 0f)
          accum += p_xy[i, j] * (float) Math.Log(p_xy[i, j] / p_x[i]);
    if (basis.HasValue)
      return -accum / (float) Math.Log(basis.Value);
    else
      return -accum;
  }

  /**
                 __               p(y)
     H(X|Y)  =  \    p(x, y) log -------
                /__              p(x, y)

   */
  public static float ConditionalEntropyXY(float[,] p_xy, float[] p_y, int? basis = null) {
    float accum = 0f;
    if (p_xy.GetLength(1) != p_y.Length)
      throw new
        ArgumentException($"Expected p_xy.GetLength(1) ({p_xy.GetLength(1)}) to equal p_y.Length ({p_y.Length}).");
    for (int i = 0; i < p_xy.GetLength(0); i++)
      for (int j = 0; j < p_xy.GetLength(1); j++)
        if (p_xy[i, j] != 0f && p_y[j] != 0f)
          accum += p_xy[i, j] * (float) Math.Log(p_xy[i, j] / p_y[j]);
    if (basis.HasValue)
      return -accum / (float) Math.Log(basis.Value);
    else
      return -accum;
  }

  /**
                   __            p(x)
      D(p||q)  =  \     p(x) log ----
                  /__ x          q(x)

   */
  public static float RelativeEntropy(float[] p_x, float[] q_x, int? basis = null) {
    float accum = 0f;
    if (p_x.Length != q_x.Length)
      throw new ArgumentException($"Expected same lengths but p_x and q_x are {p_x.Length} and {q_x.Length} respectively.");
    for (int i = 0; i < p_x.Length; i++)
      if (p_x[i] != 0f && q_x[i] != 0f)
        accum = p_x[i] * (float) Math.Log(p_x[i] / q_x[i]);
    if (basis.HasValue)
      return -accum / (float) Math.Log(basis.Value);
    else
      return -accum;
  }

  /**
                  __                 1
     h(x, y)  =  \    p(x, y) log ------
                 /__              p(x,y)

  */
  public static float JointEntropy(float[,] p_xy, int? basis = null) {
    float accum = 0f;
    for (int i = 0; i < p_xy.GetLength(0); i++)
      for (int j = 0; j < p_xy.GetLength(1); j++)
        if (p_xy[i, j] != 0f)
          accum += p_xy[i, j] * (float) Math.Log(p_xy[i, j]);
    if (basis.HasValue)
      return -accum / (float) Math.Log(basis.Value);
    else
      return -accum;
  }

  /**
                __
     p (x)  =  \     p(x, y)
      X        /__ y

   */
  public static float[] MarginalX(float[,] p_xy) {
    float[] p_x = new float[p_xy.GetLength(0)];
    for (int i = 0; i < p_xy.GetLength(0); i++)
      for (int j = 0; j < p_xy.GetLength(1); j++)
        p_x[i] += p_xy[i, j];
    return p_x;
  }

  /**
                __
     p (y)  =  \     p(x, y)
      Y        /__ x

   */
  public static float[] MarginalY(float[,] p_xy) {
    float[] p_y = new float[p_xy.GetLength(1)];
    for (int i = 0; i < p_xy.GetLength(0); i++)
      for (int j = 0; j < p_xy.GetLength(1); j++)
        p_y[j] += p_xy[i, j];
    return p_y;
  }

  /**
     I(X; Y)  =  H(X, Y)  -  H(X|Y)  -  H(Y|X)
   */
  public static float MutualInformation(float[] p_x, float[] p_y, float[,] p_xy, int? basis = null) {
    return Entropy(p_x, basis) + Entropy(p_y, basis) - JointEntropy(p_xy, basis);
  }

  /**
     VI(X;Y)  =  H(X, Y)  -  I(X; Y)

     Similar to Mutual Information I(X; Y) but it's a true metric; it obeys the
     triangle inequality.
   */
  public static float VariationOfInformation(float[] p_x, float[] p_y, float[,] p_xy, int? basis = null) {
    return JointEntropy(p_xy, basis) - MutualInformation(p_x, p_y, p_xy, basis);
  }

}
}
