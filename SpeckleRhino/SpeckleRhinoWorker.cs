using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeckleRhino
{

    /// <summary>
    /// 
    /// </summary>
    public interface ISpeckleRhinoWorker { }

    /// <summary>
    /// Abstract SpeckleRhinoWorker class which contains generic members for both Senders and Receivers.
    /// </summary>
    public abstract class SpeckleRhinoWorker : ISpeckleRhinoWorker
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Uuid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StreamId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SpeckleRhinoWorker()
        {

        }

    }
}
