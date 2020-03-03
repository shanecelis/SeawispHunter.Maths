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
