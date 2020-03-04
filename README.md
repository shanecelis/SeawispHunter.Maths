SeawispHunter.Maths
===================

SeawispHunter.Maths is a C# dotnet standard 2.0 library. It's not much but it
was something I needed. If you're looking for a real math library with all the
good stuff, see [Math.NET Numerics](https://numerics.mathdotnet.com).

![H(X) = \sum p(x) \log \frac{1}{p(x)}](https://render.githubusercontent.com/render/math?math=H(X)%20%3D%20%5Csum%20p(x)%20%5Clog%20%5Cfrac%7B1%7D%7Bp(x)%7D)

The reason for this library is to compute
[Entropy](https://en.wikipedia.org/wiki/Entropy_(information_theory)),
[Conditional Entropy](https://en.wikipedia.org/wiki/Conditional_entropy), and
[Mutual Information](https://en.wikipedia.org/wiki/Mutual_information). 

Entropy
-------

![H(X) = \sum p(x) \log \frac{1}{p(x)}](https://render.githubusercontent.com/render/math?math=H(X)%20%3D%20%5Csum%20p(x)%20%5Clog%20%5Cfrac%7B1%7D%7Bp(x)%7D)

Where is the minus signs? [Cleaned up for math hygiene reasons.](https://twitter.com/shanecelis/status/1234058415007203328)

Conditional Entropy
-------------------

![H(Y|X) = - \sum p(x, y) \log \frac{p(x, y)}{p(x)}](https://render.githubusercontent.com/render/math?math=H(Y%7CX)%20%3D%20-%20%5Csum%20p(x%2C%20y)%20%5Clog%20%5Cfrac%7Bp(x%2C%20y)%7D%7Bp(x)%7D)

Mutual Information
------------------

![I(X; Y) = H(X, Y) - H(X|Y) - H(Y|X)](https://render.githubusercontent.com/render/math?math=I(X%3B%20Y)%20%3D%20H(X%2C%20Y)%20-%20H(X%7CY)%20-%20H(Y%7CX))

Compute any of the above using the static class `ProbabilityDistribution`. It
expects probability distributions as an array of floats `float[]` and joint
probability distributions as a multidimensional array of floats `float[,]`.

The class `Tally` can create probability distributions from events. Here are a
few examples.

Example 0: Compute Entropy
--------------------------

Suppose I have an artificial neural network that I am trying to analyze. Each
node can produce values of [0, 1]. We can discretize this into 3 different bins.
(Normally I might want more bins maybe 10 but this is easier to follow.)

```cs
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
```


Example 1: Compute Conditional Entropy
--------------------------------------

Suppose I have an artificial neural network with sensors and effectors that I am
want to analyze to determine if knowledge of sensor helps determine the
effector, i.e., the conditional entropy `H(effector|sensor)`. Each node can
produce values of [0, 1). We discretize these into 4 different bins. In this
case, we need to collect two values.

```cs
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
```


Example 2: Compute Conditional Entropy for Arrays
-------------------------------------------------

Suppose the same instance as above but we have two sensors and two effectors and
we want to capture any possible relationships between the two.

```cs
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
float[,] pxy = tally.probabilityXY[1, 0];
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
```

All example code can be run and modified as tests in the `ReadmeTests.cs` file.

License
-------

This project is released under the MIT license.


Note on Namespace
-----------------

I would have liked to use the namespace "SeawispHunter.Math"; however, that
conflicts with the static class "System.Math". Now I'm not English but I guess
University of Sussex exposed me to enough "Maths" that it felt natural enough to
use it here.

Contact
-------

I won't be providing any support for this, but if you want to follow me and the
strange things I make, you can find me on twitter
[@shanecelis](https://twitter.com/shanecelis).
