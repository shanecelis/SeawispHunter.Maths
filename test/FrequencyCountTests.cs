using System;
using Xunit;
using SeawispHunter.InformationTheory;

namespace test {
public class FrequencyCountTests {
  [Fact]
  public void TestOneSample() {
    var fc = new FrequencyCountSingle(10, -1f, 1f);
    fc.Add(0f);
    Assert.Equal(1f, fc.Probability(0f));
    Assert.Equal(0f, fc.Probability(1f));
    Assert.Equal(0f, fc.Entropy());
  }

  [Fact]
  public void TestOneSampleArray() {
    var fc = new FrequencyArraySingle(10, -1f, 1f);
    fc.Add(new [] { 0f });
    Assert.Equal(new [] { 1f }, fc.Probability(new [] { 0f }));
    Assert.Equal(new [] { 0f }, fc.Probability(new [] { 1f }));
    Assert.Equal(new [] { 0f }, fc.Entropy());
  }

  [Fact]
  public void TestTwoSamples() {
    var fc = new FrequencyCountSingle(10, -1f, 1f);
    fc.Add(0f);
    fc.Add(0.5f);
    Assert.Equal(0.5f, fc.Probability(0f));
    Assert.Equal(0f, fc.Probability(1f));
    Assert.Equal(0.5f, fc.Probability(0.5f));
    Assert.Equal(0.301f, fc.Entropy(), 3);
  }

  [Fact]
  public void TestAlphabet() {
    var fc = new FrequencyCountAlphabet(new[] { "a", "b" });
    fc.Add("a");
    fc.Add("b");
    Assert.Equal(0.5f, fc.Probability("a"));
    Assert.Equal(0.5f, fc.Probability("b"));
    Assert.Equal(1f, fc.Entropy(), 2);
  }

  [Fact]
  public void TestBadAlphabet() {
    var fc = new FrequencyCountAlphabet(new[] { "a", "b" });
    fc.Add("a");
    fc.Add("b");
    Assert.Throws<ArgumentException>(() => fc.Add("c"));
  }


  [Fact]
  public void TestPairedAlphabet() {
    var fc = new FrequencyPairAlphabet(new[] { "a", "b" });
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
    var fc = new FrequencyPairAlphabet(new[] { "a", "b" });
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
    var fc = new FrequencyPairAlphabet(new[] { "a", "b" });
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
}
}
