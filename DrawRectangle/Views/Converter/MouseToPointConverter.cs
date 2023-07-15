using Reactive.Bindings.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DrawRectangle.Views.Converter
{
    internal class MouseToPointConverter : ReactiveConverter<MouseEventArgs, Point>
    {
        protected override IObservable<Point> OnConvert(IObservable<MouseEventArgs> source)
        {
            return source.Select(x => x.GetPosition((IInputElement)AssociateObject));
        }
    }
}
