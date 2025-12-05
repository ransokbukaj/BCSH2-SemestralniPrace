using CommunityToolkit.Mvvm.ComponentModel;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{
    public class SculptureViewModel : ObservableObject
    {
        private SculptureRepository sculptureRepository;
        private CounterRepository counterRepository;
    }
}
