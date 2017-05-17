using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeckleRhino
{

  public interface ISpeckleRhinoWorker { }
  public class SpeckleRhinoWorker : ISpeckleRhinoWorker
  {

    public Guid Uuid { get; private set; }

    public string Type { get; set; }

    public SpeckleRhinoWorker()
    {
      Uuid = Guid.NewGuid();
    }

  }
}
