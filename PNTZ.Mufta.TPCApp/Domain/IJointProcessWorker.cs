using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Domain
{
    public interface IJointProcessWorker
    {
        void Evaluate(uint result);

        event EventHandler<JointResult> PipeAppear;
        event EventHandler<EventArgs> RecordingBegun;
        event EventHandler<JointResult> RecordingFinished;
        event EventHandler AwaitForEvaluation;
        event EventHandler<TqTnLenPoint> NewTqTnLenPoint;
        event EventHandler<JointResult> JointFinished;

        bool CyclicallyListen { get; set; }

        void SetActualRecipe(JointRecipe recipe);

    }
}
