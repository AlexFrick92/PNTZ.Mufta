using NUnit.Framework;
using PNTZ.Mufta.TPCApp.Domain;
using PNTZ.Mufta.TPCApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNTZ.Mufta.TPCApp.Test.ViewModel
{
    [TestFixture]
    public class TqTnLenPointViewModelTests
    {
        [Test]
        public void Torque_ReturnsAbsoluteValue()
        {
            var model = new TqTnLenPoint { Torque = -12.3f };
            var vm = new TqTnLenPointViewModel(model);

            Assert.That(vm.Torque, Is.EqualTo(12.3f).Within(0.0001f));
        }

        [Test]
        public void Length_ReturnsMillimeters()
        {
            var model = new TqTnLenPoint { Length = 1.234f }; // метры
            var vm = new TqTnLenPointViewModel(model);

            Assert.That(vm.Length, Is.EqualTo(1234f).Within(0.001f));
        }

        [Test]
        public void Turns_ReturnsTurnsFromModel()
        {
            var model = new TqTnLenPoint { Turns = 42.0f };
            var vm = new TqTnLenPointViewModel(model);

            Assert.That(vm.Turns, Is.EqualTo(42.0f));
        }

        [Test]
        public void TimeStamp_ReturnsValueFromModel()
        {
            var model = new TqTnLenPoint { TimeStamp = 999 };
            var vm = new TqTnLenPointViewModel(model);

            Assert.That(vm.TimeStamp, Is.EqualTo(999));
        }

        [Test]
        public void TurnsPerMinute_ReturnsValueFromModel()
        {
            var model = new TqTnLenPoint { TurnsPerMinute = 120.5f };
            var vm = new TqTnLenPointViewModel(model);

            Assert.That(vm.TurnsPerMinute, Is.EqualTo(120.5f));
        }
    }
}
