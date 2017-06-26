using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeckleRhino
{
    public class SpeckleRhinoSenderWorker : SpeckleRhinoWorker, IEquatable<SpeckleRhinoReceiverWorker>, INotifyPropertyChanged, IDisposable
    {
        #region Members

        /// <summary>
        /// The streamId for the sender.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// The stream Name.
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Constructors

        public SpeckleRhinoSenderWorker()
        {
            Type = "Sender";
        }

        public SpeckleRhinoSenderWorker(string id, string name) : this()
        {
            Id = id;
            Name = name;
        }

        #endregion

        #region Methods

        public bool Equals(SpeckleRhinoReceiverWorker other)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Events / Event Handlers

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

    }

    /// <summary>
    /// Collection class for anything related to handling comparison of SpeckleRhinoSenderrWorkers
    /// </summary>
    public class SpeckleRhinoSenderWorkerCollection : Collection<SpeckleRhinoSenderWorker> { }
}
