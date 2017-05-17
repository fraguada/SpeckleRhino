using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeckleRhino
{
  /// <summary>
  /// SpeckleRhinoReveiverWorker
  /// This is a class which acts on behalf of the js receiver.  
  /// It does the work of doing things to Rhino such as adding geometry to the document,
  /// keeping track of geometry, etc.
  /// </summary>
  public class SpeckleRhinoReceiverWorker: SpeckleRhinoWorker, IEquatable<SpeckleRhinoReceiverWorker>, INotifyPropertyChanged
  {

    #region Members
    /// <summary>
    /// The Id for the receiver.  This can be the tream id, or something else identifyable that
    /// connects this to the js receiver.
    /// </summary>
    public string Id { get; private set; }

    //Will this class hold a collection of uuids to track geometry in the document?

    //Will this class do the conduit work as well?

    //Need to develop the events/handlers to communicate with Cef

    #endregion Members

    #region Constructors

    public SpeckleRhinoReceiverWorker()
    {
      Type = "Receiver";
    }

    public SpeckleRhinoReceiverWorker(string id) : this()
    {
      Id = id;
    }

    

    #endregion Constructors

    #region Methods

    public bool Equals(SpeckleRhinoReceiverWorker other)
    {
      throw new NotImplementedException();
    }

    #endregion Methods

    #region Events / Event Handlers

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events / Event Handlers

  }

  /// <summary>
  /// Collection class for anything related to handling comparison of SpeckleRhinoReceiverWorkers
  /// </summary>
  public class SpeckleRhinoReceiverWorkerCollection : Collection<SpeckleRhinoReceiverWorker> { }
}
