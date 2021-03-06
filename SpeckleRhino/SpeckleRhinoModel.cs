﻿using System.ComponentModel;

namespace SpeckleRhino
{
    public class SpeckleRhinoModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public SpeckleRhinoReceiverWorkerCollection Receivers { get; set; }

        public SpeckleRhinoSenderWorkerCollection Senders { get; set; }

        public SpeckleRhinoModel()
        {
            Receivers = new SpeckleRhinoReceiverWorkerCollection();
            Senders = new SpeckleRhinoSenderWorkerCollection();
        }
    }
}