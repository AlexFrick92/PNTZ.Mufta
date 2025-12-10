using Desktop.MVVM;
using PNTZ.Mufta.TPCApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PNTZ.Mufta.TPCApp.ViewModel.Joint
{
    public class JointProcessDataViewModel : BaseViewModel
    {
        //Текущие показания датчиков
        private TqTnLenPoint _actualPoint;
        public TqTnLenPoint ActualPoint
        {
            get { return _actualPoint; }
            set { _actualPoint = value; OnPropertyChanged(nameof(ActualPoint)); }
        }
        //Время в секундах с начала процесса стыковки
        private int _secondsFromBeginJointing;
        public int SecondsFromBeginJointing
        {
            get { return _secondsFromBeginJointing; }
            set { _secondsFromBeginJointing = value; OnPropertyChanged(nameof(SecondsFromBeginJointing)); }
        }

        private DispatcherTimer _timer;
        private DateTime _jointingStartTime;

        public void UpdateRecipe(JointRecipe jointRecipe)
        {
        }
        public void BeginNewJointing()
        {
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += UpdateTimer_Tick;
            _jointingStartTime = DateTime.Now;
            SecondsFromBeginJointing = 0;
            _timer.Start();
        }
        public void FinishJointing(JointResult jointResult)
        {
            _timer.Stop();
            _timer.Tick -= UpdateTimer_Tick;
        }
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            SecondsFromBeginJointing = (int)(DateTime.Now - _jointingStartTime).TotalSeconds;
        }
    }
}
