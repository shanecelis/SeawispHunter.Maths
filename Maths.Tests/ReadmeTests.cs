
/* Original code[1] Copyright (c) 2020 Shane Celis[2]
   Licensed under the MIT License[3]

   [1]: https://github.com/shanecelis/SeawispHunter.Maths
   [2]: https://twitter.com/shanecelis
   [3]: https://opensource.org/licenses/MIT
*/

using System;
using Xunit;
using SeawispHunter.Maths;

namespace SeawispHunter.Maths.Tests {
public class ReadmeTests {
  [Fact]
  public void TestReadmeExample0() {
    int binCount = 3;
    Tally<float> tally = new Tally<float>(binCount, x => (int) (x * binCount));

    // Some where, this is called repeatedly.
    // tally.Add(neuron.value);
    // But let's supply some fake values for demonstration purposes.
    tally.Add(0.2f);
    tally.Add(0.1f);
    tally.Add(0.4f);
    tally.Add(0.5f);

    // Finally we analyze it.
    float[] p = tally.probability;
    Assert.Equal(new [] { 2/4f, 2/4f, 0f }, p);
    float H = ProbabilityDistribution.Entropy(p);
    // Here's the entropy without any normalization.
    Assert.Equal(0.7f, H, 1);

    // Let's use a base of 2 so the entropy is in the units of bits.
    float Hbits = ProbabilityDistribution.Entropy(p, 2);
    Assert.Equal(1f, Hbits, 1);
    // So this neuron's value carries one bit of information. It's either going
    // into the first bin or the second bin at an equal probability and never
    // going into the third bin.
  }


  [Fact]
  public void TestReadmeExample1() {
    int binCount = 4;
    Tally<float, float> tally
      = new Tally<float, float>(binCount, x => (int) (x * binCount),
                                binCount, y => (int) (y * binCount));

    // Some where, this is called repeatedly.
    // tally.Add(sensor.value, effector.value);
    // But let's supply some fake values for demonstration purposes.
    tally.Add(0.6f, 0.1f);
    tally.Add(0.5f, 0.5f);
    tally.Add(0.7f, 0.9f);
    tally.Add(0.7f, 0.3f);

    // Finally we analyze it.
    float[] px = tally.probabilityX;
    Assert.Equal(new [] { 0f, 0f, 1f, 0f }, px);
    float[] py = tally.probabilityY;
    Assert.Equal(new [] { 1/4f, 1/4f, 1/4f, 1/4f }, py);
    float[,] pxy = tally.probabilityXY;
    Assert.Equal(new [,] {
        {   0f,   0f,   0f,   0f },
        {   0f,   0f,   0f,   0f },
        { 1/4f, 1/4f, 1/4f, 1/4f },
        {   0f,   0f,   0f,   0f },
      }, pxy);
    float Hsensor = ProbabilityDistribution.Entropy(px, 2);
    float Heffector = ProbabilityDistribution.Entropy(py, 2);
    // H(effector | sensor)
    float Heffector_sensor = ProbabilityDistribution.ConditionalEntropyYX(pxy, px, 2);

    Assert.Equal(0f, Hsensor, 1);
    // So the sensor carries no information. It's going to the second bin always
    // based on what's been seen.
    Assert.Equal(2f, Heffector, 1);
    // The effector carries 2 bits of information. It could show up in any of
    // the bins with equal probability.  It would take two bits to describe which bin.
    Assert.Equal(2f, Heffector_sensor, 1);
    // Given that we know the sensor, there's no reduction in randomness for the
    // effector. In fact since H(effector) = H(effector|sensor) we now know that
    // the sensor and effector are entirely independent of one another.
  }

  [Fact]
  public void TestReadmeExample2() {
    int binCount = 4;
    ArrayTally<float, float> tally
      = new ArrayTally<float, float>(binCount, x => (int) (x * binCount),
                                     binCount, y => (int) (y * binCount));

    // Some where, this is called repeatedly.
    // tally.Add(sensor.value, effector.value);
    // But let's supply some fake values for demonstration purposes.
    tally.Add(new [] { 0.6f, 0.1f }, new [] { 0.1f, 0f });
    tally.Add(new [] { 0.5f, 0.5f }, new [] { 0.5f, 0f });
    tally.Add(new [] { 0.7f, 0.9f }, new [] { 0.9f, 0f });
    tally.Add(new [] { 0.7f, 0.3f }, new [] { 0.3f, 0f });


    // float[] px = tally.probabilityX[0];
    // float[] py = tally.probabilityY[0];
    // float[,] pxy = tally.probabilityXY[0, 0];
    // If we analyze the first element of X and Y, we'll get the same results
    // from example 2.  However, if we look at the second element of X, which
    // is the same as the first element of Y, we'll get something different.

    // Finally we analyze it.
    float[] px = tally.probabilityX[1];
    Assert.Equal(new [] { 1/4f, 1/4f, 1/4f, 1/4f }, px);
    float[] py = tally.probabilityY[0];
    Assert.Equal(new [] { 1/4f, 1/4f, 1/4f, 1/4f }, py);
    float[,] pxy = tally.probabilityXY[0, 0];
    float Hsensor = ProbabilityDistribution.Entropy(px, 2);
    float Heffector = ProbabilityDistribution.Entropy(py, 2);
    // H(effector | sensor)
    float Heffector_sensor = ProbabilityDistribution.ConditionalEntropyYX(pxy, px, 2);

    Assert.Equal(2f, Hsensor, 1);
    // So the sensor carries 2 bits of information. It's a copy of what the
    // first effector's producing.
    Assert.Equal(2f, Heffector, 1);
    // The effector carries 2 bits of information. It could show up in any of
    // the bins with equal probability.  It would take two bits to describe which bin.
    Assert.Equal(0f, Heffector_sensor, 1);
    // Given that we know the sensor, there's zero information required to know
    // how the effector will behave. H(effector|sensor) = 0 means the effector
    // is completely determined by the sensor, which makes sense since they're
    // the same values.
  }

}
}
