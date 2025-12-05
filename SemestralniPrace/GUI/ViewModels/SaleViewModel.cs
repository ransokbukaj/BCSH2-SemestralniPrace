using CommunityToolkit.Mvvm.ComponentModel;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.ViewModels
{
    public class SaleViewModel : ObservableObject
    {
        private SaleRepository saleRepository;
        private CounterRepository counterRepository;
    }
}
