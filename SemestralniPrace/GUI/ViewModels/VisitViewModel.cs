using CommunityToolkit.Mvvm.ComponentModel;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{
    public class VisitViewModel : ObservableObject
    {
        private VisitRepository visitRepository;
        private CounterRepository counterRepository;
    }
}
