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
}
}
