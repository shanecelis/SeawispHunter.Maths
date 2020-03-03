SeawispHunter.Maths
===================

SeawispHunter.Maths is a C# dotnet standard 2.0 library. It's not much but it
was something I needed. If you're looking for a real math library with all the
good stuff, see [Math.NET Numerics](https://numerics.mathdotnet.com).


![H(X) = \sum p(x) \log \frac{1}{p(x)}](https://render.githubusercontent.com/render/math?math=H(X)%20%3D%20%5Csum%20p(x)%20%5Clog%20%5Cfrac%7B1%7D%7Bp(x)%7D)

The reason for this library was to compute Entropy, Conditional Entropy, and
Mutual Information. These are all available in the static class
`ProbabilityDistribution`. It expects probability distributions as an array of
floats `float[]` and joint probability distributions as `float[,]`.

The class `Tally` allows one to create probability distributions from events
that have been seen. 

Example 0: Compute Entropy
--------------------------

Suppose I have an artificial neural network that I am trying to analyze. Each
node can produce values of [0, 1].  We can discretize this into 10 different bins.

```cs
int binCount = 10;
Tally<float> tally = new Tally<float>(binCount, x => (int) (x * (binCount - 1)));

// Some where, this is called repeatedly.
// tally.Add(neuron.value);
// But let's supply some fake values for demonstration purposes.
tally.Add(0.3f);
tally.Add(0.1f);
tally.Add(0.4f);
// ...

// Finally we analyze it.
float[] p = tally.probability;
float H = ProbabilityDistribution.Entropy(p);
Assert.Equal(1.1f, H, 1);
```


Example 1: Compute Joint Entropy
--------------------------------

Suppose I have an artificial neural network with sensors and effectors that I am
want to analyze to determine if knowledge of sensor helps determine the
effector, i.e., the conditional entropy `H(effector|sensor)`. Each node can produce values of [0,
1]. We discretize these into 10 different bins. In this case, we need to collect
two values.

```cs
int binCount = 10;
Tally<float, float> tally 
  = new Tally<float, float>(binCount, x => (int) (x * (binCount - 1)),
                            binCount, y => (int) (y * (binCount - 1)));

// Some where, this is called repeatedly.
// tally.Add(sensor.value, effector.value);
// But let's supply some fake values for demonstration purposes.
tally.Add(0.3f, 0.1f);
tally.Add(0.1f, 0.5f);
tally.Add(0.4f, 0.9f);
// ...

// Finally we analyze it.
float[,] pxy = tally.probabilityXY;
float[] px = tally.probabilityX;
float H = ProbabilityDistribution.ConditionalEntropy(pxy, px);
Assert.Equal(1.1f, H, 1);
```


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
