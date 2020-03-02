using System;
using Xunit;
using SeawispHunter.InformationTheory;

namespace test {
public class ProbabilityDistributionTests {
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
    p_x = ProbabilityDistribution.MarginalX(p_xy);
    p_y = ProbabilityDistribution.MarginalY(p_xy);
    Assert.True(ProbabilityDistribution.IsValid(p_x));
    Assert.True(ProbabilityDistribution.IsValid(p_y));
    Assert.True(ProbabilityDistribution.IsValid(p_xy));
    Assert.Equal(1/8f, p_xy[0, 0]);
    Assert.Equal(1/16f, p_xy[1, 0]);
    Assert.Equal(1/16f, p_xy[2, 0]);
    // Assert.Equal(1/8f, p_xy[0]);
    // Assert.Equal(1/16f, p_xy[1]);
    // Assert.Equal(1/32f, p_xy[2]);
    Assert.Equal(new [] { 1/4f, 1/4f, 1/4f, 1/4f }, p_x);
    Assert.Equal(new [] { 1/2f, 1/4f, 1/8f, 1/8f }, p_y);
    Assert.Equal(2f, ProbabilityDistribution.Entropy(p_x, 2));
    Assert.Equal(7/4f, ProbabilityDistribution.Entropy(p_y, 2));
    Assert.Equal(27/8f, ProbabilityDistribution.JointEntropy(p_xy, 2));
    Assert.Equal(11/8f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, 2));
    Assert.Equal(3/8f, ProbabilityDistribution.MutualInformation(p_x, p_y, p_xy, 2));
  }

  /** http://www.cs.tau.ac.il/~iftachh/Courses/Info/Fall14/Printouts/Lesson2_h.pdf */
  [Fact]
  public void TestExample() {
    p_xy = new [,] {
      { 1/4f, 1/4f },
      { 1/2f, 0f }
    };
    p_x = ProbabilityDistribution.MarginalX(p_xy);
    p_y = ProbabilityDistribution.MarginalY(p_xy);
    Assert.True(ProbabilityDistribution.IsValid(p_x));
    Assert.True(ProbabilityDistribution.IsValid(p_y));
    Assert.True(ProbabilityDistribution.IsValid(p_xy));
    // Assert.Equal(2f, ProbabilityDistribution.Entropy(p_x, 2));
    // Assert.Equal(7/4f, ProbabilityDistribution.Entropy(p_y, 2));
    Assert.Equal(3/2f, ProbabilityDistribution.JointEntropy(p_xy, 2));
    Assert.Equal(3/4f, ProbabilityDistribution.JointEntropy(p_xy, p_xy.Length));
    Assert.Equal(1.04f, ProbabilityDistribution.JointEntropy(p_xy), 2);
    Assert.Equal(1/2f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, 2));
    Assert.NotEqual(1/4f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x));
    Assert.Equal(1/4f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, p_xy.Length), 2);
    Assert.Equal(1/2f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, p_x.Length), 2);
    Assert.Equal(0.35f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x), 2);
    Assert.Equal(0.29f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_y, 2), 2);
    Assert.Equal(0.2f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_y), 2);
    // Assert.Equal(3/8f, ProbabilityDistribution.MutualInformation(p_x, p_y, p_xy, 2));
  }

  [Fact]
  public void TestExample2() {
    p_xy = new [,] {
      { 1/4f, 1/4f, 0f },
      { 1/2f, 0f, 0f },
      { 0f, 0f, 0f }
    };
    p_x = ProbabilityDistribution.MarginalX(p_xy);
    p_y = ProbabilityDistribution.MarginalY(p_xy);
    // Assert.Equal(2f, ProbabilityDistribution.Entropy(p_x, 2));
    // Assert.Equal(7/4f, ProbabilityDistribution.Entropy(p_y, 2));
    Assert.Equal(3/2f, ProbabilityDistribution.JointEntropy(p_xy, 2));
    Assert.Equal(0.47f, ProbabilityDistribution.JointEntropy(p_xy, p_xy.Length), 2);
    Assert.Equal(1.04f, ProbabilityDistribution.JointEntropy(p_xy), 2);
    Assert.Equal(1/2f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, 2));
    Assert.Equal(0.16f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, p_xy.Length), 2);
    Assert.Equal(0.32f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, p_x.Length), 2);
    Assert.Equal(0.35f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x), 2);
    Assert.Equal(0.69f, ProbabilityDistribution.ConditionalEntropyXY(p_xy, p_y, 2), 2);
    Assert.Equal(0.22f, ProbabilityDistribution.ConditionalEntropyXY(p_xy, p_y, p_xy.Length), 2);
    Assert.Equal(0.43f, ProbabilityDistribution.ConditionalEntropyXY(p_xy, p_y, p_y.Length), 2);
    Assert.Equal(0.48f, ProbabilityDistribution.ConditionalEntropyXY(p_xy, p_y), 2);
    // Assert.Equal(3/8f, ProbabilityDistribution.MutualInformation(p_x, p_y, p_xy, 2));
  }


  [Fact]
  public void TestTwoIndependentCoinFlipsBadSetup() {
    p_xy = new [,] {
      { 1/2f, 1/2f },
      { 1/2f, 1/2f }
    };
    p_x = ProbabilityDistribution.MarginalX(p_xy);
    p_y = ProbabilityDistribution.MarginalY(p_xy);
    Assert.False(ProbabilityDistribution.IsValid(p_x));
    Assert.False(ProbabilityDistribution.IsValid(p_y));
    Assert.False(ProbabilityDistribution.IsValid(p_xy));
    ProbabilityDistribution.Normalize(p_x);
    ProbabilityDistribution.Normalize(p_y);
    ProbabilityDistribution.Normalize(p_xy);
    Assert.True(ProbabilityDistribution.IsValid(p_x));
    Assert.True(ProbabilityDistribution.IsValid(p_y));
    Assert.True(ProbabilityDistribution.IsValid(p_xy));
  }

  [Fact]
  public void TestTwoIndependentCoinFlips() {
    p_xy = new [,] {
      { 1/4f, 1/4f },
      { 1/4f, 1/4f }
    };
    p_x = ProbabilityDistribution.MarginalX(p_xy);
    p_y = ProbabilityDistribution.MarginalY(p_xy);
    Assert.True(ProbabilityDistribution.IsValid(p_x));
    Assert.True(ProbabilityDistribution.IsValid(p_y));
    Assert.True(ProbabilityDistribution.IsValid(p_xy));
    Assert.Equal(1f, ProbabilityDistribution.Entropy(p_x, 2), 2);
    Assert.Equal(1f, ProbabilityDistribution.Entropy(p_y, 2), 2);
    Assert.Equal(1f, ProbabilityDistribution.Entropy(p_x, p_x.Length), 2);
    Assert.Equal(1f, ProbabilityDistribution.Entropy(p_y, p_y.Length), 2);
    Assert.Equal(0.69f, ProbabilityDistribution.Entropy(p_x), 2);
    Assert.Equal(0.69f, ProbabilityDistribution.Entropy(p_y), 2);
    Assert.Equal(2f, ProbabilityDistribution.JointEntropy(p_xy, 2), 2);
    Assert.Equal(1f, ProbabilityDistribution.JointEntropy(p_xy, p_xy.Length), 2);
    Assert.Equal(1.39f, ProbabilityDistribution.JointEntropy(p_xy), 2);
    Assert.Equal(1f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, 2), 2);
    Assert.Equal(1/2f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, p_xy.Length), 2);
    Assert.Equal(1f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, p_x.Length), 2);
    Assert.Equal(0.69f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x), 2);
    Assert.Equal(1f, ProbabilityDistribution.ConditionalEntropyXY(p_xy, p_y, 2), 2);
    Assert.Equal(0.69f, ProbabilityDistribution.ConditionalEntropyXY(p_xy, p_y), 2);

    Assert.Equal(0f, ProbabilityDistribution.MutualInformation(p_x, p_y, p_xy, 2), 2);
    Assert.Equal(0f, ProbabilityDistribution.MutualInformation(p_x, p_y, p_xy), 2);
  }

  [Fact]
  public void TestThreeIndependentCoinFlips() {
    p_xy = new [,] {
      { 1/9f, 1/9f, 1/9f },
      { 1/9f, 1/9f, 1/9f },
      { 1/9f, 1/9f, 1/9f }
    };
    p_x = ProbabilityDistribution.MarginalX(p_xy);
    p_y = ProbabilityDistribution.MarginalY(p_xy);
    Assert.True(ProbabilityDistribution.IsValid(p_x));
    Assert.True(ProbabilityDistribution.IsValid(p_y));
    Assert.True(ProbabilityDistribution.IsValid(p_xy));
    Assert.Equal(1.58f, ProbabilityDistribution.Entropy(p_x, 2), 2);
    Assert.Equal(1.58f, ProbabilityDistribution.Entropy(p_y, 2), 2);
    Assert.Equal(1f, ProbabilityDistribution.Entropy(p_x, p_x.Length), 2);
    Assert.Equal(1f, ProbabilityDistribution.Entropy(p_y, p_y.Length), 2);
    Assert.Equal(1.1f, ProbabilityDistribution.Entropy(p_x), 2);
    Assert.Equal(1.1f, ProbabilityDistribution.Entropy(p_y), 2);
    Assert.Equal(3.17f, ProbabilityDistribution.JointEntropy(p_xy, 2), 2);
    Assert.Equal(1f, ProbabilityDistribution.JointEntropy(p_xy, p_xy.Length), 2);
    Assert.Equal(2.2f, ProbabilityDistribution.JointEntropy(p_xy), 2);
    Assert.Equal(1.58f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, 2), 2);
    Assert.Equal(0.5f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, p_xy.Length), 2);
    Assert.Equal(1f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x, p_x.Length), 2);
    Assert.Equal(1.1f, ProbabilityDistribution.ConditionalEntropyYX(p_xy, p_x), 2);
    Assert.Equal(1.58f, ProbabilityDistribution.ConditionalEntropyXY(p_xy, p_y, 2), 2);
    Assert.Equal(1.1f, ProbabilityDistribution.ConditionalEntropyXY(p_xy, p_y), 2);

    Assert.Equal(0f, ProbabilityDistribution.MutualInformation(p_x, p_y, p_xy, 2), 2);
    Assert.Equal(0f, ProbabilityDistribution.MutualInformation(p_x, p_y, p_xy), 2);
    Assert.Equal(0f, ProbabilityDistribution.MutualInformation(p_x, p_y, p_xy, p_xy.Length), 2);
    Assert.Equal(0f, ProbabilityDistribution.MutualInformation(p_x, p_y, p_xy, p_x.Length), 2);
  }
}
}
