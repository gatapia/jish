using System.Collections.Generic;

namespace js.net.jish.vsext.Console
{
  internal class InputHistory
  {
    private const int MAX_HISTORY = 50;

    private readonly Queue<string> inputs = new Queue<string>();

    public void Add(string input)
    {
      if (!string.IsNullOrEmpty(input))
      {
        inputs.Enqueue(input);
        if (inputs.Count >= MAX_HISTORY)
        {
          inputs.Dequeue();
        }
      }
    }

    public IList<string> History { get { return inputs.ToArray(); } }
  }
}