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
public class TallyTests {
  [Fact]
  public void TestOneSample() {
    var fc = new TallySingle(10, -1f, 1f);
    fc.Add(0f);
    Assert.Equal(1f, fc.Probability(0f));
    Assert.Equal(0f, fc.Probability(1f));
    Assert.Equal(0f, ProbabilityDistribution.Entropy(fc.probability));
  }

  [Fact]
  public void TestOneSampleArray() {
    var fc = new FrequencyArrayTallySingle(10, -1f, 1f);
    fc.Add(new [] { 0f });
    Assert.Equal(new [] { 1f }, fc.Probability(new [] { 0f }));
    Assert.Equal(new [] { 0f }, fc.Probability(new [] { 1f }));
    Assert.Equal(new [] { 0f }, fc.Entropy());
  }

  [Fact]
  public void TestTwoSamples() {
    var fc = new TallySingle(10, -1f, 1f);
    fc.Add(0f);
    fc.Add(0.5f);
    Assert.Equal(0.5f, fc.Probability(0f));
    Assert.Equal(0f, fc.Probability(1f));
    Assert.Equal(0.5f, fc.Probability(0.5f));
    Assert.Equal(0.301f, ProbabilityDistribution.Entropy(fc.probability, fc.binCount), 3);
  }

  [Fact]
  public void TestAlphabet() {
    var fc = new TallyAlphabet(new[] { "a", "b" });
    fc.Add("a");
    fc.Add("b");
    Assert.Equal(0.5f, fc.Probability("a"));
    Assert.Equal(0.5f, fc.Probability("b"));
    Assert.Equal(1f, ProbabilityDistribution.Entropy(fc.probability, fc.binCount), 2);
  }

  [Fact]
  public void TestBadAlphabet() {
    var fc = new TallyAlphabet(new[] { "a", "b" });
    fc.Add("a");
    fc.Add("b");
    Assert.Throws<ArgumentException>(() => fc.Add("c"));
  }

  [Fact]
  public void TestPairedAlphabet() {
    var fc = new TallyAlphabet<int>(new[] { "a", "b" }, 2, y => y);
    fc.Add("a", 0);
    fc.Add("a", 0);
    fc.Add("a", 1);
    fc.Add("a", 1);
    fc.Add("b", 0);
    fc.Add("b", 0);
    fc.Add("b", 0);
    fc.Add("b", 0);
    fc.Add("b", 1);
    Assert.Equal(4f/9, fc.ProbabilityX("a"));
    Assert.Equal(5f/9, fc.ProbabilityX("b"));
    Assert.Equal(6/9f, fc.ProbabilityY(0));
    Assert.Equal(3/9f, fc.ProbabilityY(1));
    Assert.Equal(2/9f, fc.ProbabilityXY("a", 0));
    Assert.Equal(2/4f, fc.ProbabilityYGivenX(0, "a"));
    Assert.Equal(2/4f, fc.ProbabilityYGivenX(1, "a"));
    Assert.Equal(4/5f, fc.ProbabilityYGivenX(0, "b"), 2);
    Assert.Equal(1/5f, fc.ProbabilityYGivenX(1, "b"), 2);
    Assert.Equal(0.8f, fc.EntropyYGivenX(), 1);
    Assert.Equal(0.9f, fc.EntropyXGivenY(), 1);
    Assert.Equal(1.8f, fc.EntropyXY(), 1);
    Assert.Equal(0.99f, fc.EntropyX(), 2);
    Assert.Equal(0.9f, fc.EntropyY(), 1);
    Assert.Equal(0f, fc.EntropyXGivenY() - fc.EntropyXY() + fc.EntropyY(), 1);
    Assert.Equal(0f, fc.EntropyYGivenX() - fc.EntropyXY() + fc.EntropyX(), 1);
    Assert.Equal(0.07f, fc.MutualInformationXY(), 2);
  }

  [Fact]
  public void TestPairedAlphabet2() {
    var fc = new TallyAlphabet<int>(new[] { "a", "b" }, 2, y => y);
    fc.Add("a", 1);
    fc.Add("a", 1);
    fc.Add("a", 1);
    fc.Add("a", 1);
    fc.Add("b", 0);
    fc.Add("b", 0);
    fc.Add("b", 0);
    fc.Add("b", 0);
    Assert.Equal(1/2f, fc.ProbabilityX("a"));
    Assert.Equal(1/2f, fc.ProbabilityX("b"));
    Assert.Equal(1/2f, fc.ProbabilityY(0));
    Assert.Equal(1/2f, fc.ProbabilityY(1));
    Assert.Equal(0f, fc.ProbabilityXY("a", 0));
    Assert.Equal(0f, fc.ProbabilityYGivenX(0, "a"));
    Assert.Equal(1f, fc.ProbabilityYGivenX(1, "a"));
    Assert.Equal(1f, fc.ProbabilityYGivenX(0, "b"), 2);
    Assert.Equal(0f, fc.ProbabilityYGivenX(1, "b"), 2);
    Assert.Equal(0f, fc.EntropyYGivenX(), 1);
    Assert.Equal(0f, fc.EntropyXGivenY(), 1);
    Assert.Equal(1f, fc.EntropyXY(), 1);
    Assert.Equal(1f, fc.EntropyX(), 2);
    Assert.Equal(1f, fc.EntropyY(), 1);
    Assert.Equal(0f, fc.EntropyXGivenY() - fc.EntropyXY() + fc.EntropyY(), 1);
    Assert.Equal(0f, fc.EntropyYGivenX() - fc.EntropyXY() + fc.EntropyX(), 1);
    Assert.Equal(1f, fc.MutualInformationXY(), 2);
  }

  [Fact]
  public void TestPairedAlphabet3() {
    var fc = new TallyAlphabet<int>(new[] { "a", "b" }, 2, y => y);
    fc.Add("a", 1);
    fc.Add("a", 1);
    fc.Add("a", 0);
    fc.Add("a", 0);
    fc.Add("b", 1);
    fc.Add("b", 1);
    fc.Add("b", 0);
    fc.Add("b", 0);
    Assert.Equal(1/2f, fc.ProbabilityX("a"));
    Assert.Equal(1/2f, fc.ProbabilityX("b"));
    Assert.Equal(1/2f, fc.ProbabilityY(0));
    Assert.Equal(1/2f, fc.ProbabilityY(1));
    Assert.Equal(2/8f, fc.ProbabilityXY("a", 0));
    Assert.Equal(1/2f, fc.ProbabilityYGivenX(0, "a"));
    Assert.Equal(1/2f, fc.ProbabilityYGivenX(1, "a"));
    Assert.Equal(1/2f, fc.ProbabilityYGivenX(0, "b"), 2);
    Assert.Equal(1/2f, fc.ProbabilityYGivenX(1, "b"), 2);
    Assert.Equal(1f, fc.EntropyYGivenX(), 1);
    Assert.Equal(1f, fc.EntropyXGivenY(), 1);
    Assert.Equal(2f, fc.EntropyXY(), 1);
    Assert.Equal(1f, fc.EntropyX(), 2);
    Assert.Equal(1f, fc.EntropyY(), 1);
    // H(X|Y) = H(X,Y) - H(Y)
    // This should always be true.
    Assert.Equal(0f, fc.EntropyXGivenY() - fc.EntropyXY() + fc.EntropyY(), 1);
    Assert.Equal(0f, fc.EntropyYGivenX() - fc.EntropyXY() + fc.EntropyX(), 1);
    Assert.Equal(0f, fc.MutualInformationXY(), 2);
  }

  [Fact]
  public void TestArrayTallyAlphabet() {
    var fc = new ArrayTallyAlphabet<int>(new[] { "a", "b", "c" }, 3, y => y);
    fc.Add(new [] { "a" }, new [] { 2 });
    fc.Add(new [] { "a" }, new [] { 1 });
    fc.Add(new [] { "a" }, new [] { 0 });
    fc.Add(new [] { "b" }, new [] { 2 });
    fc.Add(new [] { "b" }, new [] { 1 });
    fc.Add(new [] { "b" }, new [] { 0 });
    fc.Add(new [] { "c" }, new [] { 2 });
    fc.Add(new [] { "c" }, new [] { 1 });
    fc.Add(new [] { "c" }, new [] { 0 });
    Assert.Equal(9, fc.probabilityXY[0,0].Length);
    Assert.Equal(new [] { 1/3f, 1/3f, 1/3f }, fc.probabilityX[0]);
    Assert.Equal(new [] { 1/3f, 1/3f, 1/3f }, fc.probabilityY[0]);
    Assert.Equal(new [,] {
      {1/9f, 1/9f, 1/9f},
      {1/9f, 1/9f, 1/9f},
      {1/9f, 1/9f, 1/9f},
    }, fc.probabilityXY[0, 0]);

    Assert.Equal(1/2f, ProbabilityDistribution.ConditionalEntropyYX(fc.probabilityXY[0,0], fc.probabilityX[0], fc.probabilityXY[0,0].Length));
    Assert.Equal(1f, ProbabilityDistribution.ConditionalEntropyYX(fc.probabilityXY[0,0], fc.probabilityX[0], fc.probabilityX[0].Length));
    Assert.Equal(1/2f, ProbabilityDistribution.ConditionalEntropyXY(fc.probabilityXY[0,0], fc.probabilityY[0], fc.probabilityXY[0,0].Length));
    Assert.Equal(1f, ProbabilityDistribution.ConditionalEntropyXY(fc.probabilityXY[0,0], fc.probabilityY[0], fc.probabilityY[0].Length));
    Assert.Equal(2f, ProbabilityDistribution.JointEntropy(fc.probabilityXY[0,0], fc.probabilityX[0].Length), 1);
    Assert.Equal(1f, ProbabilityDistribution.JointEntropy(fc.probabilityXY[0,0], fc.probabilityXY[0,0].Length), 1);
    Assert.Equal(3.2f, ProbabilityDistribution.JointEntropy(fc.probabilityXY[0,0], 2), 1);
    Assert.Equal(1f, ProbabilityDistribution.Entropy(fc.probabilityX[0], fc.probabilityX[0].Length), 2);
    Assert.Equal(0.5f, ProbabilityDistribution.Entropy(fc.probabilityX[0], fc.probabilityXY[0,0].Length), 2);
    Assert.Equal(1f, ProbabilityDistribution.Entropy(fc.probabilityY[0], fc.probabilityY[0].Length), 2);
    Assert.Equal(0.5f, ProbabilityDistribution.Entropy(fc.probabilityY[0], fc.probabilityXY[0,0].Length), 2);
    // H(X|Y) = H(X,Y) - H(Y)
    // This should always be true.
    Assert.Equal(0f, ProbabilityDistribution.ConditionalEntropyXY(fc.probabilityXY[0,0], fc.probabilityY[0])
                     - ProbabilityDistribution.JointEntropy(fc.probabilityXY[0,0])
                     + ProbabilityDistribution.ConditionalEntropyXY(fc.probabilityXY[0,0], fc.probabilityY[0]), 1);
  }

  [Fact]
  public void TestArrayTallyAlphabet2() {
    var fc = new ArrayTallyAlphabet<int>(new[] { "a", "b", "c" }, 3, y => y);
    fc.Add(new [] { "a", "b" }, new [] { 2, 1 });
    fc.Add(new [] { "a", "b" }, new [] { 1, 1 });
    fc.Add(new [] { "a", "b" }, new [] { 0, 1 });
    fc.Add(new [] { "b", "b" }, new [] { 2, 1 });
    fc.Add(new [] { "b", "b" }, new [] { 1, 1 });
    fc.Add(new [] { "b", "b" }, new [] { 0, 1 });
    fc.Add(new [] { "c", "b" }, new [] { 2, 1 });
    fc.Add(new [] { "c", "b" }, new [] { 1, 1 });
    fc.Add(new [] { "c", "b" }, new [] { 0, 1 });

    Assert.Equal(9, fc.probabilityXY[0,0].Length);
    Assert.Equal(new [] { 1/3f, 1/3f, 1/3f }, fc.probabilityX[0]);
    Assert.Equal(new [,] {
      {1/9f, 1/9f, 1/9f },
      {1/9f, 1/9f, 1/9f },
      {1/9f, 1/9f, 1/9f },
    }, fc.probabilityXY[0, 0]);
    Assert.Equal(new [,] {
      {0f, 0f, 0f },
      {1/3f, 1/3f, 1/3f },
      {0f, 0f, 0f },
    }, fc.probabilityXY[1, 0]);

    Assert.Equal(new [,] {
      {0f, 0f, 0f },
      {0f, 1f, 0f },
      {0f, 0f, 0f },
    }, fc.probabilityXY[1, 1]);
    Assert.Equal(new [] { 1/3f, 1/3f, 1/3f }, ProbabilityDistribution.MarginalY(fc.probabilityXY[0, 0]));
    Assert.Equal(new [] { 1/3f, 1/3f, 1/3f }, ProbabilityDistribution.MarginalY(fc.probabilityXY[1, 0]));
    Assert.Equal(new [] { 0f, 1f, 0f }, ProbabilityDistribution.MarginalX(fc.probabilityXY[1, 0]));
    Assert.Equal(new [] { 0f, 1f, 0f }, fc.probabilityX[1]);
    Assert.Equal(new [] { 1/3f, 1/3f, 1/3f }, fc.probabilityY[0]);
    Assert.Equal(new [] { 0f, 1f, 0f }, fc.probabilityY[1]);
    Assert.Equal(1/2f, ProbabilityDistribution.ConditionalEntropyYX(fc.probabilityXY[0,0], fc.probabilityX[0], fc.probabilityXY[0,0].Length));
    Assert.Equal(1f, ProbabilityDistribution.ConditionalEntropyYX(fc.probabilityXY[0,0], fc.probabilityX[0], fc.probabilityX[0].Length));
    Assert.Equal(1/2f, ProbabilityDistribution.ConditionalEntropyXY(fc.probabilityXY[0,0], fc.probabilityY[0], fc.probabilityXY[0,0].Length));
    Assert.Equal(1f, ProbabilityDistribution.ConditionalEntropyXY(fc.probabilityXY[0,0], fc.probabilityY[0], fc.probabilityY[0].Length));
    Assert.Equal(2f, ProbabilityDistribution.JointEntropy(fc.probabilityXY[0,0], fc.probabilityX[0].Length), 1);
    Assert.Equal(1f, ProbabilityDistribution.JointEntropy(fc.probabilityXY[0,0], fc.probabilityXY[0,0].Length), 1);
    Assert.Equal(3.2f, ProbabilityDistribution.JointEntropy(fc.probabilityXY[0,0], 2), 1);
    Assert.Equal(1f, ProbabilityDistribution.Entropy(fc.probabilityX[0], fc.probabilityX[0].Length), 2);
    Assert.Equal(0.5f, ProbabilityDistribution.Entropy(fc.probabilityX[0], fc.probabilityXY[0,0].Length), 2);
    Assert.Equal(1f, ProbabilityDistribution.Entropy(fc.probabilityY[0], fc.probabilityY[0].Length), 2);
    Assert.Equal(0.5f, ProbabilityDistribution.Entropy(fc.probabilityY[0], fc.probabilityXY[0,0].Length), 2);
    // H(X|Y) = H(X,Y) - H(Y)
    // This should always be true.
    Assert.Equal(0f, ProbabilityDistribution.ConditionalEntropyXY(fc.probabilityXY[0,0], fc.probabilityY[0])
                     - ProbabilityDistribution.JointEntropy(fc.probabilityXY[0,0])
                     + ProbabilityDistribution.ConditionalEntropyXY(fc.probabilityXY[0,0], fc.probabilityY[0]), 1);
  }


  [Fact]
  public void TestCompareWithProbability() {
    var fc = new TallyAlphabet<int>(new[] { "a", "b", "c" }, 3, y => y);
    fc.Add("a", 2);
    fc.Add("a", 1);
    fc.Add("a", 0);
    fc.Add("b", 2);
    fc.Add("b", 1);
    fc.Add("b", 0);
    fc.Add("c", 2);
    fc.Add("c", 1);
    fc.Add("c", 0);
    Assert.Equal(1/3f, fc.ProbabilityX("a"), 2);
    Assert.Equal(1/3f, fc.ProbabilityX("b"), 2);
    Assert.Equal(1/3f, fc.ProbabilityX("c"), 2);
    Assert.Equal(1/3f, fc.ProbabilityY(0));
    Assert.Equal(1/3f, fc.ProbabilityY(1));
    Assert.Equal(1/9f, fc.ProbabilityXY("a", 0));
    Assert.Equal(1/3f, fc.ProbabilityYGivenX(0, "a"), 2);
    Assert.Equal(1/3f, fc.ProbabilityYGivenX(1, "a"), 2);
    Assert.Equal(1/3f, fc.ProbabilityYGivenX(0, "b"), 2);
    Assert.Equal(1/3f, fc.ProbabilityYGivenX(1, "b"), 2);
    Assert.Equal(1f, fc.EntropyYGivenX(), 1);

    Assert.Equal(9, fc.probabilityXY.Length);
    Assert.Equal(new [] { 1/3f, 1/3f, 1/3f }, fc.probabilityX);
    Assert.Equal(new [] { 1/3f, 1/3f, 1/3f }, fc.probabilityY);
    Assert.Equal(new [,] {
      {1/9f, 1/9f, 1/9f},
      {1/9f, 1/9f, 1/9f},
      {1/9f, 1/9f, 1/9f},
    }, fc.probabilityXY);

    Assert.Equal(1/2f, ProbabilityDistribution.ConditionalEntropyYX(fc.probabilityXY, fc.probabilityX, fc.probabilityXY.Length));
    Assert.Equal(1f, ProbabilityDistribution.ConditionalEntropyYX(fc.probabilityXY, fc.probabilityX, fc.probabilityX.Length));
    Assert.Equal(1f, fc.EntropyXGivenY(), 1);
    Assert.Equal(1/2f, ProbabilityDistribution.ConditionalEntropyXY(fc.probabilityXY, fc.probabilityY, fc.probabilityXY.Length));
    Assert.Equal(1f, ProbabilityDistribution.ConditionalEntropyXY(fc.probabilityXY, fc.probabilityY, fc.probabilityY.Length));
    Assert.Equal(2f, fc.EntropyXY(), 1);
    Assert.Equal(2f, ProbabilityDistribution.JointEntropy(fc.probabilityXY, fc.probabilityX.Length), 1);
    Assert.Equal(1f, ProbabilityDistribution.JointEntropy(fc.probabilityXY, fc.probabilityXY.Length), 1);
    Assert.Equal(3.2f, ProbabilityDistribution.JointEntropy(fc.probabilityXY, 2), 1);
    Assert.Equal(1f, fc.EntropyX(), 2);
    Assert.Equal(1f, ProbabilityDistribution.Entropy(fc.probabilityX, fc.probabilityX.Length), 2);
    Assert.Equal(0.5f, ProbabilityDistribution.Entropy(fc.probabilityX, fc.probabilityXY.Length), 2);
    Assert.Equal(1f, fc.EntropyY(), 1);
    Assert.Equal(1f, ProbabilityDistribution.Entropy(fc.probabilityY, fc.probabilityY.Length), 2);
    Assert.Equal(0.5f, ProbabilityDistribution.Entropy(fc.probabilityY, fc.probabilityXY.Length), 2);
    // H(X|Y) = H(X,Y) - H(Y)
    // This should always be true.
    Assert.Equal(0f, fc.EntropyXGivenY() - fc.EntropyXY() + fc.EntropyY(), 1);
    Assert.Equal(0f, ProbabilityDistribution.ConditionalEntropyXY(fc.probabilityXY, fc.probabilityY)
                     - ProbabilityDistribution.JointEntropy(fc.probabilityXY)
                     + ProbabilityDistribution.ConditionalEntropyXY(fc.probabilityXY, fc.probabilityY), 1);
    Assert.Equal(0f, fc.MutualInformationXY(), 2);
  }
}
}
