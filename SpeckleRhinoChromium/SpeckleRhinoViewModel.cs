using System;
using System.ComponentModel;

namespace SpeckleRhino
{
  public class SpeckleRhinoViewModel : INotifyPropertyChanged
  {

    #region ctor
    public SpeckleRhinoViewModel()
    {
      SpeckleRhinoModel model = new SpeckleRhinoModel();
      model.PropertyChanged += new PropertyChangedEventHandler(model_PropertyChanged);
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
