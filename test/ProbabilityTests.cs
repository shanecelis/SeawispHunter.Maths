using System;
using Xunit;
using SeawispHunter.InformationTheory;

namespace test {
public class ProbabilityTests {
  float[,] p_xy;
  float[] p_x;
  float[] p_y;

  /**
     https://www.cl.cam.ac.uk/teaching/0910/InfoTheory/Exercises09.pdf
  */
  [Fact]
  public void TestMarginalDistributionX() {
    p_xy = new [,] {
      { 1/8f, 1/16f, 1/32f, 1/32f},
      { 1/16f, 1/8f, 1/32f, 1/32f},
      { 1/16f, 1/16f, 1/16f, 1/16f },
      { 1/4f, 0f, 0f, 0f}
    };
    p_x = Probability.MarginalDistributionX(p_xy);
    p_y = Probability.MarginalDistributionY(p_xy);
    Assert.True(Probability.IsValidDistribution(p_x));
    Assert.True(Probability.IsValidDistribution(p_y));
    Assert.True(Probability.IsValidDistribution(p_xy));
    Assert.Equal(1/8f, p_xy[0, 0]);
    Assert.Equal(1/16f, p_xy[1, 0]);
    Assert.Equal(1/16f, p_xy[2, 0]);
    // Assert.Equal(1/8f, p_xy[0]);
    // Assert.Equal(1/16f, p_xy[1]);
    // Assert.Equal(1/32f, p_xy[2]);
    Assert.Equal(new [] { 1/4f, 1/4f, 1/4f, 1/4f }, p_x);
    Assert.Equal(new [] { 1/2f, 1/4f, 1/8f, 1/8f }, p_y);
    Assert.Equal(2f, Probability.Entropy(p_x, 2));
    Assert.Equal(7/4f, Probability.Entropy(p_y, 2));
    Assert.Equal(27/8f, Probability.JointEntropy(p_xy, 2));
    Assert.Equal(11/8f, Probability.ConditionalEntropyYX(p_xy, p_x, 2));
    Assert.Equal(3/8f, Probability.MutualInformation(p_x, p_y, p_xy, 2));
  }

  /** http://www.cs.tau.ac.il/~iftachh/Courses/Info/Fall14/Printouts/Lesson2_h.pdf */
  [Fact]
  public void TestExample() {
    p_xy = new [,] {
      { 1/4f, 1/4f },
      { 1/2f, 0f }
    };
    p_x = Probability.MarginalDistributionX(p_xy);
    p_y = Probability.MarginalDistributionY(p_xy);
    Assert.True(Probability.IsValidDistribution(p_x));
    Assert.True(Probability.IsValidDistribution(p_y));
    Assert.True(Probability.IsValidDistribution(p_xy));
    // Assert.Equal(2f, Probability.Entropy(p_x, 2));
    // Assert.Equal(7/4f, Probability.Entropy(p_y, 2));
    Assert.Equal(3/2f, Probability.JointEntropy(p_xy, 2));
    Assert.Equal(3/4f, Probability.JointEntropy(p_xy, p_xy.Length));
    Assert.Equal(1.04f, Probability.JointEntropy(p_xy), 2);
    Assert.Equal(1/2f, Probability.ConditionalEntropyYX(p_xy, p_x, 2));
    Assert.NotEqual(1/4f, Probability.ConditionalEntropyYX(p_xy, p_x));
    Assert.Equal(1/4f, Probability.ConditionalEntropyYX(p_xy, p_x, p_xy.Length), 2);
    Assert.Equal(1/2f, Probability.ConditionalEntropyYX(p_xy, p_x, p_x.Length), 2);
    Assert.Equal(0.35f, Probability.ConditionalEntropyYX(p_xy, p_x), 2);
    Assert.Equal(0.29f, Probability.ConditionalEntropyYX(p_xy, p_y, 2), 2);
    Assert.Equal(0.2f, Probability.ConditionalEntropyYX(p_xy, p_y), 2);
    // Assert.Equal(3/8f, Probability.MutualInformation(p_x, p_y, p_xy, 2));
  }

  [Fact]
  public void TestExample2() {
    p_xy = new [,] {
      { 1/4f, 1/4f, 0f },
      { 1/2f, 0f, 0f },
      { 0f, 0f, 0f }
    };
    p_x = Probability.MarginalDistributionX(p_xy);
    p_y = Probability.MarginalDistributionY(p_xy);
    // Assert.Equal(2f, Probability.Entropy(p_x, 2));
    // Assert.Equal(7/4f, Probability.Entropy(p_y, 2));
    Assert.Equal(3/2f, Probability.JointEntropy(p_xy, 2));
    Assert.Equal(0.47f, Probability.JointEntropy(p_xy, p_xy.Length), 2);
    Assert.Equal(1.04f, Probability.JointEntropy(p_xy), 2);
    Assert.Equal(1/2f, Probability.ConditionalEntropyYX(p_xy, p_x, 2));
    Assert.Equal(0.16f, Probability.ConditionalEntropyYX(p_xy, p_x, p_xy.Length), 2);
    Assert.Equal(0.32f, Probability.ConditionalEntropyYX(p_xy, p_x, p_x.Length), 2);
    Assert.Equal(0.35f, Probability.ConditionalEntropyYX(p_xy, p_x), 2);
    Assert.Equal(0.69f, Probability.ConditionalEntropyXY(p_xy, p_y, 2), 2);
    Assert.Equal(0.22f, Probability.ConditionalEntropyXY(p_xy, p_y, p_xy.Length), 2);
    Assert.Equal(0.43f, Probability.ConditionalEntropyXY(p_xy, p_y, p_y.Length), 2);
    Assert.Equal(0.48f, Probability.ConditionalEntropyXY(p_xy, p_y), 2);
    // Assert.Equal(3/8f, Probability.MutualInformation(p_x, p_y, p_xy, 2));
  }


  [Fact]
  public void TestTwoIndependentCoinFlipsBadSetup() {
    p_xy = new [,] {
      { 1/2f, 1/2f },
      { 1/2f, 1/2f }
    };
    p_x = Probability.MarginalDistributionX(p_xy);
    p_y = Probability.MarginalDistributionY(p_xy);
    Assert.False(Probability.IsValidDistribution(p_x));
    Assert.False(Probability.IsValidDistribution(p_y));
    Assert.False(Probability.IsValidDistribution(p_xy));
    Probability.NormalizeDistribution(p_x);
    Probability.NormalizeDistribution(p_y);
    Probability.NormalizeDistribution(p_xy);
    Assert.True(Probability.IsValidDistribution(p_x));
    Assert.True(Probability.IsValidDistribution(p_y));
    Assert.True(Probability.IsValidDistribution(p_xy));
  }

  [Fact]
  public void TestTwoIndependentCoinFlips() {
    p_xy = new [,] {
      { 1/4f, 1/4f },
      { 1/4f, 1/4f }
    };
    p_x = Probability.MarginalDistributionX(p_xy);
    p_y = Probability.MarginalDistributionY(p_xy);
    Assert.True(Probability.IsValidDistribution(p_x));
    Assert.True(Probability.IsValidDistribution(p_y));
    Assert.True(Probability.IsValidDistribution(p_xy));
    Assert.Equal(1f, Probability.Entropy(p_x, 2), 2);
    Assert.Equal(1f, Probability.Entropy(p_y, 2), 2);
    Assert.Equal(1f, Probability.Entropy(p_x, p_x.Length), 2);
    Assert.Equal(1f, Probability.Entropy(p_y, p_y.Length), 2);
    Assert.Equal(0.69f, Probability.Entropy(p_x), 2);
    Assert.Equal(0.69f, Probability.Entropy(p_y), 2);
    Assert.Equal(2f, Probability.JointEntropy(p_xy, 2), 2);
    Assert.Equal(1f, Probability.JointEntropy(p_xy, p_xy.Length), 2);
    Assert.Equal(1.39f, Probability.JointEntropy(p_xy), 2);
    Assert.Equal(1f, Probability.ConditionalEntropyYX(p_xy, p_x, 2), 2);
    Assert.Equal(1/2f, Probability.ConditionalEntropyYX(p_xy, p_x, p_xy.Length), 2);
    Assert.Equal(1f, Probability.ConditionalEntropyYX(p_xy, p_x, p_x.Length), 2);
    Assert.Equal(0.69f, Probability.ConditionalEntropyYX(p_xy, p_x), 2);
    Assert.Equal(1f, Probability.ConditionalEntropyXY(p_xy, p_y, 2), 2);
    Assert.Equal(0.69f, Probability.ConditionalEntropyXY(p_xy, p_y), 2);

    Assert.Equal(0f, Probability.MutualInformation(p_x, p_y, p_xy, 2), 2);
    Assert.Equal(0f, Probability.MutualInformation(p_x, p_y, p_xy), 2);
  }

  [Fact]
  public void TestThreeIndependentCoinFlips() {
    p_xy = new [,] {
      { 1/9f, 1/9f, 1/9f },
      { 1/9f, 1/9f, 1/9f },
      { 1/9f, 1/9f, 1/9f }
    };
    p_x = Probability.MarginalDistributionX(p_xy);
    p_y = Probability.MarginalDistributionY(p_xy);
    Assert.True(Probability.IsValidDistribution(p_x));
    Assert.True(Probability.IsValidDistribution(p_y));
    Assert.True(Probability.IsValidDistribution(p_xy));
    Assert.Equal(1.58f, Probability.Entropy(p_x, 2), 2);
    Assert.Equal(1.58f, Probability.Entropy(p_y, 2), 2);
    Assert.Equal(1f, Probability.Entropy(p_x, p_x.Length), 2);
    Assert.Equal(1f, Probability.Entropy(p_y, p_y.Length), 2);
    Assert.Equal(1.1f, Probability.Entropy(p_x), 2);
    Assert.Equal(1.1f, Probability.Entropy(p_y), 2);
    Assert.Equal(3.17f, Probability.JointEntropy(p_xy, 2), 2);
    Assert.Equal(1f, Probability.JointEntropy(p_xy, p_xy.Length), 2);
    Assert.Equal(2.2f, Probability.JointEntropy(p_xy), 2);
    Assert.Equal(1.58f, Probability.ConditionalEntropyYX(p_xy, p_x, 2), 2);
    Assert.Equal(0.5f, Probability.ConditionalEntropyYX(p_xy, p_x, p_xy.Length), 2);
    Assert.Equal(1f, Probability.ConditionalEntropyYX(p_xy, p_x, p_x.Length), 2);
    Assert.Equal(1.1f, Probability.ConditionalEntropyYX(p_xy, p_x), 2);
    Assert.Equal(1.58f, Probability.ConditionalEntropyXY(p_xy, p_y, 2), 2);
    Assert.Equal(1.1f, Probability.ConditionalEntropyXY(p_xy, p_y), 2);

    Assert.Equal(0f, Probability.MutualInformation(p_x, p_y, p_xy, 2), 2);
    Assert.Equal(0f, Probability.MutualInformation(p_x, p_y, p_xy), 2);
    Assert.Equal(0f, Probability.MutualInformation(p_x, p_y, p_xy, p_xy.Length), 2);
    Assert.Equal(0f, Probability.MutualInformation(p_x, p_y, p_xy, p_x.Length), 2);
  }
}
}
