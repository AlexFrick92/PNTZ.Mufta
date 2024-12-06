using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.ViewModel
{
    public class JointResultViewModel : BaseViewModel
    {
        JointResult ResultModel { get; set; }

        public JointResultViewModel(JointResult result)
        {
            ResultModel = result;
        }

        public string RecipeName { get => ResultModel.Recipe.Name; }

    }
}
