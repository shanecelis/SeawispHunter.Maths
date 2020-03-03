SeawispHunter.Maths
===================

SeawispHunter.Maths is a dotnet standard 2.0 library. I'm not English but I used
the namespace of "Maths" rather than "Math" because I did not want to conflict
with the static class "System.Math".

![H(X) = \sum p(x) \log \frac{1}{p(x)}](https://render.githubusercontent.com/render/math?math=H(X)%20%3D%20%5Csum%20p(x)%20%5Clog%20%5Cfrac%7B1%7D%7Bp(x)%7D)

The reason for this library was to compute Entropy, Conditional Entropy, and
Mutual Information.  

Entropy
-------

![H(X) = \sum p(x) \log \frac{1}{p(x)}](https://render.githubusercontent.com/render/math?math=H(X)%20%3D%20%5Csum%20p(x)%20%5Clog%20%5Cfrac%7B1%7D%7Bp(x)%7D)

Conditional Entropy
-------------------

![H(Y|X) = - \sum p(x, y) \log \frac{p(x, y)}{p(x)}](https://render.githubusercontent.com/render/math?math=H(Y%7CX)%20%3D%20-%20%5Csum%20p(x%2C%20y)%20%5Clog%20%5Cfrac%7Bp(x%2C%20y)%7D%7Bp(x)%7D)

Mutual Information
------------------

![I(X; Y) = H(X, Y) - H(X|Y) - H(Y|X)](https://render.githubusercontent.com/render/math?math=I(X%3B%20Y)%20%3D%20H(X%2C%20Y)%20-%20H(X%7CY)%20-%20H(Y%7CX))


