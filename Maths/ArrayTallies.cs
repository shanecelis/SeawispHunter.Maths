/* Original code[1] Copyright (c) 2020 Shane Celis[2]
   Licensed under the MIT License[3]

   [1]: https://github.com/shanecelis/SeawispHunter.Maths
   [2]: https://twitter.com/shanecelis
   [3]: https://opensource.org/licenses/MIT
*/

using System;

namespace SeawispHunter.Maths {

public class ArrayTallySingle : ArrayTally<float, float> {

  public ArrayTallySingle(int binCount, float min, float max)
    : base(binCount, x => (int) ((Clamp(x, min, max) - min) / (max - min) * (binCount - 1)),
           binCount, y => (int) ((Clamp(y, min, max) - min) / (max - min) * (binCount - 1))) { }

  static float Clamp(float x, float min, float max) => System.Math.Min(System.Math.Max(x, min), max);

}

public class ArrayTallyAlphabet<Y> : ArrayTally<string, Y> {

  public ArrayTallyAlphabet(string[] alphabet,
                            int binCountY, Func<Y, int> binY)
    : base(alphabet.Length, x => Array.IndexOf(alphabet, x),
           binCountY, binY) { }
}


public class TallySingle : Tally<float> {

  public TallySingle(int binCount, float min, float max)
    : base(binCount, x => (int) ((Clamp(x, min, max) - min) / (max - min) * (binCount - 1))) { }

  public static float Clamp(float x, float min, float max) => System.Math.Min(System.Math.Max(x, min), max);

}

public class TallyAlphabet : Tally<string> {
  public TallyAlphabet(string[] alphabet)
    : base(alphabet.Length, x => Array.IndexOf(alphabet, x)) { }
}


public class TallyAlphabet<Y> : Tally<string, Y> {
  public TallyAlphabet(string[] xalphabet, int binCountY, Func<Y, int> binY)
    : base(xalphabet.Length,
           x => Array.IndexOf(xalphabet, x),
           binCountY,
           binY) { }
}


}
