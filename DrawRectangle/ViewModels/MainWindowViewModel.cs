using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Data;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Xps;

namespace DrawRectangle.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        // 画像
        public ReactivePropertySlim<BitmapSource> Image { get; } = new ReactivePropertySlim<BitmapSource>();
        // Canvasサイズ変更用
        public DelegateCommand<SizeChangedEventArgs> SizeChangedCommand { get; }
        public DelegateCommand<RoutedEventArgs> LoadCommand { get; }
        public ReactivePropertySlim<double> CanvasWidth { get; } = new ReactivePropertySlim<double>();
        public ReactivePropertySlim<double> CanvasHeight { get; } = new ReactivePropertySlim<double>();
        // ドラッグアンドドロップ用
        private Point _startPoint;
        public ReactiveCommand<Point> MouseMoveCommand { get; } = new ReactiveCommand<Point>();
        public ReactiveCommand<Point> MouseUpCommand { get; } = new ReactiveCommand<Point>();
        public ReactiveCommand<Point> MouseDownCommand { get; } = new ReactiveCommand<Point>();
        public ReactivePropertySlim<Rect> Rect { get; } = new ReactivePropertySlim<Rect>();
        // 実画像の選択範囲用
        private double _scale = 1;
        public ReactivePropertySlim<Rect> SelectedRect { get; } = new ReactivePropertySlim<Rect>();
        public ReactivePropertySlim<string> SelectedArea { get; } = new ReactivePropertySlim<string>();

        public MainWindowViewModel()
        {
            // サイズ変更イベント
            SizeChangedCommand = new DelegateCommand<SizeChangedEventArgs>(args =>
            {
                CanvasWidth.Value = args.NewSize.Width;
                CanvasHeight.Value = args.NewSize.Height;
                
                // 拡大率を計算
                _scale = CanvasWidth.Value / Image.Value.Width;
            });

            LoadCommand = new DelegateCommand<RoutedEventArgs>(args =>
            {
                var image = args.Source as System.Windows.Controls.Image;
                CanvasWidth.Value = image.ActualWidth;
                CanvasHeight.Value = image.ActualHeight;

                // 拡大率を計算
                _scale = CanvasWidth.Value / Image.Value.Width;
            });

            // マウスダウン時
            MouseDownCommand
                .Subscribe(point =>
                {
                    // クリック位置を記憶
                    _startPoint.X = point.X;
                    _startPoint.Y = point.Y;
                });

            // ドラッグイベント
            var drag = MouseDownCommand
                .SelectMany(MouseMoveCommand)
                .TakeUntil(MouseUpCommand)
                .Repeat()
                .Subscribe(point =>
                {
                    // 選択位置を計算
                    var x = _startPoint.X < point.X ? _startPoint.X : point.X;
                    var y = _startPoint.Y < point.Y ? _startPoint.Y : point.Y;
                    var width = Math.Abs(point.X - _startPoint.X);
                    var height = Math.Abs(point.Y - _startPoint.Y);
                    Rect.Value = new Rect(x, y, width, height);

                    // 実画像の選択範囲を計算
                    SelectedRect.Value = new Rect(x / _scale, y / _scale, width / _scale, height / _scale);
                    SelectedArea.Value = $"({SelectedRect.Value.X:0.0}, {SelectedRect.Value.Y:0.0}), ({SelectedRect.Value.Width:0.0}, {SelectedRect.Value.Height:0.0})";
                });

            Image.Value = new BitmapImage(new Uri(@"D:\test.png"));
        }
    }
}
