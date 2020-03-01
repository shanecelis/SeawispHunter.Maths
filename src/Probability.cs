using System;

public static class Probability {

  public static bool IsValidDistribution(float[] p) {
    float sum = 0f;
    for (int i = 0; i < p.Length; i++)
      sum += p[i];
    return sum == 1f;
  }
  public static void NormalizeDistribution(float[] p) {
    float sum = 0f;
    for (int i = 0; i < p.Length; i++)
      sum += p[i];
    for (int i = 0; i < p.Length; i++)
      p[i] /= sum;
  }

  public static bool IsValidDistribution(float[,] p) {
    float sum = 0f;
    for (int i = 0; i < p.GetLength(0); i++)
      for (int j = 0; j < p.GetLength(0); j++)
        sum += p[i, j];
    return sum == 1f;
  }

  public static void NormalizeDistribution(float[,] p) {
    float sum = 0f;
    for (int i = 0; i < p.GetLength(0); i++)
      for (int j = 0; j < p.GetLength(0); j++)
        sum += p[i, j];

    for (int i = 0; i < p.GetLength(0); i++)
      for (int j = 0; j < p.GetLength(0); j++)
        p[i, j] /= sum;
  }

  public static float Entropy(float[] p, int? basis = null) {
    float accum = 0f;
    for (int i = 0; i < p.Length; i++)
      accum += p[i] * (float) Math.Log(p[i]);
    if (basis.HasValue)
      return -accum / (float) Math.Log(basis.Value);
    else
      return -accum;
  }

  public static float ConditionalEntropyYX(float[,] p_xy, float[] p_x, int? basis = null) {
    float accum = 0f;
    if (p_xy.GetLength(0) != p_x.Length)
      throw new ArgumentException($"Expected p_xy.GetLength(0) ({p_xy.GetLength(0)}) to equal p_x.Length ({p_x.Length}).");
    for (int i = 0; i < p_xy.GetLength(0); i++)
      for (int j = 0; j < p_xy.GetLength(1); j++)
        if (p_xy[i, j] != 0f && p_x[i] != 0f)
          accum += p_xy[i, j] * (float) Math.Log(p_xy[i, j] / p_x[i]);
    if (basis.HasValue)
      return -accum / (float) Math.Log(basis.Value);
    else
      return -accum;
  }

  public static float ConditionalEntropyXY(float[,] p_xy, float[] p_y, int? basis = null) {
    float accum = 0f;
    if (p_xy.GetLength(1) != p_y.Length)
      throw new ArgumentException($"Expected p_xy.GetLength(1) ({p_xy.GetLength(1)}) to equal p_y.Length ({p_y.Length}).");
    for (int i = 0; i < p_xy.GetLength(0); i++)
      for (int j = 0; j < p_xy.GetLength(1); j++)
        if (p_xy[i, j] != 0f && p_y[j] != 0f)
          accum += p_xy[i, j] * (float) Math.Log(p_xy[i, j] / p_y[j]);
    if (basis.HasValue)
      return - accum / (float) Math.Log(basis.Value);
    else
      return - accum;
  }

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

  public static float[] MarginalDistributionX(float[,] p_xy) {
    float[] p_x = new float[p_xy.GetLength(0)];
    for (int i = 0; i < p_xy.GetLength(0); i++)
      for (int j = 0; j < p_xy.GetLength(1); j++)
        p_x[i] += p_xy[i, j];
    return p_x;
  }

  public static float[] MarginalDistributionY(float[,] p_xy) {
    float[] p_y = new float[p_xy.GetLength(1)];
    for (int i = 0; i < p_xy.GetLength(0); i++)
      for (int j = 0; j < p_xy.GetLength(1); j++)
        p_y[j] += p_xy[i, j];
    return p_y;
  }

  public static float MutualInformation(float[] p_x, float[] p_y, float[,] p_xy, int? basis = null) {
    return Entropy(p_x, basis) + Entropy(p_y, basis) - JointEntropy(p_xy, basis);
  }

}
