using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.Repository;
using Promatis.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class ResultsViewModel : BaseViewModel
    {
        public ResultsViewModel(RepositoryContext repo)
        {
            this.repo = repo;

            GetResultCommand = new RelayCommand((arg) =>
            {
                Results = new ObservableCollection<JointResultViewModel>();
                Results.AddRange(repo.LoadResults().Select(r => new JointResultViewModel(r)));
                Console.WriteLine("Results co" + Results.Count);                
                Console.WriteLine("Result torq" + Results.First().FinalTorque);
                OnPropertyChanged(nameof(Results));
            });

        }


        public ObservableCollection<JointResultViewModel> Results { get; set;  }

        JointResultViewModel selectedResult;
        public JointResultViewModel SelectedResult
        {
            get => selectedResult;
            set
            {             
                selectedResult = value;
                OnPropertyChanged(nameof(SelectedResult));
            }

        }
                


        RepositoryContext repo;

        public ICommand GetResultCommand { get; set; }



    }
}
