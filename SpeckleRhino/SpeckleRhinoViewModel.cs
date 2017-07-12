using System;
using System.ComponentModel;

namespace SpeckleRhino
{
    public class SpeckleRhinoViewModel : INotifyPropertyChanged
    {

        public SpeckleRhinoModel Model { get; private set; }

        #region ctor
        public SpeckleRhinoViewModel()
        {
            Model = new SpeckleRhinoModel();
            Model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
        }
        #endregion

        #region Events / Handlers
        public event PropertyChangedEventHandler PropertyChanged;
        private void model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion


    }
}
